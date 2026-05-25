using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BankingAPI.Migrations
{
    /// <inheritdoc />
    public partial class Transaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionReferenceNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FromAccountNumber = table.Column<string>(type: "text", nullable: false),
                    ToAccountNumber = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionNumber", x => x.TransactionReferenceNumber);
                    table.ForeignKey(
                        name: "FK_Transaction_FromAccount",
                        column: x => x.FromAccountNumber,
                        principalTable: "Accounts",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_ToAccount",
                        column: x => x.ToAccountNumber,
                        principalTable: "Accounts",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FromAccountNumber",
                table: "Transactions",
                column: "FromAccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ToAccountNumber",
                table: "Transactions",
                column: "ToAccountNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
