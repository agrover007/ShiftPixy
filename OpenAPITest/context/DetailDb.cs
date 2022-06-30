using Microsoft.EntityFrameworkCore;

namespace OpenAPITest
{
    public class DetailDb : DbContext
    {
        public DetailDb (DbContextOptions options) : base(options) { }
        public DbSet<Detail> Details { get; set; }
    }
}
