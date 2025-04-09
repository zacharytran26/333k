﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


//TODO: Update this using statement to include your project name
using Tran_Zachary_HW5.Models;
using Tran_Zachary_HW5.DAL;
using System;

//TODO: Upddate this namespace to match your project name
namespace Tran_Zachary_HW5.Controllers
{
    public class SeedController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedController(AppDbContext db, UserManager<AppUser> um, RoleManager<IdentityRole> rm)
        {
            _context = db;
            _userManager = um;
            _roleManager = rm;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SeedRoles()
        {
            try
            {
                //call the method to seed the roles
                await Seeding.SeedRoles.AddAllRoles(_roleManager);
            }
            catch (Exception ex)
            {
                //add the error messages to a list of strings
                List<String> errorList = new List<String>();

                //Add the outer message
                errorList.Add(ex.Message);

                //Add the message from the inner exception
                errorList.Add(ex.InnerException.Message);

                //Add additional inner exception messages, if there are any
                if (ex.InnerException.InnerException != null)
                {
                    errorList.Add(ex.InnerException.InnerException.Message);
                }

                return View("Error", errorList);
            }

            //this is the happy path - seeding worked!
            return View("Confirm");
        }
        public async Task<IActionResult> SeedPeople()
        {
            try
            {
                //call the method to seed the users
                await Seeding.SeedUsers.SeedAllUsers(_userManager, _context);
            }
            catch (Exception ex)
            {
                //add the error messages to a list of strings
                List<String> errorList = new List<String>();

                //Add the outer message
                errorList.Add(ex.Message);

                if (ex.InnerException != null)
                {
                    //Add the message from the inner exception
                    errorList.Add(ex.InnerException.Message);

                    //Add additional inner exception messages, if there are any
                    if (ex.InnerException.InnerException != null)
                    {
                        errorList.Add(ex.InnerException.InnerException.Message);
                    }

                }


                return View("Error", errorList);
            }

            //this is the happy path - seeding worked!
            return View("Confirm");
        }
    }
}
