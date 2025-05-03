﻿using Tiger.Exporters;

namespace Tiger.Schema.Entity;

public class Entity : Tag<SEntity>
{
    public TfxFeatureRenderer FeatureType = TfxFeatureRenderer.DynamicObjects;
    // Entity features, todo clean this up
    public EntitySkeleton? Skeleton { get; set; }
    public EntityModel? Model { get; private set; }
    public EntityResource? ModelParentResource { get; private set; }
    public EntityModel? PhysicsModel { get; private set; }
    public EntityPhysicsModelParent? PhysicsModelParentResource { get; private set; }
    public EntityResource? PatternAudio { get; private set; }
    public EntityResource? PatternAudioUnnamed { get; private set; }
    public EntityControlRig? ControlRig { get; private set; }
    public EntityResource? EntityChildren { get; private set; }
    public string? EntityName { get; set; } // Usually just the generic name (Ogre, Vandal, etc)
    public DestinyGenderDefinition Gender { get; set; } = DestinyGenderDefinition.None; // Only used for player armor

    public List<EntityResource>? EntityChildren2 { get; private set; } // Some weird collection of entities thats only in Pre-BL?

    private bool _loaded = false;

    public Entity(FileHash hash) : base(hash)
    {
        Load();
    }

    public Entity(FileHash hash, bool shouldParse = true) : base(hash, shouldParse)
    {
        if (shouldParse)
        {
            Load();
        }
    }


    // TODO: Figure out a way to make Dynamics view not take 10gb of ram and almost a minute to load.
    // Tried setting most things to NoLoad but it made little difference. I'm not entirely sure what's being the
    // resource hog. Thought it was EntityModel but I guess not.
    public void Load()
    {
        Deserialize();
        _loaded = true;
        //Debug.Assert(_tag.FileSize != 0); // Is this really needed?
        foreach (FileHash? resourceHash in _tag.EntityResources.Select(GetReader(), r => r.Resource))
        {
            if (Strategy.IsD1() && resourceHash.GetReferenceHash() != 0x80800861)
                continue;
            EntityResource resource = FileResourcer.Get().GetFile<EntityResource>(resourceHash);
            switch (resource.TagData.Unk10.GetValue(resource.GetReader()))
            {
                case S8A6D8080:  // Entity model
                    Model = ((S8F6D8080)resource.TagData.Unk18.GetValue(resource.GetReader())).Model;
                    ModelParentResource = resource;
                    break;

                case S5B6D8080:  // Entity physics model
                    PhysicsModel = ((S6C6D8080)resource.TagData.Unk18.GetValue(resource.GetReader())).PhysicsModel;
                    PhysicsModelParentResource = FileResourcer.Get().GetFile<EntityPhysicsModelParent>(resource.Hash);
                    break;

                case SD5818080:
                case SDD818080:  // Entity skeleton FK
                    Skeleton = FileResourcer.Get().GetFile<EntitySkeleton>(resource.Hash);
                    break;

                //case S668B8080:  // Entity skeleton IK  todo shadowkeep
                //    ControlRig = FileResourcer.Get().GetFile<EntityControlRig>(resource.Hash);
                //    break;

                case S97318080: // todo shadowkeep
                    PatternAudio = resource;
                    break;

                case SF62C8080: // todo shadowkeep
                    PatternAudioUnnamed = resource;
                    break;

                case S357C8080: // Generic name
                    // we care more about the specific name so if the entity name is already assigned, dont assign this one
                    if (EntityName == null)
                    {
                        StringHash genericName = ((S18808080)resource.TagData.Unk18.GetValue(resource.GetReader())).Unk3C0.TagData.EntityName;
                        if (GlobalStrings.Get().GetString(genericName) != genericName)
                            EntityName = GlobalStrings.Get().GetString(genericName);
                    }
                    break;

                case SDA5E8080: // Specific name
                    StringHash specificName = Strategy.CurrentStrategy != TigerStrategy.DESTINY1_RISE_OF_IRON ?
                        ((SDB5E8080)resource.TagData.Unk18.GetValue(resource.GetReader())).Unk108.TagData.EntityName :
                        ((SDB5E8080)resource.TagData.Unk18.GetValue(resource.GetReader())).EntityName;

                    // Don't assign a name if the name hash doesnt return an actual string (returns the name hash instead)
                    if (GlobalStrings.Get().GetString(specificName) != specificName)
                        EntityName = GlobalStrings.Get().GetString(specificName);
                    break;

                case S79948080 when Strategy.IsPreBL(): // No idea if this is SK only
                    if (EntityChildren2 is null)
                        EntityChildren2 = new();

                    EntityChildren2.Add(resource);
                    break;

                case S12848080:
                    EntityChildren = resource;
                    break;

                default:
                    //Console.WriteLine($"{resource.TagData.Unk18.GetValue(resource.GetReader())}");
                    // throw new NotImplementedException($"Implement parsing for {resource.Resource._tag.Unk08}");
                    break;
            }
        }
    }

    public List<DynamicMeshPart> Load(ExportDetailLevel detailLevel)
    {
        if (!_loaded)
        {
            Load();
        }
        var dynamicParts = new List<DynamicMeshPart>();
        if (Model != null)
        {
            dynamicParts = dynamicParts.Concat(Model.Load(detailLevel, ModelParentResource, hasSkeleton: Skeleton != null)).ToList();
        }
        if (PhysicsModel != null)
        {
            dynamicParts = dynamicParts.Concat(PhysicsModel.Load(detailLevel, PhysicsModelParentResource, hasSkeleton: Skeleton != null)).ToList();
        }
        return dynamicParts;
    }

    public void SaveMaterialsFromParts(ExporterScene scene, List<DynamicMeshPart> dynamicParts)
    {
        foreach (DynamicMeshPart dynamicPart in dynamicParts)
        {
            if (dynamicPart.Material == null) continue;
            scene.Materials.Add(new ExportMaterial(dynamicPart.Material));
        }
    }

    public void SaveTexturePlates(string saveDirectory)
    {
        if (ModelParentResource is null)
            return;

        Directory.CreateDirectory($"{saveDirectory}/Textures/");
        var parentResource = (S8F6D8080)ModelParentResource.TagData.Unk18.GetValue(ModelParentResource.GetReader());

        if (Strategy.CurrentStrategy >= TigerStrategy.DESTINY2_SHADOWKEEP_2601 && parentResource.TexturePlates is null)
            return;

        if (Strategy.IsD1() && (parentResource.TexturePlatesROI.Count == 0 || parentResource.TexturePlatesROI[0].TexturePlates is null))
            return;

        S1C6E8080 rsrc = Strategy.CurrentStrategy != TigerStrategy.DESTINY1_RISE_OF_IRON
            ? parentResource.TexturePlates.TagData : parentResource.TexturePlatesROI[0].TexturePlates.TagData;


        rsrc.AlbedoPlate?.SavePlatedTexture($"{saveDirectory}/Textures/{Hash}_albedo");
        rsrc.NormalPlate?.SavePlatedTexture($"{saveDirectory}/Textures/{Hash}_normal");
        rsrc.GStackPlate?.SavePlatedTexture($"{saveDirectory}/Textures/{Hash}_gstack");
        rsrc.DyemapPlate?.SavePlatedTexture($"{saveDirectory}/Textures/{Hash}_dyemap");
    }

    private readonly object _lock = new();
    public bool HasGeometry()
    {
        lock (_lock)
        {
            if (!_loaded)
            {
                Load();
            }
        }
        return Model != null;

        //// saves about 10 seconds when loading dynamics list, but its not really worth it since
        //// the entity will need to be fully loaded after anyways, which adds time
        //foreach (var resourceHash in _tag.EntityResources.Select(GetReader(), r => r.Resource))
        //{
        //    if (resourceHash.ContainsHash(0x80806D8A)) // 8A6D8080
        //        return true;
        //}
        //return false;
    }

    public List<Entity> GetEntityChildren()
    {
        lock (_lock)
        {
            if (!_loaded)
            {
                Load();
            }
        }

        List<Entity> entities = new();
        if (Strategy.IsPreBL() && EntityChildren2 is not null)
            entities.AddRange(GetEntityChildren2());

        if (EntityChildren is null)
            return entities;

        if (Strategy.IsD1())
        {
            foreach (S712B8080 entry in ((S0E848080)EntityChildren.TagData.Unk18.GetValue(EntityChildren.GetReader())).Unk100)
            {
                if (entry.Entity is null)
                    continue;
                Entity entity = FileResourcer.Get().GetFile<Entity>(entry.Entity);
                if (entity.HasGeometry())
                {
                    //entity.ModelParent = ModelParent;
                    //var parent = entity.ModelParent.TagData.Meshes.Enumerate(entity.ModelParent.GetReader()).FirstOrDefault().ModelTranslation;
                    //var offset = entry.Transforms.FirstOrDefault().Translation;
                    //Console.WriteLine($"Entity {entity.Hash}");
                    //Console.WriteLine($"ModelParent {parent}");
                    //Console.WriteLine($"TranslationOffset {offset}");

                    //entity.Model.TranslationOffset = parent + new Vector4(offset.Z, offset.X, offset.Y);
                    //entity.Model.RotationOffset = entry.Transforms.FirstOrDefault().Rotation;
                    entities.Add(entity);
                    //Just in case
                    foreach (Entity child in entity.GetEntityChildren())
                        entities.Add(child);
                }
            }
        }
        else
        {
            if (EntityChildren.TagData.Unk18.GetValue(EntityChildren.GetReader()) is S0E848080)
            {
                foreach (S1B848080 entry in ((S0E848080)EntityChildren.TagData.Unk18.GetValue(EntityChildren.GetReader())).Unk88)
                {
                    foreach (S1D848080 entry2 in entry.Unk08)
                    {
                        if (entry2.Entity is null)
                            continue;

                        Entity entity = FileResourcer.Get().GetFile<Entity>(entry2.Entity.Hash);
                        if (entity.HasGeometry())
                        {
                            entities.Add(entity);
                            //Just in case
                            foreach (Entity child in entity.GetEntityChildren())
                                entities.Add(child);
                        }
                    }
                }
            }
        }

        return entities;
    }

    public List<Entity> GetEntityChildren2() // THIS SUCKS WHYYY BUNGIEEE
    {
        List<Entity> entities = new();
        if (EntityChildren2 is null)
            return entities;

        foreach (EntityResource resource in EntityChildren2)
        {
            if (resource.TagData.Unk18.GetValue(resource.GetReader()) is S79818080)
            {
                foreach (SF1918080 entry in ((S79818080)resource.TagData.Unk18.GetValue(resource.GetReader())).Array2)
                {
                    if (entry.Unk10.GetValue(resource.GetReader()) is S81888080 entry2)
                    {
                        if (entry2.Entity is null)
                            continue;
#if DEBUG
                        Console.WriteLine($"GetEntityChildren2: {resource.Hash} : {entry2.Entity.Hash}");
#endif
                        Entity entity = FileResourcer.Get().GetFile<Entity>(entry2.Entity.Hash);
                        if (!entities.Contains(entity) && entity.HasGeometry())
                        {
                            entities.Add(entity);
                            //Just in case
                            foreach (Entity child in entity.GetEntityChildren())
                                entities.Add(child);
                        }
                    }
                }
            }
        }
        return entities;
    }
}
