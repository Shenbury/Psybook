using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Psybook.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedDictionaryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingSlots",
                table: "BookingSlots");

            migrationBuilder.RenameTable(
                name: "BookingSlots",
                newName: "CalendarSlots");

            migrationBuilder.AlterColumn<int>(
                name: "Color",
                table: "CalendarSlots",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "BookingExperience",
                table: "CalendarSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "CalendarSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstLineAddress",
                table: "CalendarSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "CalendarSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "CalendarSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "CalendarSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalendarSlots",
                table: "CalendarSlots",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ExperienceRecords",
                columns: table => new
                {
                    BookingExperience = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllDay = table.Column<bool>(type: "bit", nullable: false),
                    Length = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceRecords", x => x.BookingExperience);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExperienceRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalendarSlots",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "BookingExperience",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "FirstLineAddress",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "CalendarSlots");

            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "CalendarSlots");

            migrationBuilder.RenameTable(
                name: "CalendarSlots",
                newName: "BookingSlots");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "BookingSlots",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingSlots",
                table: "BookingSlots",
                column: "Id");
        }
    }
}
