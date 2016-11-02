﻿using Ds.Data.Conventions;
using GWA.Domaine.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWA.Data.Context
{
   public class GWAContext :DbContext
    {
        public GWAContext()
              : base("GWADB")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Conventions.Add(new DatetimeConvention());
            
            modelBuilder.Conventions.Add(new KeyConvention());
        }

        DbSet<User> users { get; set; }
        DbSet<Product> products { get; set; }
        DbSet<Auction> auctions { get; set; }
        DbSet<Token> tokens { get; set; }
        DbSet<Category> category { get; set; }
        DbSet<Notification> notifications { get; set; }
        DbSet<Bid> bids { get; set; }
        DbSet<Payment> payments { get; set; }
        DbSet<Session> sessions { get; set; }
        DbSet<ShoppingCart> shoppingCarts { get; set; }
        //DbSet<Subscription> subscription { get; set; }
        DbSet<Command> commands { get; set; }
    }
}
