using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcAppTest.Migrations
{
    public partial class AddIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Order_Date",
                table: "Order",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_Date",
                table: "Order");
        }
    }
}
