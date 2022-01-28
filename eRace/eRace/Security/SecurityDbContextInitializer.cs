using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#region Additional Namespaces
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Data.Entity;
using eRace.Models;
#endregion

namespace eRace.Security
{
    #region TODO #3 Create the SecurityDbContextInitializer class
    // This class will work with the ApplicationDbContext class to "seed" the database
    // when it generates the database tables if they do not exist.
    public class SecurityDbContextInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {

        protected override void Seed(ApplicationDbContext context)
        {
            #region Phase A - Set up our Security Roles
            // 1. Instantiate a Controller class from ASP.Net Identity to add roles
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            // 2. Grab our list of security roles from the web.config
            var startupRoles = ConfigurationManager.AppSettings["startupRoles"].Split(';');
            // 3. Loop through and create the security roles
            foreach (var role in startupRoles)
                roleManager.Create(new IdentityRole { Name = role });
            #endregion

            #region Phase B - Add a Website Administrator
            // 1. Get the values from the <appSettings>
            string adminUser = ConfigurationManager.AppSettings["adminUserName"];
            string adminRole = ConfigurationManager.AppSettings["adminRole"];
            string adminEmail = ConfigurationManager.AppSettings["adminEmail"];
            string adminPassword = ConfigurationManager.AppSettings["adminPassword"];

            // 2. Instantiate my Controller to manage Users
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            //                \   IdentityConfig.cs    /             \IdentityModels.cs/
            // 3. Add the web admin to the database
            var result = userManager.Create(new ApplicationUser
            {
                UserName = adminUser,
                Email = adminEmail
                //EmployeeId = null
            }, adminPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(adminUser).Id, adminRole);
            #endregion

            #region Phase C - Add a Customer
            // no customer users for this application

            #endregion

            #region Phase D - Add a Employee
            //1.Get the values from the<appSettings>

            // Add Director
            int dirId = int.Parse(ConfigurationManager.AppSettings["dirId"]);
            string dirUser = ConfigurationManager.AppSettings["dirUserName"];
            string dirRole = ConfigurationManager.AppSettings["dirRole"];
            string dirEmail = ConfigurationManager.AppSettings["dirEmail"];
            string dirPassword = ConfigurationManager.AppSettings["dirPassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = dirId,
                UserName = dirUser,
                Email = dirEmail
            }, dirPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(dirUser).Id, dirRole);

            // Add Food Service
            int foodServiceId = int.Parse(ConfigurationManager.AppSettings["foodServiceId"]);
            string foodServiceUser = ConfigurationManager.AppSettings["foodServiceUserName"];
            string foodServiceRole = ConfigurationManager.AppSettings["foodServiceRole"];
            string foodServiceEmail = ConfigurationManager.AppSettings["foodServiceEmail"];
            string foodServicePassword = ConfigurationManager.AppSettings["foodServicePassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = foodServiceId,
                UserName = foodServiceUser,
                Email = foodServiceEmail
            }, foodServicePassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(foodServiceUser).Id, foodServiceRole);

            // Add Office Manager
            int officeManagerId = int.Parse(ConfigurationManager.AppSettings["officeManagerId"]);
            string officeManagerUser = ConfigurationManager.AppSettings["officeManagerUserName"];
            string officeManagerRole = ConfigurationManager.AppSettings["officeManagerRole"];
            string officeManagerEmail = ConfigurationManager.AppSettings["officeManagerEmail"];
            string officeManagerPassword = ConfigurationManager.AppSettings["officeManagerPassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = officeManagerId,
                UserName = officeManagerUser,
                Email = officeManagerEmail
            }, officeManagerPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(officeManagerUser).Id, officeManagerRole);

            // Add Clerk
            int clerkId = int.Parse(ConfigurationManager.AppSettings["clerkId"]);
            string clerkUser = ConfigurationManager.AppSettings["clerkUserName"];
            string clerkRole = ConfigurationManager.AppSettings["clerkRole"];
            string clerkEmail = ConfigurationManager.AppSettings["clerkEmail"];
            string clerkPassword = ConfigurationManager.AppSettings["clerkPassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = clerkId,
                UserName = clerkUser,
                Email = clerkEmail
            }, clerkPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(clerkUser).Id, clerkRole);


            #endregion

            base.Seed(context);
        }
    }
    #endregion
}