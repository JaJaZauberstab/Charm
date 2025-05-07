namespace Tiger.Schema.Entity;

public class EntityModelParent : EntityResource
{
    public EntityModelParent(FileHash resource) : base(resource)
    {
    }

    public EntityModel GetModel()
    {
        return ((S8F6D8080)TagData.Unk18.GetValue(GetReader())).Model;
    }

    // TODO: Fill this out with model resource related methods? 
}
