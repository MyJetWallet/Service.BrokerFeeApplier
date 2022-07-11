using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BrokerFeeApplier.Postgres.Migrations
{
    public partial class type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "broker_fee_applier",
                table: "fireblocks_fee_application",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "broker_fee_applier",
                table: "fireblocks_fee_application");
        }
    }
}
