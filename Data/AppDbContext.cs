using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using UserManagementAPI.Models;

namespace UserManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> user { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }       
    }
}
