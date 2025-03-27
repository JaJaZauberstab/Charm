using Tiger.Exporters;
using Tiger.Schema.Entity;

namespace Tiger.Schema;

public class WaterDecals
{
    public TfxFeatureRenderer FeatureType = TfxFeatureRenderer.Water;
    public SMapWaterDecal Water;
    public MapTransform Transform;
    public WaterDecals(SMapWaterDecal entry)
    {
        Water = entry;
    }

    public void LoadIntoExporter(ExporterScene scene)
    {
        scene.AddMapModel(Water.Model, Transform.Translation, Transform.Rotation, new Tiger.Schema.Vector3(Transform.Translation.W));

        foreach (DynamicMeshPart part in Water.Model.Load(ExportDetailLevel.MostDetailed, null))
        {
            if (part.Material == null) continue;
            scene.Materials.Add(new ExportMaterial(part.Material));
        }
    }
}
