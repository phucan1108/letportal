using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace LetPortal.Core.Persistences
{
    public sealed class ConventionPackDefault
    {
        public static void Register()
        {
            var pack = new ConventionPack();
            pack.Add(new CamelCaseElementNameConvention());
            pack.Add(new IgnoreExtraElementsConvention(true));
            ConventionRegistry.Register(
               "Camel Element",
               pack,
               t => t.FullName.StartsWith("LetPortal."));


            BsonClassMap.RegisterClassMap<Entity>(cm =>
            {
                cm.AutoMap();
                cm.MapIdField(a => a.Id);
                cm.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
            });

            BsonClassMap.RegisterClassMap<BackupableEntity>(cm =>
            {
                cm.AutoMap();
            });
        }
    }
}
