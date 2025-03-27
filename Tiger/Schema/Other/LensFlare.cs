using Tiger.Exporters;

namespace Tiger.Schema;

public class LensFlare : Tag<SLensFlare>
{
    public MapTransform Transform { get; set; }
    public List<FileHash> Materials { get; set; }
    public TfxFeatureRenderer FeatureType = TfxFeatureRenderer.LensFlares;

    public LensFlare(FileHash hash) : base(hash)
    {
    }

    public void LoadIntoExporter(ExporterScene scene) // Not ideal
    {
        Exporter.Get().GetGlobalScene().AddToGlobalScene(this);
        Materials = new();
        using TigerReader reader = GetReader();
        for (int i = 0; i < _tag.Entries.Count; i++)
        {
            var entry = _tag.Entries.ElementAt(reader, i);
            if (entry.Material == null) continue;
            entry.Material.RenderStage = TfxRenderStage.LensFlares;
            scene.Materials.Add(new ExportMaterial(entry.Material));
            Materials.Add(entry.Material.Hash);
        }
    }
}
