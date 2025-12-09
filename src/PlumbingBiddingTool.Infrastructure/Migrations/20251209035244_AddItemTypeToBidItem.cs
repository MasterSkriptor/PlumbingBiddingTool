using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlumbingBiddingTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemTypeToBidItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "BidItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "BidItems");
        }
    }
}
