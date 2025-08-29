using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSE3200.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DonorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DonorEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DonorPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DonationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donations_Disasters_DisasterId",
                        column: x => x.DisasterId,
                        principalTable: "Disasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DisasterId",
                table: "Donations",
                column: "DisasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonationDate",
                table: "Donations",
                column: "DonationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonorUserId",
                table: "Donations",
                column: "DonorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_PaymentStatus",
                table: "Donations",
                column: "PaymentStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Donations");
        }
    }
}
