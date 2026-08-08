using System;

namespace ArtemisBankingPro.Core.Application.Exceptions;

public class HighRiskLoanException : Exception
{
    public HighRiskLoanException(string message) : base(message)
    {
    }
}
