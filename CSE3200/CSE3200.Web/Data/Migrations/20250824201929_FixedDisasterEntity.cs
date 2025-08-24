using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSE3200.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixedDisasterEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Disasters",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "Disasters",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Disasters_CreatedBy",
                table: "Disasters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Disasters_CreatedDate",
                table: "Disasters",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Disasters_Status",
                table: "Disasters",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Disasters_CreatedBy",
                table: "Disasters");

            migrationBuilder.DropIndex(
                name: "IX_Disasters_CreatedDate",
                table: "Disasters");

            migrationBuilder.DropIndex(
                name: "IX_Disasters_Status",
                table: "Disasters");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Disasters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "Disasters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);
        }
    }
}
