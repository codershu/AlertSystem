using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemAlert.Data.Context.Models;

namespace SystemAlert.Data.Context
{
    public class AdminContext : DbContext
    {
        protected AdminContext() : base() { }

        public AdminContext(DbContextOptions<AdminContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alert> Alert { get; set; }
        public virtual DbSet<AlertConfiguration> AlertConfiguration { get; set; }
        public virtual DbSet<AlertType> AlertType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
