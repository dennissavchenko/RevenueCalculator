using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Revenue.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientType",
                columns: table => new
                {
                    ClientTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientType", x => x.ClientTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    IdDiscount = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.IdDiscount);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatus",
                columns: table => new
                {
                    IdPaymentStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatus", x => x.IdPaymentStatus);
                });

            migrationBuilder.CreateTable(
                name: "SoftwareCategory",
                columns: table => new
                {
                    IdSoftwareCategory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareCategory", x => x.IdSoftwareCategory);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    IdClient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientTypeId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KRS = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PESEL = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.IdClient);
                    table.ForeignKey(
                        name: "FK_Client_ClientType_ClientTypeId",
                        column: x => x.ClientTypeId,
                        principalTable: "ClientType",
                        principalColumn: "ClientTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Software",
                columns: table => new
                {
                    IdSoftware = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CurrentVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubscriptionCost = table.Column<double>(type: "float", nullable: false),
                    UpfrontCost = table.Column<double>(type: "float", nullable: false),
                    IdSoftwareCategory = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Software", x => x.IdSoftware);
                    table.ForeignKey(
                        name: "FK_Software_SoftwareCategory_IdSoftwareCategory",
                        column: x => x.IdSoftwareCategory,
                        principalTable: "SoftwareCategory",
                        principalColumn: "IdSoftwareCategory",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    IdContract = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    AdditionalYearsOfSupport = table.Column<int>(type: "int", nullable: false),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    SoftwareVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdSoftware = table.Column<int>(type: "int", nullable: false),
                    IdClient = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.IdContract);
                    table.ForeignKey(
                        name: "FK_Contract_Client_IdClient",
                        column: x => x.IdClient,
                        principalTable: "Client",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contract_Software_IdSoftware",
                        column: x => x.IdSoftware,
                        principalTable: "Software",
                        principalColumn: "IdSoftware",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoftwareDiscount",
                columns: table => new
                {
                    IdDiscount = table.Column<int>(type: "int", nullable: false),
                    IdSoftware = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareDiscount", x => new { x.IdDiscount, x.IdSoftware });
                    table.ForeignKey(
                        name: "FK_SoftwareDiscount_Discount_IdDiscount",
                        column: x => x.IdDiscount,
                        principalTable: "Discount",
                        principalColumn: "IdDiscount",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SoftwareDiscount_Software_IdSoftware",
                        column: x => x.IdSoftware,
                        principalTable: "Software",
                        principalColumn: "IdSoftware",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    IdPayment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    IdContract = table.Column<int>(type: "int", nullable: false),
                    IdPaymentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.IdPayment);
                    table.ForeignKey(
                        name: "FK_Payment_Contract_IdContract",
                        column: x => x.IdContract,
                        principalTable: "Contract",
                        principalColumn: "IdContract",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_PaymentStatus_IdPaymentStatus",
                        column: x => x.IdPaymentStatus,
                        principalTable: "PaymentStatus",
                        principalColumn: "IdPaymentStatus",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_ClientTypeId",
                table: "Client",
                column: "ClientTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_KRS",
                table: "Client",
                column: "KRS",
                unique: true,
                filter: "[KRS] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Client_PESEL",
                table: "Client",
                column: "PESEL",
                unique: true,
                filter: "[PESEL] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_IdClient",
                table: "Contract",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_IdSoftware",
                table: "Contract",
                column: "IdSoftware");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_IdContract",
                table: "Payment",
                column: "IdContract");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_IdPaymentStatus",
                table: "Payment",
                column: "IdPaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Software_IdSoftwareCategory",
                table: "Software",
                column: "IdSoftwareCategory");

            migrationBuilder.CreateIndex(
                name: "IX_SoftwareDiscount_IdSoftware",
                table: "SoftwareDiscount",
                column: "IdSoftware");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "SoftwareDiscount");

            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropTable(
                name: "PaymentStatus");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Software");

            migrationBuilder.DropTable(
                name: "ClientType");

            migrationBuilder.DropTable(
                name: "SoftwareCategory");
        }
    }
}
