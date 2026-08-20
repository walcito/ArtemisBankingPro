using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtemisBankingPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Users_AssociatedUserId",
                table: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_Trades_AssociatedUserId",
                table: "Trades");

            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Beneficiaries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryAccountNumber",
                table: "Beneficiaries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "Beneficiaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TradeId",
                table: "Users",
                column: "TradeId",
                unique: true,
                filter: "[TradeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_OwnerUserId_BeneficiaryAccountNumber",
                table: "Beneficiaries",
                columns: new[] { "OwnerUserId", "BeneficiaryAccountNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Beneficiaries_Users_OwnerUserId",
                table: "Beneficiaries",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Trades_TradeId",
                table: "Users",
                column: "TradeId",
                principalTable: "Trades",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beneficiaries_Users_OwnerUserId",
                table: "Beneficiaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Trades_TradeId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TradeId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Beneficiaries_OwnerUserId_BeneficiaryAccountNumber",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "BeneficiaryAccountNumber",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Beneficiaries");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_AssociatedUserId",
                table: "Trades",
                column: "AssociatedUserId",
                unique: true,
                filter: "[AssociatedUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Users_AssociatedUserId",
                table: "Trades",
                column: "AssociatedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
