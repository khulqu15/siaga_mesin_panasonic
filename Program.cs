using Microsoft.EntityFrameworkCore;
using panasonic_machine_checker.data;

namespace panasonic_machine_checker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            // builder.Services.AddSingleton<DbPanasonicContext>();

            var key = builder.Configuration["Jwt:Key"];

            builder.Services.AddDbContext<DbPanasonicContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("myconn")));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
