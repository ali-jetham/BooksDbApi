using System;
using System.Collections.Generic;
using LifeDbApi.Models.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeDbApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:book_source", "goodreads,google,open_library")
                .Annotation("Npgsql:Enum:book_status", "abandoned,paused,read,reading,to_read")
                .Annotation("Npgsql:Enum:oauth_provider", "apple,facebook,google,microsoft");

            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<BookSource>(type: "book_source", nullable: false),
                    isbn10 = table.Column<string>(type: "varchar(10)", nullable: false),
                    isbn13 = table.Column<string>(type: "varchar(13)", nullable: false),
                    genre = table.Column<List<string>>(type: "text[]", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    authors = table.Column<List<string>>(type: "text[]", nullable: false),
                    page_count = table.Column<int>(type: "integer", nullable: true),
                    publication_date = table.Column<DateOnly>(type: "date", nullable: true),
                    publisher = table.Column<List<string>>(type: "text[]", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    cover_url = table.Column<string>(type: "text", nullable: false),
                    series = table.Column<string>(type: "text", nullable: true),
                    edition = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_books", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    o_auth_provider = table.Column<OAuthProvider>(type: "oauth_provider", nullable: false),
                    sub = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "collections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(64)", nullable: false),
                    description = table.Column<string>(type: "varchar(300)", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_collections", x => x.id);
                    table.ForeignKey(
                        name: "fk_collections_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    revoked = table.Column<bool>(type: "boolean", nullable: false),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_books",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<BookStatus>(type: "book_status", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: true),
                    date_added = table.Column<DateOnly>(type: "date", nullable: true),
                    date_started = table.Column<DateOnly>(type: "date", nullable: true),
                    date_finished = table.Column<DateOnly>(type: "date", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: false),
                    favorite = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_books", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_books_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_books_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_collections_user_id",
                table: "collections",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_books_book_id",
                table: "user_books",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_books_user_id",
                table: "user_books",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "collections");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "user_books");

            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
