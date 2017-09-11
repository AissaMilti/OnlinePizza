using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlinePizza.Data;
using OnlinePizza.Models;

namespace OnlinePizza.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void RegisterUser(ApplicationUser applicationUser)
        {
            var user = new ApplicationUser
            {
                Name = applicationUser.Name,
                Address = applicationUser.Address,
                PostalCode = applicationUser.PostalCode,
                City = applicationUser.City,
                UserEmail = applicationUser.UserEmail,
                Password = applicationUser.Password,
                Phone = applicationUser.Phone
            };

            _context.ApplicationUsers.Add(user);
            _context.SaveChanges();
        }

    }
}
