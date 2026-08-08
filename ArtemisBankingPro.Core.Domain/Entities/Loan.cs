using System;
using System.Collections.Generic;
using ArtemisBankingPro.Core.Domain.Enums;

namespace ArtemisBankingPro.Core.Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public string LoanNumber { get; set; } = string.Empty; // Número único de 9 dígitos
    
    // Relación con el Usuario (Dueño del préstamo)
    public int UserId { get; set; }
    public User? User { get; set; }

    // Datos Financieros del Préstamo
    public decimal DisbursedAmount { get; set; } // Monto Desembolsado
    public decimal AnnualInterestRate { get; set; } // Tasa Interés Anual
    public int TermInMonths { get; set; } // Plazo en Meses
    public decimal CurrentDebt { get; set; } // Deuda Actual
    
    // Estado (Activo, Pagado)
    public LoanStatus Status { get; set; }

    // Relación 1 a Muchos con Cuotas
    public ICollection<AmortizationInstallment> Installments { get; set; }

    public Loan()
    {
        Installments = new HashSet<AmortizationInstallment>();
    }
}
