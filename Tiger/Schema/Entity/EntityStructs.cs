﻿using Tiger.Schema.Audio;
using Tiger.Schema.Investment;
using Tiger.Schema.Model;
using Tiger.Schema.Shaders;

namespace Tiger.Schema.Entity;

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "34078080", 0xA8)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "0F9C8080", 0xA0)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "D89A8080", 0x98)]
public struct SEntity
{
    public long FileSize;
    //[SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON)]
    //[SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    //public ResourcePointer Unk08; // D1 06098080

    [SchemaField(0x20, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x10, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x08, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public DynamicArrayUnloaded<D2Class_CD9A8080> EntityResources;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "61038080", 8)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "259C8080", 8)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "F09A8080", 8)]
public struct D2Class_F09A8080
{
    public TigerHash Unk00;
    public ushort Unk04;
    public ushort Unk06;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "70038080", 0x28)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "229C8080", 0x28)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "ED9A8080", 0x28)]
public struct D2Class_ED9A8080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "90078080", 0x18)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "209C8080", 0x18)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "EB9A8080", 0x18)]
public struct D2Class_EB9A8080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "06008080", 0x2)]
public struct D2Class_06008080
{
    public short Unk0;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "15078080", 0xC)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "049C8080", 0xC)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "CD9A8080", 0xC)]
public struct D2Class_CD9A8080  // entity resource entry
{
    public FileHash Resource; // Can sometimes be a non-entity resource in D1, for whatever reason
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "61088080", 0xA0)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "369C8080", 0xA0)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "069B8080", 0xA0)]
public struct D2Class_069B8080  // Entity resource
{
    public long FileSize;

    [SchemaField(0x10)]
    public ResourcePointer Unk10; // this isnt any of the ones in Entity.Load in beyond light
    public ResourcePointer Unk18;


    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x80, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public Tag UnkHash80;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public Tag UnkHash84;  // 819A8080
    // Rest is unknown
}


/*
 * The external material map provides the mapping of external material index -> material tag
 * could be these external materials are dynamic themselves - we'll extract them all but select the first
 */
[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "9C1A8080", 0x290)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "BD728080", 0x340)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "8F6D8080", 0x3E0)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "8F6D8080", 0x450)]
public struct D2Class_8F6D8080
{
    [SchemaField(0x15C, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x1DC, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x224, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public EntityModel Model;

    [SchemaField(0x1A8, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public DynamicArray<S121B8080> TexturePlatesROI;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x310, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]  // todo shadowkeep, beyond light
    public Tag<D2Class_1C6E8080> TexturePlates;

    [SchemaField(0x230, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x2D0, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x360, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x3C0, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public DynamicArrayUnloaded<SExternalMaterialMapEntry> ExternalMaterialsMap;

    [SchemaField(0x260, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x300, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x398, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x3F0, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public DynamicArrayUnloaded<D2Class_986D8080> Unk3F0;

    [SchemaField(0x270, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x310, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x3A0, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x400, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public DynamicArrayUnloaded<D2Class_14008080> ExternalMaterials;
}

// Physics model resource, same layout as normal model resource?
[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "F61B8080", 0x840)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "86728080", 0x360)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "6C6D8080", 0x480)]
public struct D2Class_6C6D8080
{
    [SchemaField(0x15C, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x1DC, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x224, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public EntityModel PhysicsModel;

    [SchemaField(0x230, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x2D0, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x360, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x3C0, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public DynamicArrayUnloaded<SExternalMaterialMapEntry> ExternalMaterialsMap;

    //[SchemaField(0x260, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    //[SchemaField(0x300, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    //[SchemaField(0x398, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    //[SchemaField(0x3F0, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    //public DynamicArrayUnloaded<D2Class_986D8080> Unk3F0;

    [SchemaField(0x270, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x310, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x3A0, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x400, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public DynamicArrayUnloaded<D2Class_14008080> ExternalMaterials;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "121B8080", 0x30)]
public struct S121B8080
{
    [SchemaField(0x28)]
    public Tag<D2Class_1C6E8080> TexturePlates;
}

#region Texture Plates

/// <summary>
/// Texture plate header that stores all the texture plates used for the EntityModel.
/// </summary>
[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "3C1C8080", 0x30)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "1C6E8080", 0x38)]
public struct D2Class_1C6E8080
{
    public long FileSize;

    [SchemaField(0x24, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x28, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public TexturePlate AlbedoPlate;
    public TexturePlate NormalPlate;
    public TexturePlate GStackPlate;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public TexturePlate DyemapPlate;
}

/// <summary>
/// Texture plate that stores the data for placing textures on a canvas.
/// </summary>
[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "47018080", 0x20)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "919E8080", 0x20)]
public struct D2Class_919E8080
{
    public long FileSize;
    [SchemaField(0x10)]
    public DynamicArrayUnloaded<D2Class_939E8080> PlateTransforms;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "63018080", 0x14)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "939E8080", 0x14)]
public struct D2Class_939E8080
{
    public Texture Texture;
    public IntVector2 Translation;
    public IntVector2 Scale;
}

#endregion

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "FE1A8080", 0x8)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "C5728080", 0x8)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "986D8080", 0x8)]
public struct D2Class_986D8080
{
    public ushort Unk00;
    public ushort Unk02;
    public ushort Unk04;
    public ushort Unk06;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "841A8080", 0xC)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "C4728080", 0xC)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "976D8080", 0xC)]
public struct SExternalMaterialMapEntry
{
    public int MaterialCount;
    public int MaterialStartIndex;
    public int Unk08;  // maybe some kind of LOD or dynamic marker
}

[SchemaStruct("14008080", 0x4)]
public struct D2Class_14008080
{
    public Material Material;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "4E058080", 0x5C)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "8F9A8080", 0x38)]
public struct D2Class_8F9A8080
{
    // public InlineGlobalPointer Unk0;
    [SchemaField(0x8, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x10, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public TigerHash Unk10;
    // public InlineGlobalPointer Unk18;
    [SchemaField(0x30, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x28, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public TigerHash Unk28;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "801A8080", 0x1D0)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "B8728080", 0x200)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "8A6D8080", 0x2E0)]
public struct D2Class_8A6D8080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "BD068080", 0x100)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "45858080", 0x100)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "DD818080", 0x100)]
public struct D2Class_DD818080
{
    public ResourceInTagPointer Unk00;
    [SchemaField(0x30)]
    public DynamicArray<D2Class_DC818080> Unk30;
    public DynamicArray<D2Class_40868080> Unk40;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "98058080", 0x10)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "DC818080", 0x40)]
public struct D2Class_DC818080
{
    //public ResourceInTagPointer Unk00;
    //public ResourcePointer Unk10;
    [SchemaField(0x0, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x20, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public DynamicArray<D2Class_4F9F8080> Unk20;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "B0018080", 0x20)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "759F8080", 0x20)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "4F9F8080", 0x20)]
public struct D2Class_4F9F8080
{
    public Tiger.Schema.Vector4 Rotation;
    public Tiger.Schema.Vector4 Translation;
}

[SchemaStruct("40868080", 8)]
public struct D2Class_40868080
{
    public ushort Unk00;
    public ushort Unk02;
    public uint Unk04;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "9A048080", 0xE0)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "46858080", 0xF0)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "DE818080", 0x108)]
public struct D2Class_DE818080
{
    //public ResourceInTagPointer Unk00;
    //[SchemaField(0x48)]
    //public ResourcePointer Unk48;
    //public Tag Unk50;  // 239B8080 WQ, 549C8080 SK
    //[SchemaField(0x60, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    //[SchemaField(0x68, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    //public ResourcePointer Unk68;
    //public Tag Unk70;  // 239B8080 WQ, 549C8080 SK
    [SchemaField(0x70, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x78, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x88, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public TigerHash Unk88;
    public TigerHash Unk8C;  // this is actually zeros in SK
    [SchemaField(0x88, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x80, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x90, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public DynamicArrayUnloaded<D2Class_42868080> NodeHierarchy;
    public DynamicArrayUnloaded<D2Class_4F9F8080> DefaultObjectSpaceTransforms;
    public DynamicArrayUnloaded<D2Class_4F9F8080> DefaultInverseObjectSpaceTransforms;
    public DynamicArrayUnloaded<D2Class_06008080> RangeIndexMap;
    public DynamicArrayUnloaded<D2Class_06008080> InnerIndexMap;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public Vector4 UnkE0;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0xD8, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0xF0, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public DynamicArrayUnloaded<D2Class_E1818080> UnkF0; // lod distance?
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "F4048080", 0x10)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "088A8080", 0x10)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "42868080", 0x10)]
public struct D2Class_42868080
{
    public TigerHash NodeHash;
    public int ParentNodeIndex;
    public int FirstChildNodeIndex;
    public int NextSiblingNodeIndex;
}

[SchemaStruct("E1818080", 0x18)]
public struct D2Class_E1818080
{
    public ResourceInTagPointer Unk00;
    public long Unk10;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "B51A8080", 0x44)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "A5738080", 0xA0)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "076F8080", 0xA0)]
public struct SEntityModel  // Entity model
{
    public long FileSize;
    [SchemaField(0x10)]
    public DynamicArrayUnloaded<SEntityModelMesh> Meshes;
    [SchemaField(0x20)]
    public Vector4 Unk20;
    public long Unk30;
    [SchemaField(0x38)]
    public long UnkFlags38;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)] // Model transforms are stored in SEntityModelMesh for D1
    [SchemaField(0x50, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public Vector4 ModelScale;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public Vector4 ModelTranslation;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public Vector2 TexcoordScale;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public Vector2 TexcoordTranslation;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public Vector4 Unk80;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public TigerHash Unk90;
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public TigerHash Unk94;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "BF1B8080", 0xA0)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "78738080", 0x88)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "C56E8080", 0x80)]
public struct SEntityModelMesh
{
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public Vector4 ModelScale;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public Vector4 ModelTranslation;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public Vector2 TexcoordScale;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public Vector2 TexcoordTranslation;

    [SchemaField(0x30, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public VertexBuffer Vertices1;  // vert file 1 (positions)
    public VertexBuffer Vertices2;  // vert file 2 (texcoords/normals)
    public VertexBuffer OldWeights;  // old weights
    public TigerHash Unk0C;  // nothing ever
    public IndexBuffer Indices;  // indices

    [SchemaField(TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public VertexBuffer VertexColour;  // vertex colour

    [SchemaField(TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public VertexBuffer SinglePassSkinningBuffer;  // single pass skinning buffer
    public int Zeros1C;
    public DynamicArrayUnloaded<D2Class_CB6E8080> Parts;

    /// Range of parts to render per render stage
    /// Can be obtained as follows:
    ///
    ///     - Start = part_range_per_render_stage[stage]
    ///     - End = part_range_per_render_stage[stage + 1]
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, ArraySizeConst = 20)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, ArraySizeConst = 24)]
    [SchemaField(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, ArraySizeConst = 25)] // ArraySizeConst being the number of elements
    public short[] PartRangePerRenderStage;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, ArraySizeConst = 19)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, ArraySizeConst = 24)]
    public byte[] InputLayoutPerRenderStageBL;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, ArraySizeConst = 23)]
    [SchemaField(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, Obsolete = true)]
    public short[] InputLayoutPerRenderStageSK;

    public Range GetRangeForStage(int stage)
    {
        int start = PartRangePerRenderStage[stage];
        int end = PartRangePerRenderStage[stage + 1];
        return new Range(start, end);
    }

    public int GetInputLayoutForStage(int stage)
    {
        return Strategy.IsPreBL() ? InputLayoutPerRenderStageSK[stage] : InputLayoutPerRenderStageBL[stage];
    }
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "EF1A8080", 0x24)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "7E738080", 0x20)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "CB6E8080", 0x24)]
public struct D2Class_CB6E8080  // TODO use DCG to figure out what this is
{
    public Material Material;  // AA6D8080
    public short VariantShaderIndex;  // variant_shader_index
    public short PrimitiveType;
    public uint IndexOffset;
    public uint IndexCount;
    public uint Unk10;  // might be number of strips?

    [SchemaField(0x18, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x14, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public short ExternalIdentifier;  // external_identifier
    public byte Unk16;
    public byte Unk17;

    // need to check this on WQ, theres no way its an int
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public int FlagsD2;

    [SchemaField(0x1C, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public short FlagsD1; //??

    [SchemaField(0x1E, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x1A, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x1C, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public byte GearDyeChangeColorIndex;   // sbyte gear_dye_change_color_index
    public ELodCategory LodCategory;
    public byte Unk1E;
    public byte LodRun;  // lod_run
    [SchemaField(TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public int Unk20; // variant_shader_index?

    public int GetFlags()
    {
        if (Strategy.IsD1())
            return FlagsD1;
        else
            return FlagsD2;
    }
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "791A8080", 0x210)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "73728080", 0x240)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "5B6D8080", 0x320)]
public struct D2Class_5B6D8080
{
    // Full of relative pointer shit
    // Tables start at 0x1f0
    //[SchemaField(0x210)]
    //public DynamicArray<D2Class_0B008080> Unk210;
    //[SchemaField(0x220)]
    //public DynamicArray<D2Class_D99E8080> Unk220;
    // there are more tables
}

[SchemaStruct("0B008080", 4)]
public struct D2Class_0B008080
{
    public uint Unk00;
}

[SchemaStruct("668B8080", 0x70)]
public struct D2Class_668B8080
{
    [SchemaField(0x30)]
    public DynamicArrayUnloaded<D2Class_628B8080> Unk30;
}

[SchemaStruct("628B8080", 0x30)]
public struct D2Class_628B8080
{
    public Vector4 Unk00;
}

[SchemaStruct("0F008080", 4)]
public struct D2Class_0F008080
{
    public float Unk00;
}

[SchemaStruct("90008080", 0x10)]
public struct D2Class_90008080
{
    public Vector4 Unk00;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "9B208080", 0x330)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "DA5E8080", 0x150)]
public struct D2Class_DA5E8080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "89208080", 0x270)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "DB5E8080", 0x240)]
public struct D2Class_DB5E8080
{
    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x108, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public Tag<D2Class_23978080> Unk108;

    [SchemaField(0x114, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_WITCHQUEEN_6307, Obsolete = true)]
    public StringHash EntityName;
}

[SchemaStruct("23978080", 0x48)]
public struct D2Class_23978080
{
    public long FileSize;
    public StringHash EntityName;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "63268080", 0xC60)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "1C888080", 0x50)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "12848080", 0x50)]
public struct D2Class_12848080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "08278080", 0x598)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "18888080", 0x90)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "0E848080", 0xA0)]
public struct D2Class_0E848080
{
    [SchemaField(0x100, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public DynamicArray<S712B8080> Unk100;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x78, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x88, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public DynamicArray<D2Class_1B848080> Unk88;
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "20888080", 0x18)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "1B848080", 0x18)]
[SchemaStruct(TigerStrategy.DESTINY2_LATEST, "1B848080", 0x38)]
public struct D2Class_1B848080
{
    [SchemaField(0x8, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x28, TigerStrategy.DESTINY2_LATEST)]
    public DynamicArray<D2Class_1D848080> Unk08;
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "22888080", 0x8)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "1D848080", 0x18)]
public struct D2Class_1D848080
{
    public int Unk00;
    public int Unk04;

    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, Tag64 = true)]
    public Tag Entity;
}

[SchemaStruct("07008080", 4)]
public struct D2Class_07008080
{
    public uint Unk00;
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "81888080", 0xEC)]
public struct D2Class_81888080
{
    [SchemaField(0x74)]
    public Tag Entity;
}

// General, parents that reference Entity

[SchemaStruct("30898080", 0x28)]
public struct D2Class_30898080
{
    public long FileSize;
    public DynamicArray<D2Class_34898080> Unk08;
    public DynamicArray<D2Class_33898080> Unk18;
}

[SchemaStruct("34898080", 0x20)]
public struct D2Class_34898080
{
}

[SchemaStruct("33898080", 0x20)]
public struct D2Class_33898080
{
    public StringPointer TagPath;
    [SchemaField(Tag64 = true)]
    public Tag Tag;  // if .pattern.tft, then Entity - if .budget_set.tft, then parent of itself
    public StringPointer TagNote;
}

[SchemaStruct("ED9E8080", 0x58)]
public struct D2Class_ED9E8080
{
    public long FileSize;
    [SchemaField(0x18)]
    public Tag Unk18;
    [SchemaField(0x28)]
    public DynamicArray<D2Class_F19E8080> Unk28;
}

[SchemaStruct("F19E8080", 0x18)]
public struct D2Class_F19E8080
{
    public StringPointer TagPath;
    [SchemaField(0x8, Tag64 = true)]
    public Tag Tag;  // if .pattern.tft, then Entity
}

[SchemaStruct("7E988080", 8)]
public struct D2Class_7E988080
{
    public Tag Unk00;
    public Tag Unk08;
}

[SchemaStruct("44318080", 8)]
public struct D2Class_44318080
{
    public long FileSize;
    [SchemaField(0x8, Tag64 = true)]
    public Entity? Entity;
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "3B9A8080", 0x50)]
public struct D2Class_3B9A8080
{
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "8F948080", 0xC8)]
public struct D2Class_8F948080
{
    [SchemaField(0xA8)]
    public DynamicArray<D2Class_56838080> UnkA8;
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "56838080", 0x68)]
public struct D2Class_56838080
{
    [SchemaField(0x8)]
    public DynamicArray<D2Class_58838080> Table1; // Why...Are these all the same...?
    public DynamicArray<D2Class_58838080> Table2;
    public DynamicArray<D2Class_58838080> Table3;
    public DynamicArray<D2Class_58838080> Table4;
    public DynamicArray<D2Class_58838080> Table5;
    public DynamicArray<D2Class_58838080> Table6;
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "58838080", 0x18)]
public struct D2Class_58838080
{
    public ResourceInTablePointer<SMapDataEntry>? Datatable;
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "B67E8080", 0x34)]
public struct D2Class_B67E8080
{
    [SchemaField(0x20)]
    public StringHash EntityName;
}

#region Named entities

//I think this is the old struct for named bags, it seems like it changed to 1D478080?

//[SchemaStruct("C96C8080", 0x50)]
//public struct D2Class_75988080
//{
//    public long FileSize;
//    // [DestinyField(FieldType.RelativePointer)]
//    // public string DestinationGlobalTagBagName;
//    public FileHash DestinationGlobalTagBag;
//    // [SchemaField(0x20)]
//    // public FileHash PatrolTable1;
//    // [SchemaField(0x28), DestinyField(FieldType.RelativePointer)]
//    // public string PatrolTableName;
//    // public FileHash PatrolTable2;
//}

[SchemaStruct("1D478080", 0x18)]
public struct D2Class_1D478080
{
    public long FileSize;
    public DynamicArray<D2Class_D3598080> DestinationGlobalTagBags;
}

[SchemaStruct("D3598080", 0x10)]
public struct D2Class_D3598080
{
    public FileHash DestinationGlobalTagBag;
    [SchemaField(0x8)]
    public StringPointer DestinationGlobalTagBagName;
}

#endregion

#region Audio

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "80258080", 0x598)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "6E358080", 0x6b8)]
[SchemaStruct(TigerStrategy.DESTINY2_LATEST, "6E358080", 0x6d8)]
public struct D2Class_6E358080
{
    [SchemaField(0x538, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x648, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    [SchemaField(0x668, TigerStrategy.DESTINY2_LATEST)]
    public DynamicArray<D2Class_9B318080> PatternAudioGroups;

    [SchemaField(0x4E8, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_WITCHQUEEN_6307, Obsolete = true)]
    [SchemaField(0x610, TigerStrategy.DESTINY2_LATEST)] // unsure if actually tag64
    public Tag<D2Class_A36F8080> FallbackAudioGroup;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "DE318080", 0xD8)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "9B318080", 0x128)]
public struct D2Class_9B318080
{
    public TigerHash WeaponContentGroup1Hash;
    [SchemaField(0x8)]
    public TigerHash Unk08;
    //[SchemaField(0x18, Tag64 = true)]
    //public FileHash StringContainer;  // idk why but i presume debug strings, not important

    [SchemaField(0x20, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x28, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public TigerHash WeaponContentGroup2Hash;  // "weaponContentGroupHash" from API
    // theres other stringcontainer stuff but skipping it

    [SchemaField(0x40, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_WITCHQUEEN_6307, Obsolete = true)]
    public Entity? WeaponSkeletonEntityD1;

    [SchemaField(0xA0, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public Entity? WeaponSkeletonEntityD2;

    [SchemaField(0x88, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0xD0, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public Tag<D2Class_A36F8080> AudioGroup;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "960C8080", 0x18)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "0D8C8080", 0x18)]
public struct D2Class_0D8C8080
{
    public long FileSize;
    public DynamicArray<D2Class_0F8C8080> Audio;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "FD038080", 0x18)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "0F8C8080", 0x18)]
public struct D2Class_0F8C8080
{
    public TigerHash WwiseEventHash;
    [SchemaField(0x8)]
    public DynamicArray<D2Class_138C8080> Sounds;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "0E068080", 0x24)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "138C8080", 0x28)]
[SchemaStruct(TigerStrategy.DESTINY2_LATEST, "138C8080", 0x40)]
public struct D2Class_138C8080
{
    public short Unk00;
    public short Unk02;
    [SchemaField(0x8)]
    public TigerHash Unk08;
    [SchemaField(0x10)]
    public StringPointer WwiseEventName;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public FileHash Data; // Can be WwiseSound or pattern entity
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "19278080", 0x530)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "97318080", 0x540)]
public struct D2Class_97318080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "98258080", 0x90)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "F62C8080", 0xB0)]
public struct D2Class_F62C8080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "8D248080", 0x208)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "F42C8080", 0x338)]
[SchemaStruct(TigerStrategy.DESTINY2_LATEST, "F42C8080", 0x358)]
public struct D2Class_F42C8080
{
    [SchemaField(0x188, TigerStrategy.DESTINY2_LATEST)]
    public ResourcePointer Unk188;

    [SchemaField(0x1D8, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x2C8, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    [SchemaField(0x318, TigerStrategy.DESTINY2_LATEST)]
    public DynamicArray<D2Class_FA2C8080> PatternAudioGroups;

    [SchemaField(0xD0, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_WITCHQUEEN_6307, Obsolete = true)]
    [SchemaField(0xD0, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Entity? FallbackAudio1;

    [SchemaField(0xF0, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_WITCHQUEEN_6307, Obsolete = true)]
    [SchemaField(0x100, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Entity? FallbackAudio2;

    [SchemaField(0x118, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Entity? FallbackAudio3;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "12238080", 0x140)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "FA2C8080", 0x258)]
[SchemaStruct(TigerStrategy.DESTINY2_LIGHTFALL_7366, "EEBC8080", 0x278)]
[SchemaStruct(TigerStrategy.DESTINY2_LATEST, "73B78080", 0x2A8)] // Why does this keep changing???
public struct D2Class_FA2C8080
{
    [SchemaField(0x18, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x10, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public TigerHash WeaponContentGroupHash; // "weaponContentGroupHash" from API
    public TigerHash Unk14;
    public TigerHash Unk18;

    [SchemaField(0x30, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    public TigerHash WeaponTypeHash1; // "weaponTypeHash" from API

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)] // These aren't obsolete but not needed, I don't think
    [SchemaField(0x60, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public Tag Unk60;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x78, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public Tag Unk78;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x90, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public Tag Unk90;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0xA8, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public Tag UnkA8;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0xC0, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public Tag UnkC0;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0xD8, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    public Tag UnkD8;

    [SchemaField(0x78, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0xF0, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    [SchemaField(0x120, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Tag<D2Class_A36F8080> AudioEntityParent;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x120, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    [SchemaField(0x150, TigerStrategy.DESTINY2_LATEST)] // ??
    public TigerHash WeaponTypeHash2; // "weaponTypeHash" from API

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x130, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    [SchemaField(0x160, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Tag Unk130;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x148, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    [SchemaField(0x178, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Tag Unk148;

    [SchemaField(0x80, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x118, TigerStrategy.DESTINY2_LIGHTFALL_7366)]
    [SchemaField(0x148, TigerStrategy.DESTINY2_LATEST)]
    public ResourcePointer Unk118;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x1C0, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    [SchemaField(0x1D0, TigerStrategy.DESTINY2_LIGHTFALL_7366, Tag64 = true)]
    [SchemaField(0x200, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Tag Unk1C0;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x1D8, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    [SchemaField(0x1E8, TigerStrategy.DESTINY2_LIGHTFALL_7366, Tag64 = true)]
    [SchemaField(0x218, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Tag Unk1D8;

    // public DynamicArray<D2Class_87978080> Unk1E8;
    // public DynamicArray<D2Class_84978080> Unk1F8;
    // public DynamicArray<D2Class_062D8080> Unk208;

    [SchemaField(TigerStrategy.DESTINY1_RISE_OF_IRON, Obsolete = true)]
    [SchemaField(0x248, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    [SchemaField(0x268, TigerStrategy.DESTINY2_LIGHTFALL_7366, Tag64 = true)]
    [SchemaField(0x298, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public Tag Unk248;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "65318080", 0x50)]
public struct S65318080
{
    public long FileSize;
    public StringPointer TagPath1;
    public Entity? Entity1;
    [SchemaField(0x18)]
    public StringPointer TagPath2;
    public Entity? Entity2;
    [SchemaField(0x28)]
    public StringPointer TagPath3;
    public Entity? Entity3;
    [SchemaField(0x38)]
    public StringPointer TagPath4;
    public Entity? Entity4;

}

[SchemaStruct("092D8080", 0xA0)]
public struct D2Class_092D8080
{
    public long FileSize;
    public TigerHash Unk08;
    [SchemaField(0x18, Tag64 = true)]
    public Entity? Unk18;
    [SchemaField(0x30, Tag64 = true)]
    public Entity? Unk30;
    [SchemaField(0x48, Tag64 = true)]
    public Entity? Unk48;
    [SchemaField(0x60, Tag64 = true)]
    public Entity? Unk60;
    [SchemaField(0x78, Tag64 = true)]
    public Entity? Unk78;
    [SchemaField(0x90, Tag64 = true)]
    public Entity? Unk90;
}


// Turns out this can be used for more than just sounds, recent findings have seen it used for map global channels?
[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "10068080", 0x270)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "E9848080", 0x2E8)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "79818080", 0x390)]
public struct D2Class_79818080
{
    [SchemaField(0x110, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x158, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x1A8, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x1C8, TigerStrategy.DESTINY2_LATEST)] // 100% gonna get changed with the next expansion, calling it now
    public DynamicArray<D2Class_F1918080> Array1;

    [SchemaField(0x120, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x168, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x1B8, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x1D8, TigerStrategy.DESTINY2_LATEST)]
    public DynamicArray<D2Class_F1918080> Array2;

    [SchemaField(0x130, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)] // Not obsolete, just not used for anything atm
    public DynamicArray<D2Class_F1918080> Array3;
}


[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "29068080", 0x8)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "E6938080", 0x18)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "F1918080", 0x18)]
public struct D2Class_F1918080
{
    [SchemaField(0, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x10, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    public ResourcePointer Unk10; // B9678080, 40668080
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "001F8080", 0x54)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "40668080", 0x68)]
[SchemaStruct(TigerStrategy.DESTINY2_LATEST, "40668080", 0x68)]
public struct D2Class_40668080
{
    [SchemaField(0x20, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x28, TigerStrategy.DESTINY2_WITCHQUEEN_6307, Tag64 = true)]
    [SchemaField(0x50, TigerStrategy.DESTINY2_LATEST, Tag64 = true)]
    public WwiseSound Sound;
}

[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "B9678080", 0x110)]
public struct D2Class_B9678080
{
    [SchemaField(0x28)]
    public DynamicArray<D2Class_BB678080> Unk28;
}

[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "BB678080", 0x18)]
public struct D2Class_BB678080
{
    [SchemaField(0x10)]
    public Tag<D2Class_20698080> FXContainer;
}

[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "20698080", 0x40)]
public struct D2Class_20698080
{
    public FileHash Unk00;
    [SchemaField(0x18)] // idfk why not having the above FileHash makes this read at 0x0??
    public Material UnkMat;
    [SchemaField(0x20, Tag64 = true)]
    public Tag<D2Class_29698080> ModelContainer;
}

[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "29698080", 0x18)]
public struct D2Class_29698080
{
    [SchemaField(0x10)]
    public DynamicArray<D2Class_066F8080> Models;
}

[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "066F8080", 4)]
public struct D2Class_066F8080
{
    public EntityModel Model;
}

[SchemaStruct("72818080", 0x18)]
public struct D2Class_72818080
{
}

[SchemaStruct("00488080", 0x20)]
public struct D2Class_00488080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "9A078080", 0x250)]
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "D7848080", 0x250)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "79948080", 0x300)]
public struct D2Class_79948080
{
}

[SchemaStruct("E3918080", 0x40)]
public struct D2Class_E3918080
{
}

[SchemaStruct("0A2D8080", 0x4C)]
public struct D2Class_0A2D8080
{
    [SchemaField(0x8, Tag64 = true)]
    public Entity? Unk08;
    [SchemaField(0x20, Tag64 = true)]
    public Entity? Unk20;
    [SchemaField(0x38, Tag64 = true)]
    public Entity? Unk38;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "40238080", 0x34)]
public struct S40238080
{
    [SchemaField(0x8, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public Entity? Unk08;
    [SchemaField(0x18, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public Entity? Unk20;
    [SchemaField(0x28, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(TigerStrategy.DESTINY2_SHADOWKEEP_2601, Obsolete = true)]
    public Entity? Unk38;
}

#endregion

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "7F6B8080", 0x1C0)] // Map cubemaps dont exist in D1 but this needs to exist
[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "7F6B8080", 0x1C0)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "95668080", 0x1E0)]
[SchemaStruct(TigerStrategy.DESTINY2_LATEST, "95668080", 0x1DC)]
public struct SMapCubemapResource //Dataresource for cubemaps
{
    [SchemaField(0x20)]
    public Vector4 CubemapSize; //XYZ, no W
    public Vector4 CubemapPosition; // Not actually right afaik

    [SchemaField(0xB0)]
    public long WorldID; // Same as the ID in the datatable entry

    [SchemaField(0x140, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x140, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x100, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public Vector4 CubemapRotation;

    [SchemaField(0x190, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x190, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x1B0, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x1B0, TigerStrategy.DESTINY2_LATEST, Obsolete = true)] // :(
    public StringPointer CubemapName;

    [SchemaField(0x198, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x198, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x1B8, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x1AC, TigerStrategy.DESTINY2_LATEST)]
    public Texture CubemapTexture;

    [SchemaField(0x1A0, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x1A0, TigerStrategy.DESTINY2_SHADOWKEEP_2601)]
    [SchemaField(0x1C0, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x1B4, TigerStrategy.DESTINY2_LATEST)]
    public Texture CubemapIBLTexture; //Sometype of reflection tint texture idk
}

[SchemaStruct(TigerStrategy.DESTINY2_SHADOWKEEP_2601, "D99B8080", 0x190)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "D8928080", 0x190)]
public struct D2Class_D8928080
{
    //[DestinyOffset(0x10)]
    //public DestinyHash Unk10;
    //public DestinyHash Unk14;
    //public DestinyHash Unk18;
    //public DestinyHash Unk1C;
    //public DestinyHash Unk20;
    //public DestinyHash Unk24;
    //[SchemaField(0x68)]
    //public DynamicArray<D2Class_56998080> Unk68;
    [SchemaField(0x84)]
    public Tag<SMapDataTable> Unk84;
    [SchemaField(0x90)]
    public Vector4 Rotation;
    public Vector4 Translation;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "EF8C8080", 0x60)]
public struct D2Class_EF8C8080
{
    [SchemaField(0x58)]
    public Tag<SMapDataTable> Unk58;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "FA988080", 0x80)]
public struct D2Class_FA988080
{
    [SchemaField(0x28)]
    public TigerHash FNVHash;
    [SchemaField(0x30)]
    public ulong WorldID;
    [SchemaField(0x58)]
    public DynamicArray<D2Class_05998080> Unk58;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "F88C8080", 0x80)]
public struct D2Class_F88C8080
{
    [SchemaField(0x28)]
    public TigerHash FNVHash;
    [SchemaField(0x30)]
    public ulong WorldID;
    [SchemaField(0x58)]
    public DynamicArray<D2Class_05998080> Unk58;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "EF988080", 0x80)]
public struct D2Class_EF988080
{
    [SchemaField(0x28)]
    public TigerHash FNVHash;
    [SchemaField(0x30)]
    public ulong WorldID;
    [SchemaField(0x58)]
    public DynamicArray<D2Class_05998080> Unk58;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "6F418080", 0xE0)]
public struct D2Class_6F418080
{
    [SchemaField(0x28)]
    public TigerHash FNVHash;
    [SchemaField(0x30)]
    public ulong WorldID;
    [SchemaField(0x58)]
    public DynamicArray<D2Class_05998080> Unk58;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "26988080", 0x98)]
public struct D2Class_26988080
{
    [SchemaField(0x28)]
    public TigerHash FNVHash;
    [SchemaField(0x30)]
    public ulong WorldID;
    [SchemaField(0x58)]
    public DynamicArray<D2Class_05998080> Unk58;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "95468080", 0x90)]
public struct D2Class_95468080
{
    [SchemaField(0x28)]
    public TigerHash FNVHash;
    [SchemaField(0x30)]
    public ulong WorldID;
    [SchemaField(0x58)]
    public DynamicArray<D2Class_05998080> Unk58;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "05998080", 0x10)]
public struct D2Class_05998080
{
    public TigerHash FNVHash;
    [SchemaField(0x8)]
    public ulong WorldID;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "6B908080", 0x28)]
public struct D2Class_6B908080
{
    [SchemaField(0x8)]
    public DynamicArray<D2Class_029D8080> Unk08;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "029D8080", 0x10)]
public struct D2Class_029D8080
{
    public ResourceInTablePointer<D2Class_4D898080> Unk00;
    public RelativePointer Unk08;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "4D898080", 0xC)]
public struct D2Class_4D898080
{
    public StringPointer Name;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "E3138080", 0x1190)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "357C8080", 0x1BD0)]
public struct D2Class_357C8080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "08138080", 0x2C0)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "18808080", 0x448)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "18808080", 0x478)]
public struct D2Class_18808080
{
    [SchemaField(0x278, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x398, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    [SchemaField(0x3C0, TigerStrategy.DESTINY2_WITCHQUEEN_6307)]
    [SchemaField(0x398, TigerStrategy.DESTINY2_LATEST)] // Got moved on Heresy Act 2 (3/11/25) update
    public Tag<D2Class_4D7E8080> Unk3C0;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "F3138080", 0x90)]
[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "4D7E8080", 0x30)]
public struct D2Class_4D7E8080
{
    [SchemaField(0xC, TigerStrategy.DESTINY1_RISE_OF_IRON)]
    [SchemaField(0x2C, TigerStrategy.DESTINY2_BEYONDLIGHT_3402)]
    public StringHash EntityName;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "0E478080", 0x110)]
public struct D2Class_0E478080
{
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "B5468080", 0x150)]
public struct D2Class_B5468080
{
    [SchemaField(0x80)]
    public DynamicArray<D2Class_96468080> Unk80;

    [SchemaField(0xC0)]
    public Vector4 Rotation;
    public Vector4 Translation;
}

[SchemaStruct(TigerStrategy.DESTINY2_BEYONDLIGHT_3402, "96468080", 0x78)]
[SchemaStruct(TigerStrategy.DESTINY2_WITCHQUEEN_6307, "96468080", 0x80)]
public struct D2Class_96468080
{
    [SchemaField(0x28, Tag64 = true)]
    public Tag<SMapDataTable> DataTable;
    public StringHash Name;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "B9268080", 0x1D8)]
public struct SB9268080
{
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "DA288080", 0x98)]
public struct SDA288080
{
    [SchemaField(0x68)]
    public Entity? Unk68;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "712B8080", 0xA0)]
public struct S712B8080
{
    [SchemaField(0x20)]
    public FileHash Entity;

    [SchemaField(0x88)]
    public DynamicArray<S93278080> Transforms;
}

[SchemaStruct(TigerStrategy.DESTINY1_RISE_OF_IRON, "93278080", 0x40)]
public struct S93278080
{
    [SchemaField(0x10)]
    public Vector4 Rotation;
    public Vector4 Translation;
}
