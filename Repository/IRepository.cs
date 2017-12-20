namespace GYG.MongoRepository.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Entity;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public interface IRepository<T> where T : IEntity
    {
        Task<T> Insert(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task<T> Update(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task<T> Get(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));

        Task<T> Delete(ObjectId id, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<T>> Pagination(int top, int skip, Func<T, object> orderBy, bool ascending = true, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<T>> SearchFor(FilterDefinition<T> filter);

        Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default(CancellationToken));
    }
}
