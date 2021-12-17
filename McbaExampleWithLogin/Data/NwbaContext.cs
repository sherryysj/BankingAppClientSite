using Microsoft.EntityFrameworkCore;
using Mcba.Models;

namespace Mcba.Data
{
    public class McbaContext : DbContext //Migration commends: Add-Migration xxx -> Update-Database
    {
        public McbaContext(DbContextOptions<McbaContext> options) : base(options)
        { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<BillPay> BillPays { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Transaction>().
                HasOne(x => x.Account).WithMany(x => x.Transactions).HasForeignKey(x => x.AccountNumber);
        }
    }
}
