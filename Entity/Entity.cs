namespace GYG.MongoRepository.Entity
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    public class Entity : IEntity
    {
        public ObjectId Id { get; set; }

        [BsonIgnore]
        public string IdString { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public long Version { get; set; }

        public IDictionary<string, object> Metadata { get; set; }

        public Entity()
        {
            Metadata = new Dictionary<string, object>();
        }
    }
}
