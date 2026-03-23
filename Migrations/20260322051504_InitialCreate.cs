using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.upscale.app.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FathersSurname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MothersSurname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    DocumentNumber = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Peruana"),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MainEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SecondaryEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MainPhone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    ContractType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HiringDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserPasswordEncrypt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CVF = table.Column<int>(type: "int", nullable: false),
                    ValidationCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DocumentNumber",
                table: "Users",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MainEmail",
                table: "Users",
                column: "MainEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MainPhone",
                table: "Users",
                column: "MainPhone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
