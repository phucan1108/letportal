using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.SectionParts;
using MongoDB.Bson.Serialization;

namespace LetPortal.Portal.Persistences
{
    public static class MongoDbRegistry
    {
        public static void RegisterEntities()
        {
            BsonClassMap.RegisterClassMap<App>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

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

            BsonClassMap.RegisterClassMap<Version>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Localization>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
