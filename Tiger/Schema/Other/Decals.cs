using Tiger.Exporters;

namespace Tiger.Schema;

public class Decals : Tag<SMapDecals>
{
    public TfxFeatureRenderer FeatureType = TfxFeatureRenderer.Decals;
    public Decals(FileHash hash) : base(hash)
    {
    }

    public void LoadIntoExporter(ExporterScene scene)
    {
        Exporter.Get().GetGlobalScene().AddToGlobalScene(this);

        foreach (var instance in _tag.DecalResources.Enumerate(GetReader()))
        {
            for (int i = instance.StartIndex; i < instance.StartIndex + instance.Count; i++)
            {
                if (instance.Material is null)
                    continue;

                instance.Material.RenderStage = TfxRenderStage.Decals;
                scene.Materials.Add(new(instance.Material));
            }
        }
    }

    public void DebugExport(string savePath)
    {
        List<Vector4> cube = GetCube();
        List<Transform> transforms = GetTransforms();
        int j = 0;
        foreach (var transform in transforms)
        {
            List<Vector4> transformedCubes = ApplyTransformsToCube(cube, transform);
            ExportCube($"{savePath}\\cube_{j}.obj", transformedCubes);
            j++;
        }
    }

    public List<Transform> GetTransforms()
    {
        using TigerReader reader = _tag.Transforms.GetReferenceReader();
        var stride = _tag.Transforms.TagData.Stride;
        List<Transform> transforms = new();

        for (int i = 0; i < reader.BaseStream.Length / stride; i++)
        {
            reader.BaseStream.Seek(i * stride, SeekOrigin.Begin);
            var pos = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1); // format R32g32b32Float, stride 0xC
            var rot = new Vector4(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16()); // format R16g16b16a16Snorm, stride 0x8
            var scale = new Vector4(reader.ReadHalf(), reader.ReadHalf(), reader.ReadHalf(), reader.ReadHalf()); // format R16g16b16a16Float, stride 0x8

            transforms.Add(new()
            {
                Position = pos.ToVec3(),
                Quaternion = rot,
                Scale = scale.ToVec3()
            });
        }

        return transforms;
    }

    public List<Vector4> GetCube()
    {
        using TigerReader reader = _tag.Cube.GetReferenceReader();

        int vertexCount = (int)(reader.BaseStream.Length / 4); // triangle list
        Vector4[] cubePoints = new Vector4[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            cubePoints[i] = new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        return cubePoints.ToList();
    }

    public List<Vector4> ApplyTransformsToCube(List<Vector4> cubeVertices, Transform transform)
    {
        List<Vector4> transformedVertices = new List<Vector4>();

        Vector3 position = transform.Position;
        Vector4 rotation = transform.Quaternion;
        Vector3 scale = transform.Scale;

        foreach (var vertex in cubeVertices)
        {
            // Scale
            Vector3 scaledVertex = new Vector3(vertex.X * scale.X, vertex.Y * scale.Y, vertex.Z * scale.Z);

            // Rotate
            Vector3 rotatedVertex = Vector3.Transform(scaledVertex, rotation);

            // Translate
            Vector4 finalVertex = new Vector4(rotatedVertex.X + position.X, rotatedVertex.Y + position.Y, rotatedVertex.Z + position.Z, 1);

            transformedVertices.Add(finalVertex);
        }


        return transformedVertices;
    }

    public void ExportCube(string filePath, List<Vector4> cubePoints)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write vertices
            foreach (var point in cubePoints)
            {
                writer.WriteLine($"v {point.X} {point.Y} {point.Z}");
            }

            // Write faces (each 3 vertices form a triangle)
            for (int i = 0; i < 36; i += 3)
            {
                writer.WriteLine($"f {i + 1} {i + 2} {i + 3}");
            }
        }

        Console.WriteLine($"Cube exported to {filePath}");
    }
}
