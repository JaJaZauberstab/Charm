using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tiger.Schema;
using static Tiger.Exporters.MetadataScene;

namespace Tiger.Exporters;

public class GlobalExporter : AbstractExporter
{
    private GlobalExporterScene GlobalScene => Exporter.Get().GetGlobalScene();
    private ConfigSubsystem _config => ConfigSubsystem.Get();
    private string SavePath;

    public override void Export(Exporter.ExportEventArgs args)
    {
        SavePath = args.AggregateOutput ? args.OutputDirectory : Path.Join(args.OutputDirectory, $"Maps");

        ExportAtmosphere();
        ExportLensFlares();
        ExportCubemaps();
        ExportLights();
        ExportDecals();
    }

    private void ExportAtmosphere()
    {
        if (Exporter.Get().GetOrCreateGlobalScene().TryGetItem<SMapAtmosphere>(out SMapAtmosphere atmosphere))
        {
            List<Texture> AtmosTextures = new();
            if (atmosphere.Lookup0 != null)
                AtmosTextures.Add(atmosphere.Lookup0);
            if (atmosphere.Lookup1 != null)
                AtmosTextures.Add(atmosphere.Lookup1);
            if (atmosphere.Lookup2 != null)
                AtmosTextures.Add(atmosphere.Lookup2);
            if (atmosphere.Lookup3 != null)
                AtmosTextures.Add(atmosphere.Lookup3);
            if (atmosphere.Lookup4 != null)
                AtmosTextures.Add(atmosphere.Lookup4);

            string texSavePath = $"{SavePath}/Textures/Atmosphere";
            Directory.CreateDirectory(texSavePath);

            foreach (var tex in AtmosTextures)
            {
                // Not ideal but it works
                TextureExtractor.SaveTextureToFile($"{texSavePath}/{tex.Hash}", tex.IsVolume() ? Texture.FlattenVolume(tex.GetScratchImage(true)) : tex.GetScratchImage());
                if (_config.GetS2ShaderExportEnabled())
                    Source2Handler.SaveVTEX(tex, $"{texSavePath}", "Atmosphere");
            }

            string dataSavePath = $"{SavePath}/Rendering";
            Directory.CreateDirectory(dataSavePath);

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };
            File.WriteAllText($"{dataSavePath}/Atmosphere.json", JsonConvert.SerializeObject(atmosphere, jsonSettings));
        }
    }

    private void ExportLensFlares()
    {
        if (GlobalScene.Any<LensFlare>())
        {
            ConcurrentDictionary<FileHash, LensFlareData> data = new();
            string dataSavePath = $"{SavePath}/Rendering";
            Directory.CreateDirectory(dataSavePath);

            foreach (var lensFlare in GlobalScene.GetAllOfType<LensFlare>())
            {
                var lensFlareData = data.GetOrAdd(lensFlare.Hash, _ => new LensFlareData
                {
                    Instances = new(),
                    Materials = lensFlare.Materials.Select(x => x.ToString()).ToList()
                });

                lensFlareData.Instances.Add(new JsonInstance
                {
                    Translation = new[] { lensFlare.Transform.Translation.X, lensFlare.Transform.Translation.Y, lensFlare.Transform.Translation.Z },
                    Rotation = new[] { lensFlare.Transform.Rotation.X, lensFlare.Transform.Rotation.Y, lensFlare.Transform.Rotation.Z, lensFlare.Transform.Rotation.W },
                    Scale = new[] { lensFlare.Transform.Translation.W, lensFlare.Transform.Translation.W, lensFlare.Transform.Translation.W }
                });
            }

            File.WriteAllText($"{dataSavePath}/LensFlares.json", JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }

    private void ExportCubemaps()
    {
        if (GlobalScene.Any<Cubemap>())
        {
            ConcurrentDictionary<string, CubemapData> data = new();
            string dataSavePath = $"{SavePath}/Rendering";
            Directory.CreateDirectory(dataSavePath);

            List<Texture> textures = new List<Texture>();
            string texSavePath = $"{SavePath}/Textures/Cubemaps";
            Directory.CreateDirectory(texSavePath);

            foreach (var cubemapEntry in GlobalScene.GetAllOfType<Cubemap>())
            {
                var cubemap = cubemapEntry.CubemapEntry;
                string name = cubemap.CubemapName != null ? cubemap.CubemapName.Value : $"Cubemap_{cubemap.WorldID:X}";
                _ = data.GetOrAdd(name, _ => new CubemapData
                {
                    Transform = new JsonInstance
                    {
                        Translation = new[] { cubemap.CubemapPosition.X, cubemap.CubemapPosition.Y, cubemap.CubemapPosition.Z },
                        Rotation = new[] { cubemap.CubemapRotation.X, cubemap.CubemapRotation.Y, cubemap.CubemapRotation.Z, cubemap.CubemapRotation.W },
                        Scale = new[] { cubemap.CubemapSize.X, cubemap.CubemapSize.Y, cubemap.CubemapSize.Z, }
                    },
                    CubemapTexture = cubemap.CubemapTexture != null ? cubemap.CubemapTexture.Hash : "",
                    CubemapIBLTexture = cubemap.CubemapIBLTexture != null ? cubemap.CubemapIBLTexture.Hash : "",
                });

                if (cubemap.CubemapTexture != null)
                    textures.Add(cubemap.CubemapTexture);
                if (cubemap.CubemapIBLTexture != null)
                    textures.Add(cubemap.CubemapIBLTexture);
            }

            foreach (var tex in textures)
            {
                TextureExtractor.SaveTextureToFile($"{texSavePath}/{tex.Hash}", tex.IsVolume() ? Texture.FlattenVolume(tex.GetScratchImage(true)) : Texture.FlattenCubemap(tex.GetScratchImage()));
                if (_config.GetS2ShaderExportEnabled())
                    Source2Handler.SaveVTEX(tex, $"{texSavePath}", "Cubemaps");
            }

            File.WriteAllText($"{dataSavePath}/Cubemaps.json", JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }

    private void ExportLights()
    {
        if (GlobalScene.Any<Lights.LightData>())
        {
            ConcurrentDictionary<string, LightData> data = new();
            string dataSavePath = $"{SavePath}/Rendering";
            Directory.CreateDirectory(dataSavePath);

            List<Texture> textures = new List<Texture>();
            string texSavePath = $"{SavePath}/Textures/Lights";
            Directory.CreateDirectory(texSavePath);

            foreach (var light in GlobalScene.GetAllOfType<Lights.LightData>())
            {
                // this is so stupid...but ensures theres no accidental overwrites with mismatches
                var hash = Helpers.Fnv($"{light.LightType}{light.Material}{light.Hash}");
                var lightData = data.GetOrAdd($"{light.LightType}_{hash.ToString("X")}", _ => new LightData
                {
                    Type = light.LightType.ToString(),
                    Instances = new(),
                    Color = new[] { light.Color.X, light.Color.Y, light.Color.Z, light.Color.W },
                    Attenuation = light.Attenuation,
                    Cookie = light.Cookie ?? "",
                    Bytecode = light.Bytecode.ToList(),
                    BytecodeConstants = light.BytecodeConstants,
                    Hashes = new[] { $"{light.Hash}", $"{light.Material}" }
                });

                lightData.Instances.Add(new JsonInstance
                {
                    Translation = new[] { light.Transform.Position.X, light.Transform.Position.Y, light.Transform.Position.Z },
                    Rotation = new[] { light.Transform.Quaternion.X, light.Transform.Quaternion.Y, light.Transform.Quaternion.Z, light.Transform.Quaternion.W },
                    Scale = new[] { light.Size.X, light.Size.Y, light.Size.Z },
                });

                if (light.Cookie != null)
                    textures.Add(new Texture(light.Cookie));
            }

            foreach (var tex in textures)
            {
                tex.SavetoFile($"{texSavePath}/{tex.Hash}");
                if (_config.GetS2ShaderExportEnabled())
                    Source2Handler.SaveVTEX(tex, $"{texSavePath}", "Lights");
            }
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };
            File.WriteAllText($"{dataSavePath}/Lights.json", JsonConvert.SerializeObject(data, jsonSettings));
        }
    }

    private void ExportDecals()
    {
        if (GlobalScene.Any<Decals>())
        {
            ConcurrentDictionary<FileHash, DecalsData> data = new();
            string dataSavePath = $"{SavePath}/Rendering";
            Directory.CreateDirectory(dataSavePath);

            foreach (var decal in GlobalScene.GetAllOfType<Decals>())
            {
                List<Transform> transforms = decal.GetTransforms();
                foreach (var instance in decal.TagData.DecalResources.Enumerate(decal.GetReader()))
                {
                    for (int i = instance.StartIndex; i < instance.StartIndex + instance.Count; i++)
                    {
                        var loc = transforms[i].Position;
                        var rot = transforms[i].Quaternion;
                        var scale = transforms[i].Scale;

                        var decalData = data.GetOrAdd(instance.Material.Hash, _ => new DecalsData
                        {
                            Instances = new(),
                        });

                        decalData.Instances.Add(new JsonInstance
                        {
                            Translation = new[] { loc.X, loc.Y, loc.Z },
                            Rotation = new[] { rot.X, rot.Y, rot.Z, rot.W },
                            Scale = new[] { scale.X, scale.Y, scale.Z }
                        });
                    }
                }
            }

            File.WriteAllText($"{dataSavePath}/Decals.json", JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }

    private struct DecalsData
    {
        public List<JsonInstance> Instances;
    }

    private struct LensFlareData
    {
        public List<JsonInstance> Instances;
        public List<string> Materials;
    }

    private struct CubemapData
    {
        public JsonInstance Transform;
        public string CubemapTexture;
        public string CubemapIBLTexture;
    }

    private struct LightData
    {
        public string Type;
        public List<JsonInstance> Instances;
        public float[] Color;
        public float Attenuation;
        public string Cookie;
        public List<byte> Bytecode;
        public Vector4[] BytecodeConstants;
        public string[] Hashes; // Stupid but could be useful for debugging/finding if needed
    }
}
