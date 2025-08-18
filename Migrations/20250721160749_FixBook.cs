using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeDbApi.Migrations
{
    /// <inheritdoc />
    public partial class FixBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "isbn13",
                table: "books",
                type: "varchar(13)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(13)");

            migrationBuilder.AlterColumn<string>(
                name: "isbn10",
                table: "books",
                type: "varchar(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<byte>(
                name: "edition",
                table: "books",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "books",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "publication_date_raw",
                table: "books",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "publication_date_raw",
                table: "books");

            migrationBuilder.AlterColumn<string>(
                name: "isbn13",
                table: "books",
                type: "varchar(13)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(13)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "isbn10",
                table: "books",
                type: "varchar(10)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "edition",
                table: "books",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "books",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
