using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "price_tick",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Bid = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Ask = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_tick", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    ActionAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_audit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_price_tick_Symbol",
                table: "price_tick",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_price_tick_TimestampUtc",
                table: "price_tick",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_audit_Symbol",
                table: "subscription_audit",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_audit_UserId",
                table: "subscription_audit",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "price_tick");

            migrationBuilder.DropTable(
                name: "subscription_audit");
        }
    }
}
