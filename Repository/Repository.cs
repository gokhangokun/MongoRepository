namespace GYG.MongoRepository.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Entity;
    using Exceptions;
    using Mapping;
    using MongoDB.Bson;
    using MongoDB.Driver;
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoDatabase _database;
        
        private IMongoCollection<T> Collection { get; }
        
        static Repository()
        {
            MongoClassMapHelper.RegisterConventionPacks();
            MongoClassMapHelper.SetupClassMap();
        }
        
        public Repository(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentException("Missing MongoDB connection string");
            }

            var client = new MongoClient(connectionString);
            var mongoUrl = MongoUrl.Create(connectionString);
            _database = client.GetDatabase(mongoUrl.DatabaseName);
            Collection = SetupCollection();
        }
        
        private IMongoCollection<T> SetupCollection()
        {
            try
            {
                var collectionName = BuildCollectionName();
                var collection = _database.GetCollection<T>(collectionName);
                return collection;
            }
            catch (MongoException ex)
            {
                throw new CoreException(ex.Message);
            }
        }
        
        private static string BuildCollectionName()
        {
            var className = typeof(T).Name;
            var pluralizedName = className.EndsWith("s") ? className : className + "s";
            pluralizedName = typeof(T).Name.EndsWith("y") ? className.Remove(className.Length - 1) + "ies" : pluralizedName;
            return pluralizedName;
        }
        public async Task<T> Insert(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                entity.CreateDate = DateTime.UtcNow;
                entity.UpdateDate = DateTime.UtcNow;
                await Collection.InsertOneAsync(entity, null, cancellationToken);
            }
            catch (MongoWriteException ex)
            {
                throw new EntityException(entity, "Insert failed because the entity already exists!", ex);
            }

            return entity;
        }
        
        public async Task<T> Update(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            entity.CreateDate = DateTime.UtcNow;
            entity.Version++;

            var idFilter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            var result = await Collection.ReplaceOneAsync(idFilter, entity, null, cancellationToken);

            if (result != null && ((result.IsAcknowledged && result.MatchedCount == 0) || (result.IsModifiedCountAvailable && !(result.ModifiedCount > 0))))
                throw new EntityException(entity, "Entity does not exist.");

            return entity;
        }
        
        public async Task<T> Get(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Collection.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
        }
        
        public async Task<T> Delete(ObjectId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Collection.FindOneAndDeleteAsync(e => e.Id == id, null, cancellationToken);
        }
        
        public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Collection.Find(e => true).ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<T>> Pagination(int top, int skip, Func<T, object> orderBy, bool ascending = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = Collection.Find(e => true).Skip(skip).Limit(top);

            if (ascending)
                return await query.SortBy(e => e.Id).ToListAsync(cancellationToken);

            return await query.SortByDescending(e => e.Id).ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<T>> SearchFor(FilterDefinition<T> filter)
        {
            return await Collection.Find(filter).ToListAsync();
        }
    }
}
