namespace GYG.MongoRepository.Exceptions
{
    using System;
    public class EntityException : CoreException
    {
        public object Entity { get; }

        public override int InternalExceptionCode => EntityExpectionCode;

        public EntityException(object entity, string message) : base(message)
        {
            Entity = entity;
        }

        public EntityException(object entity, string message, Exception inner) : base(message, inner)
        {
            Entity = entity;
        }
    }
}
