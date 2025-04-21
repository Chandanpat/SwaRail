using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Railway_Reservation_Sys.Data
{
    public class RailwayReservationAuthDbContext:IdentityDbContext
    {
        public RailwayReservationAuthDbContext(DbContextOptions<RailwayReservationAuthDbContext> options)
        : base(options){
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userRoleId = "e1106691-dea0-49a6-a7ee-3c174a336ee1";
            var AdminRoleId = "f1d154c3-4091-4bcd-89b2-58ad922af199"; 

            var roles = new List<IdentityRole>{
                new IdentityRole{
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId,
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                },
                new IdentityRole{
                    Id = AdminRoleId,
                    ConcurrencyStamp = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}