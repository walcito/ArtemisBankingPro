using ArtemisBankingPro.Core.Application.Exceptions;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.ViewModels.Loan;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Admin, Administrator")]
public class LoansController : Controller
{
    private readonly ILoanService _loanService;
    private readonly IMapper _mapper;

    public LoansController(ILoanService loanService, IMapper mapper)
    {
        _loanService = loanService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(
        int pageNumber = 1, int pageSize = 20, string? identificationNumber = null, LoanStatus? status = null)
    {
        var result = await _loanService.GetLoansAsync(pageNumber, pageSize, identificationNumber, status);

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
        var result = await _loanService.GetLoanDetailAsync(id);

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

    [HttpGet]
    public async Task<IActionResult> SelectClient(string? identificationNumber)
    {
        var result = await _loanService.GetEligibleClientsAsync(identificationNumber);
        var vm = new SelectClientForLoanViewModel
        {
            AverageDebt = result.AverageDebt,
            SearchIdentification = identificationNumber,
            Clients = _mapper.Map<List<EligibleClientViewModel>>(result.Clients)
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> AssignForm(int userId)
    {
        var result = await _loanService.GetEligibleClientsAsync();
        var client = result.Clients.FirstOrDefault(c => c.Id == userId);
        
        if (client == null)
        {
            TempData["ErrorMessage"] = "El cliente seleccionado no existe, no está activo o ya tiene un préstamo.";
            return RedirectToAction(nameof(SelectClient));
        }

        var vm = new AssignLoanFormViewModel
        {
            UserId = client.Id,
            ClientFullName = client.FullName,
            ClientIdentification = client.IdentificationNumber
        };
        
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Assign(AssignLoanFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var result = await _loanService.GetEligibleClientsAsync();
            var client = result.Clients.FirstOrDefault(c => c.Id == vm.UserId);
            if (client != null)
            {
                vm.ClientFullName = client.FullName;
                vm.ClientIdentification = client.IdentificationNumber;
            }
            return View("AssignForm", vm);
        }

        try
        {
            await _loanService.AssignLoanAsync(vm.UserId, vm.DisbursedAmount, vm.AnnualInterestRate, vm.TermInMonths, false);
            TempData["SuccessMessage"] = "Préstamo asignado exitosamente y desembolsado en la cuenta del cliente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ClienteAltoRiesgoException ex)
        {
            var warningVm = new HighRiskWarningViewModel
            {
                UserId = vm.UserId,
                DisbursedAmount = vm.DisbursedAmount,
                AnnualInterestRate = vm.AnnualInterestRate,
                TermInMonths = vm.TermInMonths,
                WarningMessage = ex.Message,
                ClientFullName = vm.ClientFullName
            };
            return View("HighRiskWarning", warningVm);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View("AssignForm", vm);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmHighRisk(HighRiskWarningViewModel vm)
    {
        try
        {
            await _loanService.AssignLoanAsync(vm.UserId, vm.DisbursedAmount, vm.AnnualInterestRate, vm.TermInMonths, true);
            TempData["SuccessMessage"] = "Préstamo (Alto Riesgo) asignado exitosamente y desembolsado en la cuenta del cliente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(SelectClient));
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditRate(int id)
    {
        var loan = await _loanService.GetLoanDetailAsync(id);

        if (loan == null || loan.Status != LoanStatus.Active)
        {
            TempData["ErrorMessage"] = "Préstamo no encontrado o no está activo.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new EditLoanRateViewModel
        {
            LoanId = loan.Id,
            LoanNumber = loan.LoanNumber,
            CurrentRate = loan.AnnualInterestRate
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EditRate(EditLoanRateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _loanService.EditLoanRateAsync(vm.LoanId, vm.NewAnnualInterestRate);
            TempData["SuccessMessage"] = "La tasa de interés ha sido actualizada y la tabla de amortización se ha recalculado.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(vm);
        }
    }
}
