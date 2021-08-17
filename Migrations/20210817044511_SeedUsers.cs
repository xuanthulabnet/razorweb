using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace razorweb.Migrations
{
    public partial class SeedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {  
            for (int i = 1; i < 150; i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[] {
                        "Id",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "HomeAdress"
                        

                     },
                    values: new object[] {
                        Guid.NewGuid().ToString(),
                        "User-"+i.ToString("D3"),
                        $"email{i.ToString("D3")}@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        false,
                        0,
                        "...@#%..." 
                    }
                );
            }

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
