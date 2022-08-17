using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitBaristaDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "barista");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "barista_orders",
                schema: "barista",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    item_type = table.Column<int>(type: "integer", nullable: false),
                    item_name = table.Column<string>(type: "text", nullable: false),
                    time_up = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_barista_orders", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_barista_orders_id",
                schema: "barista",
                table: "barista_orders",
                column: "id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "barista_orders",
                schema: "barista");
        }
    }
}
