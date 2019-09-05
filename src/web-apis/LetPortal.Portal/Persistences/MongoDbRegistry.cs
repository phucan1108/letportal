using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.SectionParts;
using MongoDB.Bson.Serialization;

namespace LetPortal.Portal.Persistences
{
    public static class MongoDbRegistry
    {
        public static void RegisterEntities()
        {
            BsonClassMap.RegisterClassMap<Page>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<Component>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<DynamicList>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<StandardComponent>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
