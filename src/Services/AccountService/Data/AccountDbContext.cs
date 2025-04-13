using AccountService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AccountService.Data
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
    }
}
