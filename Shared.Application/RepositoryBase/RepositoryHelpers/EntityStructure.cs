namespace  Shared.Application.RepositoryBase.RepositoryHelpers
{
    public class EntityStructure
    {
        public EntityStructure()
        {
            RelatedEntities = new List<RelatedEntity>();
        }
        public string[]? Key { get; init; }
        public List<RelatedEntity> RelatedEntities { get; init; }
    }
    public class RelatedEntity
    {
        public Type? EntityType { get; init; }
        public string[]? Keys { get; init; }
        public string? NavigationProperty { get; init; }
        public List<RelatedEntity>? RelatedEntities { get; init; }

    }
}
