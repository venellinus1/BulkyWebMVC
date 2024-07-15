
using Bulky.DataAccess.Data;
using Bulky.Models.Models;
using Bulky.Utility;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer;

public class DbInitializer
    : IDbInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDBContext _db;
    public DbInitializer(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDBContext db)
        
    {
        _userManager = userManager;
        _roleManager = roleManager;
       _db = db;
    }
    public void Initialize()
    {
        //this method will be invoked from Program.cs each time the app is restarted
        //roles creation code will be run once, 
        //any pending migrations will be run automatically

        // migrations if they are not applied
        try
        {
            if (_db.Database.GetPendingMigrations().Count() > 0)
                _db.Database.Migrate();
        }
        catch (Exception ex) { }
        // create roles if they are not created
        if (!_roleManager.RoleExistsAsync(StaticDetails.Role_Customer).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Customer)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Employee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Company)).GetAwaiter().GetResult();

            //if roles doesnt exist yet -> running the dbinitialize for first time -> create default admin user:
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "venellinus@gmail.com",//or admin@website.com
                Email = "venellinus@gmail.com",//or admin@website.com
                Name = "Venelin Vasilev",//or Admin Name
                PhoneNumber = "1234567890",
                StreetAddress = "Street 123",
                State = "PL",
                PostalCode = "12345",
                City = "Pleven"
            }, "Admin123*")//REPLACE with required Admin Password
            .GetAwaiter()
            .GetResult();

            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "venellinus@gmail.com");
            _userManager.AddToRoleAsync(user, StaticDetails.Role_Admin).GetAwaiter().GetResult();
        }

        return;
    }
}
