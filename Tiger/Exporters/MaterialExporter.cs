using ConcurrentCollections;
using Tiger.Schema;

namespace Tiger.Exporters;

// TODO: Clean this up
public class MaterialExporter : AbstractExporter
{
    public override void Export(Exporter.ExportEventArgs args)
    {
        var _config = ConfigSubsystem.Get();

        ConcurrentHashSet<Texture> mapTextures = new();
        ConcurrentHashSet<ExportMaterial> mapMaterials = new();
        ConcurrentHashSet<ExportMaterial> materials = new();

        bool saveMats = _config.GetExportMaterials();

        Parallel.ForEach(args.Scenes, scene =>
        {
            if (scene.Type is ExportType.Entity or ExportType.Static or ExportType.API or ExportType.D1API)
            {
                ConcurrentHashSet<Texture> textures = scene.Textures;

                foreach (ExportMaterial material in scene.Materials)
                {
                    materials.Add(material);
                    foreach (STextureTag texture in material.Material.Vertex.EnumerateTextures())
                    {
                        if (texture.GetTexture() == null)
                            continue;

                        textures.Add(texture.GetTexture());
                    }
                    foreach (STextureTag texture in material.Material.Pixel.EnumerateTextures())
                    {
                        if (texture.GetTexture() == null)
                            continue;

                        textures.Add(texture.GetTexture());
                    }
                }

                string textureSaveDirectory = args.AggregateOutput ? args.OutputDirectory : Path.Join(args.OutputDirectory, scene.Name);
                textureSaveDirectory = $"{textureSaveDirectory}/Textures";

                Directory.CreateDirectory(textureSaveDirectory);
                foreach (Texture texture in textures)
                {
                    texture.SavetoFile($"{textureSaveDirectory}/{texture.Hash}");
                }
                foreach (ExportMaterial material in materials)
                {
                    string shaderSaveDirectory = args.AggregateOutput ? args.OutputDirectory : Path.Join(args.OutputDirectory, scene.Name);
                    Directory.CreateDirectory(shaderSaveDirectory);
                    material.Material.Export(shaderSaveDirectory);
                }
            }
            else
            {
                mapTextures.UnionWith(scene.Textures);
                foreach (ExportMaterial material in scene.Materials)
                {
                    mapMaterials.Add(material);
                    foreach (STextureTag texture in material.Material.Vertex.EnumerateTextures())
                    {
                        if (texture.GetTexture() == null)
                            continue;

                        mapTextures.Add(texture.GetTexture());
                    }
                    foreach (STextureTag texture in material.Material.Pixel.EnumerateTextures())
                    {
                        if (texture.GetTexture() == null)
                            continue;

                        mapTextures.Add(texture.GetTexture());
                    }
                }
            }
        });

        foreach (Texture texture in mapTextures)
        {
            if (texture is null)
                continue;

            string textureSaveDirectory = args.AggregateOutput ? args.OutputDirectory : Path.Join(args.OutputDirectory, $"Maps");
            textureSaveDirectory = $"{textureSaveDirectory}/Textures";
            Directory.CreateDirectory(textureSaveDirectory);

            texture.SavetoFile($"{textureSaveDirectory}/{texture.Hash}");
        }

        if (saveMats)
        {
            foreach (ExportMaterial material in mapMaterials)
            {
                string shaderSaveDirectory = args.AggregateOutput ? args.OutputDirectory : Path.Join(args.OutputDirectory, $"Maps");
                Directory.CreateDirectory(shaderSaveDirectory);
                material.Material.Export(shaderSaveDirectory);
            }
        }
    }
}
