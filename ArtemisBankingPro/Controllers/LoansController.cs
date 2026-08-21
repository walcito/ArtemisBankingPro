using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.ViewModels.Loan;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetLoans;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;

namespace ArtemisBankingPro.WebApp.Controllers;

[AllowAnonymous]
public class LoansController : Controller
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(
        int pageNumber = 1, int pageSize = 10, string? identificationNumber = null, LoanStatus? status = null)
    {
        var query = new GetLoansQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IdentificationNumber = identificationNumber,
            Status = status
        };

        var result = await _mediator.Send(query);

        var vm = new LoanListViewModel
        {
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            Items = result.Items.Select(l => new LoanViewModel
            {
                Id = l.Id,
                LoanNumber = l.LoanNumber,
                DisbursedAmount = l.DisbursedAmount,
                AnnualInterestRate = l.AnnualInterestRate,
                TermInMonths = l.TermInMonths,
                CurrentDebt = l.CurrentDebt,
                Status = l.Status,
                UserFullName = l.UserFullName,
                UserIdentificationNumber = l.UserIdentificationNumber
            }).ToList()
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var query = new ObtenerDetallePrestamoQuery { LoanId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        var vm = new LoanDetailViewModel
        {
            Id = result.Id,
            LoanNumber = result.LoanNumber,
            DisbursedAmount = result.DisbursedAmount,
            AnnualInterestRate = result.AnnualInterestRate,
            TermInMonths = result.TermInMonths,
            CurrentDebt = result.CurrentDebt,
            Status = result.Status,
            UserId = result.UserId,
            UserFullName = result.UserFullName,
            UserIdentificationNumber = result.UserIdentificationNumber,
            Installments = result.Installments.Select(i => new LoanInstallmentViewModel
            {
                Id = i.Id,
                InstallmentNumber = i.InstallmentNumber,
                InstallmentAmount = i.InstallmentAmount,
                Interest = i.Interest,
                PrincipalAmortization = i.PrincipalAmortization,
                PendingBalance = i.PendingBalance,
                Status = i.Status,
                PaidAmount = i.PaidAmount
            }).ToList()
        };

        return View(vm);
    }
}
