using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtemisBankingPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixEntitiesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comercios_User_AssociatedUserId",
                table: "Comercios");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_User_UserId",
                table: "CreditCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CuentasAhorro_User_UserId",
                table: "CuentasAhorro");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_User_UserId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_CuentasAhorro_SavingsAccountId",
                table: "Transacciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_User_PerformedByCashierId",
                table: "Transacciones");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transacciones",
                table: "Transacciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CuentasAhorro",
                table: "CuentasAhorro");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comercios",
                table: "Comercios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Beneficiarios",
                table: "Beneficiarios");

            migrationBuilder.RenameTable(
                name: "Transacciones",
                newName: "Transactions");

            migrationBuilder.RenameTable(
                name: "CuentasAhorro",
                newName: "SavingsAccounts");

            migrationBuilder.RenameTable(
                name: "Comercios",
                newName: "Trades");

            migrationBuilder.RenameTable(
                name: "Beneficiarios",
                newName: "Beneficiaries");

            migrationBuilder.RenameIndex(
                name: "IX_Transacciones_SavingsAccountId",
                table: "Transactions",
                newName: "IX_Transactions_SavingsAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transacciones_PerformedByCashierId",
                table: "Transactions",
                newName: "IX_Transactions_PerformedByCashierId");

            migrationBuilder.RenameIndex(
                name: "IX_CuentasAhorro_UserId",
                table: "SavingsAccounts",
                newName: "IX_SavingsAccounts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comercios_AssociatedUserId",
                table: "Trades",
                newName: "IX_Trades_AssociatedUserId");

            migrationBuilder.AddColumn<string>(
                name: "LoanNumber",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavingsAccounts",
                table: "SavingsAccounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trades",
                table: "Trades",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Beneficiaries",
                table: "Beneficiaries",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Users_UserId",
                table: "CreditCards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Users_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavingsAccounts_Users_UserId",
                table: "SavingsAccounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Users_AssociatedUserId",
                table: "Trades",
                column: "AssociatedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_SavingsAccounts_SavingsAccountId",
                table: "Transactions",
                column: "SavingsAccountId",
                principalTable: "SavingsAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_PerformedByCashierId",
                table: "Transactions",
                column: "PerformedByCashierId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Users_UserId",
                table: "CreditCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Users_UserId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_SavingsAccounts_Users_UserId",
                table: "SavingsAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Users_AssociatedUserId",
                table: "Trades");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_SavingsAccounts_SavingsAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_PerformedByCashierId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trades",
                table: "Trades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavingsAccounts",
                table: "SavingsAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Beneficiaries",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "LoanNumber",
                table: "Loans");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transacciones");

            migrationBuilder.RenameTable(
                name: "Trades",
                newName: "Comercios");

            migrationBuilder.RenameTable(
                name: "SavingsAccounts",
                newName: "CuentasAhorro");

            migrationBuilder.RenameTable(
                name: "Beneficiaries",
                newName: "Beneficiarios");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_SavingsAccountId",
                table: "Transacciones",
                newName: "IX_Transacciones_SavingsAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_PerformedByCashierId",
                table: "Transacciones",
                newName: "IX_Transacciones_PerformedByCashierId");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_AssociatedUserId",
                table: "Comercios",
                newName: "IX_Comercios_AssociatedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_SavingsAccounts_UserId",
                table: "CuentasAhorro",
                newName: "IX_CuentasAhorro_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transacciones",
                table: "Transacciones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comercios",
                table: "Comercios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CuentasAhorro",
                table: "CuentasAhorro",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Beneficiarios",
                table: "Beneficiarios",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    TradeId = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Comercios_User_AssociatedUserId",
                table: "Comercios",
                column: "AssociatedUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_User_UserId",
                table: "CreditCards",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasAhorro_User_UserId",
                table: "CuentasAhorro",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_User_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_CuentasAhorro_SavingsAccountId",
                table: "Transacciones",
                column: "SavingsAccountId",
                principalTable: "CuentasAhorro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_User_PerformedByCashierId",
                table: "Transacciones",
                column: "PerformedByCashierId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
