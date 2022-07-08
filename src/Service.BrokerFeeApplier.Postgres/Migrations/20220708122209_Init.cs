using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Service.BrokerFeeApplier.Postgres.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "broker_fee_applier");

            migrationBuilder.CreateTable(
                name: "fireblocks_fee_application",
                schema: "broker_fee_applier",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    AssetSymbol = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Comment = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FeeAssetSymbol = table.Column<string>(type: "text", nullable: true),
                    Network = table.Column<string>(type: "text", nullable: true),
                    InternalNote = table.Column<string>(type: "text", nullable: true),
                    DestinationAddress = table.Column<string>(type: "text", nullable: true),
                    DestinationTag = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FeeApplicationIdInMe = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fireblocks_fee_application", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fireblocks_fee_application_Status",
                schema: "broker_fee_applier",
                table: "fireblocks_fee_application",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_fireblocks_fee_application_TransactionId",
                schema: "broker_fee_applier",
                table: "fireblocks_fee_application",
                column: "TransactionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fireblocks_fee_application",
                schema: "broker_fee_applier");
        }
    }
}
