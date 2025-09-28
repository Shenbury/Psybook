using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Psybook.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStatusManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the primary key constraint on ExperienceRecords before altering the column
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperienceRecords",
                table: "ExperienceRecords");

            migrationBuilder.AlterColumn<string>(
                name: "BookingExperience",
                table: "ExperienceRecords",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Recreate the primary key constraint
            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperienceRecords",
                table: "ExperienceRecords",
                column: "BookingExperience");

            migrationBuilder.AlterColumn<string>(
                name: "BookingExperience",
                table: "CalendarSlots",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "CalendarSlots",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "CalendarSlots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CalendarSlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "CalendarSlots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "CalendarSlots",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "CalendarSlots",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CalendarSlots",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSlots_BookingExperience",
                table: "CalendarSlots",
                column: "BookingExperience");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSlots_CreatedAt",
                table: "CalendarSlots",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSlots_Start",
                table: "CalendarSlots",
                column: "Start");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSlots_Status",
                table: "CalendarSlots",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSlots_Status_Start",
                table: "CalendarSlots",
                columns: new[] { "Status", "Start" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CalendarSlots_BookingExperience",
                table: "CalendarSlots");

            migrationBuilder.DropIndex(
                name: "IX_CalendarSlots_CreatedAt",
                table: "CalendarSlots");

            migrationBuilder.DropIndex(
                name: "IX_CalendarSlots_Start",
                table: "CalendarSlots");

            migrationBuilder.DropIndex(
                name: "IX_CalendarSlots_Status",
                table: "CalendarSlots");

            migrationBuilder.DropIndex(
                name: "IX_CalendarSlots_Status_Start",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CalendarSlots");

            // Drop the primary key constraint on ExperienceRecords before altering the column back
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperienceRecords",
                table: "ExperienceRecords");

            migrationBuilder.AlterColumn<int>(
                name: "BookingExperience",
                table: "ExperienceRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // Recreate the primary key constraint
            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperienceRecords",
                table: "ExperienceRecords",
                column: "BookingExperience");

            migrationBuilder.AlterColumn<int>(
                name: "BookingExperience",
                table: "CalendarSlots",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
