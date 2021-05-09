namespace CribblyBackend.DataAccess.Common
{
    public class Query
    {
        public string Sql { get; set; }
        public object Params { get; set; }
        public string SplitOn { get; set; }
    }
}
