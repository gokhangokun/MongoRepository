namespace GYG.MongoRepository.Mapping
{
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Conventions;
    using Entity;
    public static class MongoClassMapHelper
    {

        private static readonly object Lock = new object();

        public static void RegisterConventionPacks()
        {
            lock (Lock)
            {
                var conventionPack = new ConventionPack { new IgnoreIfNullConvention(true) };
                ConventionRegistry.Register("ConventionPack", conventionPack, t => true);
            }
        }

        public static void SetupClassMap()
        {
            lock (Lock)
            {

                if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
                {
                    BsonClassMap.RegisterClassMap<Entity>(
                        (classMap) =>
                        {
                            classMap.AutoMap();
                            classMap.MapIdProperty(p => p.Id);
                            classMap.MapExtraElementsProperty(p => p.Metadata);
                        });
                }
            }
        }
    }
}
