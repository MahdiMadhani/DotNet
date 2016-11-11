using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using InMemoryCaching.Models;

namespace InMemoryCaching.Migrations
{
    [DbContext(typeof(DALContext))]
    [Migration("20161029130001_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InMemoryCaching.Models.Account.LoginInfo", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("EmailConfirmationKey")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<bool?>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 80);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 80);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.Property<int>("RoleId");

                    b.HasKey("UserId");

                    b.HasAnnotation("Relational:TableName", "tblLogins");
                });

            modelBuilder.Entity("InMemoryCaching.Models.Account.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LoginInfoUserId");

                    b.Property<string>("Rolename");

                    b.HasKey("RoleId");
                });

            modelBuilder.Entity("InMemoryCaching.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("AssetData");

                    b.Property<string>("AssetType");

                    b.Property<string>("Titel")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "tblAlbums");
                });

            modelBuilder.Entity("InMemoryCaching.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .HasAnnotation("MaxLength", 1000);

                    b.Property<int>("PhotoId");

                    b.Property<string>("WriterEmail");

                    b.Property<string>("WriterName")
                        .HasAnnotation("MaxLength", 300);

                    b.Property<bool>("status");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "tblComments");
                });

            modelBuilder.Entity("InMemoryCaching.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AlbumID");

                    b.Property<byte[]>("ImgData");

                    b.Property<string>("ImgType");

                    b.Property<string>("Titel")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "tblPhotoes");
                });

            modelBuilder.Entity("InMemoryCaching.Models.Account.Role", b =>
                {
                    b.HasOne("InMemoryCaching.Models.Account.LoginInfo")
                        .WithMany()
                        .HasForeignKey("LoginInfoUserId");
                });

            modelBuilder.Entity("InMemoryCaching.Models.Comment", b =>
                {
                    b.HasOne("InMemoryCaching.Models.Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId");
                });

            modelBuilder.Entity("InMemoryCaching.Models.Photo", b =>
                {
                    b.HasOne("InMemoryCaching.Models.Album")
                        .WithMany()
                        .HasForeignKey("AlbumID");
                });
        }
    }
}
