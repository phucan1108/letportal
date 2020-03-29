using LetPortal.Chat.Entities;
using MongoDB.Bson.Serialization;

namespace LetPortal.Chat.Persistences
{
    public static class MongoDbRegistry
    {
        public static void RegisterEntities()
        {
            BsonClassMap.RegisterClassMap<ChatRoom>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<ChatSession>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(a => a.ChatRoom);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
        }
    }
}
