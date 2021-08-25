using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class RemoveTakeawayFromDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Takeaway",
                table: "DeliveryMethods");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Takeaway",
                table: "DeliveryMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
