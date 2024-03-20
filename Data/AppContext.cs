using Microsoft.EntityFrameworkCore;

namespace panasonic_machine_checker.Data
{
    public class AppContext: DbContext
    {
        public AppContext(DbContextOptions<AppContext> options): base(options) { }
    }
}
