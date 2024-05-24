using NpgsqlTypes;

namespace PostgresSqlFullTextSearch.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public NpgsqlTsVector SearchVector { get; set; }
    }
}