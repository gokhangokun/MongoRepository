namespace GYG.MongoRepository.Exceptions
{
    public class RepositoryException : CoreException
    {
        public override int InternalExceptionCode => RepositoryExpectionCode;

        public RepositoryException(string message) : base(message)
        {

        }
    }
}
