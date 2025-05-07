namespace Tiger.Schema.Entity;

public class EntityPhysicsModelParent : EntityResource
{
    public EntityPhysicsModelParent(FileHash resource) : base(resource)
    {
    }

    public EntityModel GetModel()
    {
        return ((S6C6D8080)TagData.Unk18.GetValue(GetReader())).PhysicsModel;
    }
}

