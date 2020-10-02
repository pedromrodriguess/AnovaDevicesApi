using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DevicesApi.Domain;

namespace DevicesApi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<DevicesApi.Domain.Device> Devices { get; set; }
        public DbSet<DevicesApi.Domain.Reading> Readings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Reading>()
                .HasKey(c => new { c.Timestamp, c.Device_id });
        }
    }
}
