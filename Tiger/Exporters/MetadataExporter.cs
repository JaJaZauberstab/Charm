using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Tiger.Exporters;

// TODO: Clean this up
public class MetadataExporter : AbstractExporter
{
    public override void Export(Exporter.ExportEventArgs args)
    {
        Parallel.ForEach(args.Scenes, scene =>
        {
            MetadataScene metadataScene = new(scene, args);
            metadataScene.WriteToFile(args);
        });
    }
}

class MetadataScene
{
    private ConfigSubsystem _charmConfig = ConfigSubsystem.Get();
    private readonly ConcurrentDictionary<string, dynamic> _config = new();
    private readonly ExportType _exportType;
    private readonly DataExportType _dataExportType;

    public MetadataScene(ExporterScene scene, Exporter.ExportEventArgs args)
    {
        ConcurrentDictionary<string, Dictionary<string, string>> parts = new();
        _config.TryAdd("Parts", parts);
        ConcurrentDictionary<string, ConcurrentBag<JsonInstance>> instances = new();
        _config.TryAdd("Instances", instances);
        ConcurrentDictionary<string, ConcurrentBag<string>> terrainDyemaps = new ConcurrentDictionary<string, ConcurrentBag<string>>();
        _config.TryAdd("TerrainDyemaps", terrainDyemaps);

        if (ConfigSubsystem.Get().GetUnrealInteropEnabled())
        {
            SetUnrealInteropPath(ConfigSubsystem.Get().GetUnrealInteropPath());
        }

        _exportType = scene.Type;
        _dataExportType = scene.DataType;
        SetType(_exportType, _dataExportType);
        SetMeshName(scene.Name);

        _config["UnifiedAssets"] = _charmConfig.GetSingleFolderMapAssetsEnabled();
        if (_charmConfig.GetSingleFolderMapAssetsEnabled() && scene.DataType == DataExportType.Map)
        {
            _config["AssetsPath"] = Path.Join(_charmConfig.GetExportSavePath(), $"Maps/Assets/").Replace("\\", "/");
        }
        else
        {
            if (args.AggregateOutput)
                _config["AssetsPath"] = args.OutputDirectory.Replace("\\", "/");
            else
                _config["AssetsPath"] = (scene.DataType == DataExportType.Map ? args.OutputDirectory : Path.Join(args.OutputDirectory, scene.Name)).Replace("\\", "/");
        }

        foreach (var mesh in scene.StaticMeshes)
        {
            foreach (var part in mesh.Parts)
            {
                AddPart(part, part.Name);
            }
        }

        foreach (var mesh in scene.TerrainMeshes)
        {
            foreach (var part in mesh.Parts)
            {
                AddPart(part, part.Name);
            }
        }

        foreach (var meshInstanced in scene.StaticMeshInstances)
        {
            AddInstanced(meshInstanced.Key, meshInstanced.Value);
        }
        foreach (var meshInstanced in scene.EntityInstances)
        {
            AddInstanced(meshInstanced.Key, meshInstanced.Value);
        }

        foreach (ExporterEntity entityMesh in scene.Entities)
        {
            foreach (var part in entityMesh.Mesh.Parts)
            {
                AddPart(part, part.Name);
            }
        }

        foreach (var dyemaps in scene.TerrainDyemaps)
        {
            foreach (var dyemap in dyemaps.Value)
                AddTerrainDyemap(dyemaps.Key, dyemap);
        }
    }

    public void AddPart(ExporterPart part, string partName)
    {
        if (!_config["Parts"].ContainsKey(part.SubName))
        {
            _config["Parts"][part.SubName] = new Dictionary<string, string>();
        }

        _config["Parts"][part.SubName].TryAdd(partName, part.Material?.Hash ?? "");
    }

    public void SetType(ExportType type, DataExportType datatype)
    {
        _config["Type"] = type.ToString();
        _config["ExportType"] = datatype.ToString();
    }

    public void SetMeshName(string meshName)
    {
        _config["MeshName"] = meshName;
    }

    public void SetUnrealInteropPath(string interopPath)
    {
        _config["UnrealInteropPath"] = new string(interopPath.Split("\\Content").Last().ToArray()).TrimStart('\\');
        if (_config["UnrealInteropPath"] == "")
        {
            _config["UnrealInteropPath"] = "Content";
        }
    }

    public void AddInstanced(string meshHash, List<Transform> transforms)
    {
        if (!_config["Instances"].ContainsKey(meshHash))
        {
            _config["Instances"][meshHash] = new ConcurrentBag<JsonInstance>();
        }
        foreach (Transform transform in transforms)
        {
            _config["Instances"][meshHash].Add(new JsonInstance
            {
                Translation = new[] { transform.Position.X, transform.Position.Y, transform.Position.Z },
                Rotation = new[] { transform.Quaternion.X, transform.Quaternion.Y, transform.Quaternion.Z, transform.Quaternion.W },
                Scale = new[] { transform.Scale.X, transform.Scale.Y, transform.Scale.Z },
                Order = transform.Order
            });
        }
    }

    // TODO: Maybe remove?
    public void AddTerrainDyemap(string modelHash, FileHash dyemapHash)
    {
        if (!_config["TerrainDyemaps"].ContainsKey(modelHash))
        {
            _config["TerrainDyemaps"][modelHash] = new ConcurrentBag<string>();
        }
        _config["TerrainDyemaps"][modelHash].Add(dyemapHash);
    }

    public void WriteToFile(Exporter.ExportEventArgs args)
    {
        string path = args.OutputDirectory;

        if (_config["Instances"].Count == 0
            && _config["Parts"].Count == 0
            && _exportType is not ExportType.EntityPoints)
            return; //Dont export if theres nothing in the cfg (this is kind of a mess though)

        if (!args.AggregateOutput)
        {
            if (_dataExportType is DataExportType.Individual)
            {
                path = Path.Join(path, _config["MeshName"]);
            }
            else if (_dataExportType is DataExportType.Map)
            {
                path = Path.Join(path, "Maps");
            }
            //else if (_exportType is ExportType.StaticInMap or ExportType.EntityInMap)
            //{
            //    return;
            //}
        }

        // Are these needed anymore?
        // If theres only 1 part, we need to rename it + the instance to the name of the mesh (unreal imports to fbx name if only 1 mesh inside)
        //if (_config["Parts"].Count == 1)
        //{
        //    var part = _config["Parts"][_config["Parts"].Keys[0]];
        //    //I'm not sure what to do if it's 0, so I guess I'll leave that to fix it in the future if something breakes.
        //    if (_config["Instances"].Count != 0)
        //    {
        //        var instance = _config["Instances"][_config["Instances"].Keys[0]];
        //        _config["Instances"] = new ConcurrentDictionary<string, ConcurrentBag<JsonInstance>>();
        //        _config["Instances"][_config["MeshName"]] = instance;
        //    }
        //    _config["Parts"] = new ConcurrentDictionary<string, string>();
        //    _config["Parts"][_config["MeshName"]] = part;
        //}

        string s = JsonConvert.SerializeObject(_config, Formatting.Indented);
        if (_config.ContainsKey("MeshName"))
            File.WriteAllText($"{path}/{_config["MeshName"]}_info.cfg", s);
        else
            File.WriteAllText($"{path}/info.cfg", s);
    }

    public struct JsonInstance
    {
        public float[] Translation;
        public float[] Rotation;  // Quaternion
        public float[] Scale;
        public float Order;
    }

    private struct JsonDecal
    {
        public string Material;
        public float[] Origin;
        public float Scale;
        public float[] Corner1;
        public float[] Corner2;
    }

    private struct JsonMaterial
    {
        public JsonMaterial()
        {
        }

        public bool BackfaceCulling { get; set; } = true;
        public List<string> UsedScopes;
        public Dictionary<string, Dictionary<int, TexInfo>> Textures;
    }

    public struct TexInfo
    {
        public string Hash { get; set; }
        public string Dimension { get; set; }
        public bool SRGB { get; set; }
    }
}
