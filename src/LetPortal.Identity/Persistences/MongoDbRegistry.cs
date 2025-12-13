using LetPortal.Identity.Entities;
using MongoDB.Bson.Serialization;

namespace LetPortal.Identity.Persistences
{
    public static class MongoDbRegistry
    {
        public static void RegisterEntities()
        {
            BsonClassMap.RegisterClassMap<IssuedToken>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(a => a.User);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<UserSession>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(a => a.User);
                cm.UnmapMember(a => a.User);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<UserActivity>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(a => a.UserSession);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
        }
    }
}
