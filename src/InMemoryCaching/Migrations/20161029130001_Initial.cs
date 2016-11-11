using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace InMemoryCaching.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblLogins",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: false),
                    EmailConfirmationKey = table.Column<string>(nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: true),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginInfo", x => x.UserId);
                });
            migrationBuilder.CreateTable(
                name: "tblAlbums",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetData = table.Column<byte[]>(nullable: true),
                    AssetType = table.Column<string>(nullable: true),
                    Titel = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Album", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LoginInfoUserId = table.Column<string>(nullable: true),
                    Rolename = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Role_LoginInfo_LoginInfoUserId",
                        column: x => x.LoginInfoUserId,
                        principalTable: "tblLogins",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "tblPhotoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlbumID = table.Column<int>(nullable: false),
                    ImgData = table.Column<byte[]>(nullable: true),
                    ImgType = table.Column<string>(nullable: true),
                    Titel = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photo_Album_AlbumID",
                        column: x => x.AlbumID,
                        principalTable: "tblAlbums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "tblComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    PhotoId = table.Column<int>(nullable: false),
                    WriterEmail = table.Column<string>(nullable: true),
                    WriterName = table.Column<string>(nullable: true),
                    status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Photo_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "tblPhotoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Role");
            migrationBuilder.DropTable("tblComments");
            migrationBuilder.DropTable("tblLogins");
            migrationBuilder.DropTable("tblPhotoes");
            migrationBuilder.DropTable("tblAlbums");
        }
    }
}
