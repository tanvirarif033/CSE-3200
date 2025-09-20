using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSE3200.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceUserVolunteerAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActualHours",
                table: "VolunteerAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "VolunteerAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedHours",
                table: "VolunteerAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "VolunteerAssignments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "VolunteerAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVolunteerRequested",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VolunteerRequestDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VolunteerRequestStatus",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerAssignments_AssignedDate",
                table: "VolunteerAssignments",
                column: "AssignedDate");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerAssignments_EndDate",
                table: "VolunteerAssignments",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerAssignments_StartDate",
                table: "VolunteerAssignments",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsVolunteerRequested",
                table: "AspNetUsers",
                column: "IsVolunteerRequested");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RegistrationDate",
                table: "AspNetUsers",
                column: "RegistrationDate");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VolunteerRequestStatus",
                table: "AspNetUsers",
                column: "VolunteerRequestStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VolunteerAssignments_AssignedDate",
                table: "VolunteerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerAssignments_EndDate",
                table: "VolunteerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerAssignments_StartDate",
                table: "VolunteerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsVolunteerRequested",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RegistrationDate",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VolunteerRequestStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ActualHours",
                table: "VolunteerAssignments");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "VolunteerAssignments");

            migrationBuilder.DropColumn(
                name: "EstimatedHours",
                table: "VolunteerAssignments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "VolunteerAssignments");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "VolunteerAssignments");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsVolunteerRequested",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VolunteerRequestDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VolunteerRequestStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
