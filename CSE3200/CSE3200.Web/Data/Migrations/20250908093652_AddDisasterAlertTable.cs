using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSE3200.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDisasterAlertTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisasterAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisasterAlerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisasterAlerts_DisplayOrder",
                table: "DisasterAlerts",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_DisasterAlerts_EndDate",
                table: "DisasterAlerts",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_DisasterAlerts_IsActive",
                table: "DisasterAlerts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DisasterAlerts_Severity",
                table: "DisasterAlerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_DisasterAlerts_StartDate",
                table: "DisasterAlerts",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisasterAlerts");
        }
    }
}
