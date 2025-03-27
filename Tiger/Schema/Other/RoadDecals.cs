using Tiger.Exporters;
using Tiger.Schema.Entity;

namespace Tiger.Schema;

public class RoadDecals : Tag<SMapRoadDecals>
{
    public TfxFeatureRenderer FeatureType = TfxFeatureRenderer.RoadDecals;
    public RoadDecals(FileHash hash) : base(hash)
    {

    }

    public void LoadIntoExporter(ExporterScene scene)
    {
        foreach (var a in _tag.Entries)
        {
            Transform transform = new Transform
            {
                Position = a.Position.ToVec3(),
                Quaternion = a.Rotation,
                Rotation = Vector4.QuaternionToEulerAngles(a.Rotation),
                Scale = new(a.Position.W)
            };

            var len = a.IndexCount * 3; //  Is actually face count
            var part = MeshPart.CreateFromBuffers<DynamicMeshPart>(a.IndexBuffer, a.VertexBuffer, a.Material, PrimitiveType.Triangles, 9, (uint)len, a.IndexOffset);
            part.TransformPosition(a.Offset, a.Scale);
            part.TransformTexcoord(a.TexcoordOffset, a.TexcoordScale);

            scene.AddMapModelParts($"{a.VertexBuffer.Hash}", new List<MeshPart> { part }, transform);
            scene.Materials.Add(new ExportMaterial(part.Material));
        }
    }
}
