using Tiger.Exporters;
using Tiger.Schema.Entity;

namespace Tiger.Schema;

public class Cubemap
{
    public TfxFeatureRenderer FeatureType = TfxFeatureRenderer.Cubemaps;
    public SMapCubemapResource CubemapEntry;
    public MapTransform CubemapTransform { get; set; }

    public Cubemap(SMapCubemapResource cubemap)
    {
        CubemapEntry = cubemap;
    }

    public void LoadIntoExporter()
    {
        Exporter.Get().GetGlobalScene().AddToGlobalScene(this);
    }
}
