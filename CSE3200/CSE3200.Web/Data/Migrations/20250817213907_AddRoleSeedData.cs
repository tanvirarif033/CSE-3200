using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CSE3200.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("02352cdf-e55b-458d-91f0-6063ec9300df"), "8/18/2025 1:02:04 AM", "Volunteer", "VOLUNTEER" },
                    { new Guid("1d348b36-86e9-4e74-9c10-7fb59d035468"), "8/18/2025 1:02:01 AM", "Admin", "ADMIN" },
                    { new Guid("ae3e5918-2742-4bac-95ff-2326c1e5966e"), "8/18/2025 1:02:03 AM", "Donor", "DONAR" },
                    { new Guid("d716bb55-e6b5-4639-a396-2d8c81f30e32"), "8/18/2025 1:02:04 AM", "Field Representative", "FIELD REPRESENTATIVE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("02352cdf-e55b-458d-91f0-6063ec9300df"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1d348b36-86e9-4e74-9c10-7fb59d035468"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ae3e5918-2742-4bac-95ff-2326c1e5966e"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d716bb55-e6b5-4639-a396-2d8c81f30e32"));
        }
    }
}
