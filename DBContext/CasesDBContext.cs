using Microsoft.EntityFrameworkCore;
using panasonic_machine_checker.DBContext;

namespace panasonic_machine_checker.DBContext
{
    public class CasesDBContext: DbContext
    {
        public CasesDBContext(DbContextOptions options): base(options) { 
        
        }

        public DbSet<Cases> Cases { get; set; }
    }
}
