using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Application.Services;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtemisBankingPro.Infrastructure.Persistence.Seed;

public class DataSeeder
{
    public static async Task SeedAsync(ArtemisBankingDbContext context)
    {
        // para datos de prueba: cliente, préstamo, tarjeta, comercio
        await PrestamoSeed.SeedAsync(context);
    }
}

public static class PrestamoSeed
{
    public static async Task SeedAsync(ArtemisBankingDbContext context)
    {
        var dbUsers = context.Set<User>();
        var dbAccounts = context.Set<SavingsAccount>();
        var dbLoans = context.Loans;

        // Si ya hay usuarios, asumimos que ya se sembró o hay datos existentes
        if (await dbUsers.AnyAsync())
        {
            return;
        }

        // 1. Crear Clientes
        var client1 = new User
        {
            FirstName = "Juan",
            LastName = "Pérez",
            IdentificationNumber = "40200000001",
            Email = "juan.perez@example.com",
            Username = "juanp",
            Role = Roles.Client,
            IsActive = true
        };

        var client2 = new User
        {
            FirstName = "María",
            LastName = "Rodríguez",
            IdentificationNumber = "40200000002",
            Email = "maria.rodriguez@example.com",
            Username = "mariar",
            Role = Roles.Client,
            IsActive = true
        };

        var client3 = new User
        {
            FirstName = "Pedro",
            LastName = "Martínez",
            IdentificationNumber = "40200000003",
            Email = "pedro.martinez@example.com",
            Username = "pedrom",
            Role = Roles.Client,
            IsActive = true
        };

        await dbUsers.AddRangeAsync(client1, client2, client3);
        await context.SaveChangesAsync();

        // 2. Crear Cuentas de Ahorro
        var account1 = new SavingsAccount
        {
            AccountNumber = "100000001",
            Balance = 150000.00m,
            UserId = client1.Id,
            Type = AccountType.Principal,
            Status = AccountStatus.Active
        };

        var account2 = new SavingsAccount
        {
            AccountNumber = "100000002",
            Balance = 250000.00m,
            UserId = client2.Id,
            Type = AccountType.Principal,
            Status = AccountStatus.Active
        };

        var account3 = new SavingsAccount
        {
            AccountNumber = "100000003",
            Balance = 5000.00m,
            UserId = client3.Id,
            Type = AccountType.Principal,
            Status = AccountStatus.Active
        };

        await dbAccounts.AddRangeAsync(account1, account2, account3);
        await context.SaveChangesAsync();

        // Instanciar servicio de amortización
        var amortizationService = new CalculadoraAmortizationService();

        // 3. Crear Préstamos
        // Préstamo 1: Juan Pérez (Activo, amortizado parcialmente)
        var loan1 = new Loan
        {
            LoanNumber = "900000001",
            UserId = client1.Id,
            DisbursedAmount = 120000.00m,
            AnnualInterestRate = 18.0m,
            TermInMonths = 12,
            Status = LoanStatus.Active
        };

        var installments1 = amortizationService.GenerateAmortizationTable(
            loan1.DisbursedAmount,
            loan1.AnnualInterestRate,
            loan1.TermInMonths
        );

        // Para el préstamo 1, marcar las primeras 4 cuotas como completamente pagadas
        // y la 5ta cuota como parcialmente pagada con RD$ 3,000.00
        decimal accumulatedPaidPrincipal = 0m;
        for (int i = 0; i < installments1.Count; i++)
        {
            var inst = installments1[i];
            if (i < 4) // Cuotas 1, 2, 3, 4
            {
                inst.Status = InstallmentStatus.Paid;
                inst.PaidAmount = inst.InstallmentAmount;
                accumulatedPaidPrincipal += inst.PrincipalAmortization;
            }
            else if (i == 4) // Cuota 5
            {
                decimal partialPayment = 3000.00m;
                inst.Status = InstallmentStatus.PartiallyPaid;
                inst.PaidAmount = partialPayment;

                // Restar interés primero. El resto amortiza capital.
                decimal interestPaid = Math.Min(partialPayment, inst.Interest);
                decimal principalPaid = Math.Max(0m, partialPayment - interestPaid);
                accumulatedPaidPrincipal += principalPaid;
            }
            // Las demás quedan Pending con PaidAmount = 0
            loan1.Installments.Add(inst);
        }

        loan1.CurrentDebt = Math.Max(0m, loan1.DisbursedAmount - accumulatedPaidPrincipal);
        await dbLoans.AddAsync(loan1);

        // Préstamo 2: María Rodríguez (Pagado)
        var loan2 = new Loan
        {
            LoanNumber = "900000002",
            UserId = client2.Id,
            DisbursedAmount = 50000.00m,
            AnnualInterestRate = 15.0m,
            TermInMonths = 6,
            Status = LoanStatus.Paid,
            CurrentDebt = 0.00m
        };

        var installments2 = amortizationService.GenerateAmortizationTable(
            loan2.DisbursedAmount,
            loan2.AnnualInterestRate,
            loan2.TermInMonths
        );

        foreach (var inst in installments2)
        {
            inst.Status = InstallmentStatus.Paid;
            inst.PaidAmount = inst.InstallmentAmount;
            loan2.Installments.Add(inst);
        }
        await dbLoans.AddAsync(loan2);

        // Préstamo 3: Pedro Martínez (Activo, sin pagos)
        var loan3 = new Loan
        {
            LoanNumber = "900000003",
            UserId = client3.Id,
            DisbursedAmount = 200000.00m,
            AnnualInterestRate = 20.0m,
            TermInMonths = 24,
            Status = LoanStatus.Active,
            CurrentDebt = 200000.00m
        };

        var installments3 = amortizationService.GenerateAmortizationTable(
            loan3.DisbursedAmount,
            loan3.AnnualInterestRate,
            loan3.TermInMonths
        );

        foreach (var inst in installments3)
        {
            inst.Status = InstallmentStatus.Pending;
            inst.PaidAmount = 0.00m;
            loan3.Installments.Add(inst);
        }
        await dbLoans.AddAsync(loan3);

        await context.SaveChangesAsync();
    }
}
