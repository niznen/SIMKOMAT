using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SelfSIMCard.Models
{
    public class SelfSIMContext: DbContext
    {
        public SelfSIMContext() : base("Lebara")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<SIMOrder> SimOrders { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<SMKToken> SMKTokens { get; set; }
        public DbSet<IAMSession> IAMSessions { get; set; }
        public DbSet<StoreSIM> StoreSIMs { get; set; }
    }
}