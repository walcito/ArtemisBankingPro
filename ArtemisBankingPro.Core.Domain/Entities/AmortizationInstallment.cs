using System;
using ArtemisBankingPro.Core.Domain.Enums;

namespace ArtemisBankingPro.Core.Domain.Entities;

public class AmortizationInstallment
{
    public int Id { get; set; }

    // Relación con el Préstamo
    public int LoanId { get; set; }
    public Loan Loan { get; set; }

    // Datos de la Cuota, se utiliza el sistema de amortización francés
    
    public int InstallmentNumber { get; set; } // Número de cuota
    public decimal InstallmentAmount { get; set; } // Cuota fija del sistema francés
    public decimal Interest { get; set; } // Interés cobrado en esta cuota
    public decimal PrincipalAmortization { get; set; } // Capital reducido (Amortización Capital)
    public decimal PendingBalance { get; set; } // Saldo después de esta cuota
    
    // Estado de la cuota (Pendiente, ParcialmentePagada, Pagada)
    public InstallmentStatus Status { get; set; }
    
    // Rastreo de pagos parciales si aplica la regla anti-sobrepago o pagos que no cubren todo
    public decimal PaidAmount { get; set; }

    public AmortizationInstallment()
    {
    }
}
