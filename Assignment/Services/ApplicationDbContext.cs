using Assignment.Framework;
using Assignment.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Assignment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSets for entities
        public DbSet<TransactionReportModel> MoneyTransfers { get; set; }
        public DbSet<RegisterModel> Registers { get; set; }
        public DbSet<LoginModel> Login { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LoginModel>()
           .ToTable("SignUpTbl");
        }
    }
}
