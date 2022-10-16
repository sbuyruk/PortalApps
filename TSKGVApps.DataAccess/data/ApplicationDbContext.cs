using Microsoft.EntityFrameworkCore;
using Model.Ortak;

namespace TSKGVApps.DataAccess.data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<YetkiTanim> YetkiTanim_Table { get; set; }
    }
}
