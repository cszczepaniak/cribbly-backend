namespace CribblyBackend.DataAccess.Repositories
{
    public class RepositoryBase
    {
        protected readonly IConnectionFactory _connectionFactory;

        public RepositoryBase(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
    }
}