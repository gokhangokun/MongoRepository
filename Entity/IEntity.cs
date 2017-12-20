namespace GYG.MongoRepository.Entity
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson;
    public interface IEntity
    {
        ObjectId Id { get; set; }
        
        DateTime CreateDate { get; set; }
        
        DateTime UpdateDate { get; set; }
        
        IDictionary<string, object> Metadata { get; set; }
        
        long Version { get; set; }
    }
}
