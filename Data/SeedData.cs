using IdentityApp.Authorization;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPass="Test@1234")
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is going to be set with a string in the code.
                // Please ensure that the password policy we set in our authentication video is respected here.
                // In a later video, we shall cover secrets and will change this. For now,
                // let's supply it as an optional string parameter

                // Let's create an admin user who can do anything using the password we set
                var adminID = await EnsureUser(serviceProvider, testUserPass, "admin@test.com");
                // we also need to attachthis admin user to the admin role. Let's do that
                await EnsureRole(serviceProvider, adminID, Constants.InvoiceAdministratorsRole);

                // let's also create a manager account with the same password
                var managerID = await EnsureUser(serviceProvider, testUserPass, "manager@test.com");
                // and then attach the manager user to the manager role.
                await EnsureRole(serviceProvider, managerID, Constants.InvoiceManagersRole);

            }
        }

        // method to ensure a user is created
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            // get the identity user service from the registered services
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            // make sure the user you are trying to create does not already exist
            var user = await userManager.FindByNameAsync(UserName);
            // if the user with that username does not exist, then let's create a new identity user.
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    Email = UserName,
                    EmailConfirmed = true
                };
                // create user with password
                var result = await userManager.CreateAsync(user, testUserPw);
            }
            // if the user was not created, we throw an exception. 
            // most probably from the password policy violation
            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }
            // user has been created successfully, let's return the user id.
            return user.Id;
        }

        // method to ensure an authorization role is created
        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            // get the identity role service from the registered services
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            // verify that you have a role manager in the registered services.
            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }
            // define a variale of type Identity result.
            // this is returned when an Identity method is executed.
            IdentityResult IR;
            // if the role we are trying to create does not already exist
            if (!await roleManager.RoleExistsAsync(role))
            {
                // we go ahead and create the role using the role name we set
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }
            // get the user service from the registered services
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            // Find the user we are trying to attach to the role by their id
            var user = await userManager.FindByIdAsync(uid);
            // if the user does not exist, the user was not created.
            // most probably due to a password policy violation
            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            // we add the user to the role and return the result of this method.
            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
    }
}
