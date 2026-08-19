using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Application.ViewModels.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.Users;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CreditCardController : Controller
    {
        private readonly ICreditCardService _creditCardService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreditCardController(
            ICreditCardService creditCardService,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _creditCardService = creditCardService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string? identification, CardStatus? status, int page = 1)
        {
            var filter = new CreditCardFilterDto
            {
                Identification = identification,
                Status = status,
                Page = page,
                PageSize = 20
            };

            var pagedResult = await _creditCardService.GetFilteredAsync(filter);
            var viewModel = new CreditCardListFilterViewModel
            {
                Identification = identification,
                Status = status,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalRecords = pagedResult.TotalRecords,
                Cards = _mapper.Map<List<CreditCardViewModel>>(pagedResult.Data)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Assign()
        {
            var allUsers = await _userRepository.GetAllAsync();
            ViewBag.ActiveClients = allUsers.Where(u => u.IsActive && u.Role == Roles.Client).ToList();
            return View(new AssignCreditCardViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignCreditCardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var allUsers = await _userRepository.GetAllAsync();
                ViewBag.ActiveClients = allUsers.Where(u => u.IsActive && u.Role == Roles.Client).ToList();
                return View(model);
            }

            var dto = _mapper.Map<CreateCreditCardDto>(model);
            var result = await _creditCardService.AssignCardAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                var allUsers = await _userRepository.GetAllAsync();
                ViewBag.ActiveClients = allUsers.Where(u => u.IsActive && u.Role == Roles.Client).ToList();
                return View(model);
            }

            TempData["SuccessMessage"] = "Tarjeta de crédito asignada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var detail = await _creditCardService.GetDetailAsync(id);
            if (detail == null)
                return NotFound();

            var viewModel = _mapper.Map<CreditCardDetailsViewModel>(detail);
            return View(viewModel);
        }

        public async Task<IActionResult> EditLimit(int id)
        {
            var detail = await _creditCardService.GetDetailAsync(id);
            if (detail == null)
                return NotFound();

            if (detail.Status == CardStatus.Cancelled)
            {
                TempData["ErrorMessage"] = "No se puede modificar una tarjeta cancelada.";
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateCreditLimitViewModel
            {
                Id = detail.Id,
                MaskedCardNumber = detail.MaskedCardNumber,
                ClientFullName = detail.ClientFullName,
                CurrentCreditLimit = detail.CreditLimit,
                CurrentDebt = detail.CurrentDebt,
                NewCreditLimit = detail.CreditLimit
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLimit(UpdateCreditLimitViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new UpdateCreditLimitRequestDto { CreditLimit = model.NewCreditLimit };
            var result = await _creditCardService.UpdateCreditLimitAsync(model.Id, dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = "Límite de crédito actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var detail = await _creditCardService.GetDetailAsync(id);
            if (detail == null)
                return NotFound();

            var model = new CancelCreditCardViewModel
            {
                Id = detail.Id,
                LastFourDigits = detail.LastFourDigits,
                ClientFullName = detail.ClientFullName,
                CurrentDebt = detail.CurrentDebt
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(CancelCreditCardViewModel model)
        {
            var result = await _creditCardService.CancelCardAsync(model.Id, model.Reason);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = "Tarjeta cancelada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}