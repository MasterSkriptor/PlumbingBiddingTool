using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlumbingBiddingTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedFixtureItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BidItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixtureItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixtureItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixtureItemBidItems",
                columns: table => new
                {
                    BidItemsId = table.Column<int>(type: "INTEGER", nullable: false),
                    FixtureItemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixtureItemBidItems", x => new { x.BidItemsId, x.FixtureItemId });
                    table.ForeignKey(
                        name: "FK_FixtureItemBidItems_BidItems_BidItemsId",
                        column: x => x.BidItemsId,
                        principalTable: "BidItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixtureItemBidItems_FixtureItems_FixtureItemId",
                        column: x => x.FixtureItemId,
                        principalTable: "FixtureItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FixtureItemBidItems_FixtureItemId",
                table: "FixtureItemBidItems",
                column: "FixtureItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FixtureItemBidItems");

            migrationBuilder.DropTable(
                name: "BidItems");

            migrationBuilder.DropTable(
                name: "FixtureItems");
        }
    }
}
