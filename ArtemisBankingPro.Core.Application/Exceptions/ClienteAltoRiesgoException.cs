using System;

namespace ArtemisBankingPro.Core.Application.Exceptions;

public class ClienteAltoRiesgoException : Exception
{
    public ClienteAltoRiesgoException(string message) : base(message)
    {
    }
}
