using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AssignDeliveryWithPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryMethodPaymentMethod",
                columns: table => new
                {
                    DeliveryMethodsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryMethodPaymentMethod", x => new { x.DeliveryMethodsId, x.PaymentMethodsId });
                    table.ForeignKey(
                        name: "FK_DeliveryMethodPaymentMethod_DeliveryMethods_DeliveryMethodsId",
                        column: x => x.DeliveryMethodsId,
                        principalTable: "DeliveryMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryMethodPaymentMethod_PaymentMethods_PaymentMethodsId",
                        column: x => x.PaymentMethodsId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryMethodPaymentMethod_PaymentMethodsId",
                table: "DeliveryMethodPaymentMethod",
                column: "PaymentMethodsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryMethodPaymentMethod");
        }
    }
}
