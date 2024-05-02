using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventlyBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddedPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PosterId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Users_PosterId",
                        column: x => x.PosterId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_EventId",
                table: "Posts",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PosterId",
                table: "Posts",
                column: "PosterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
