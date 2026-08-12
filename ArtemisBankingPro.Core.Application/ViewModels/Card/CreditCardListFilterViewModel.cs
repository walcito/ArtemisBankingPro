using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Card
{
    public class CreditCardListFilterViewModel
    {
        [Display(Name = "Filtrar por Estado")]
        public CardStatus? Status { get; set; }

        [MaxLength(11, ErrorMessage = "La cédula no puede exceder los 11 dígitos.")]
        [Display(Name = "Buscar por Cédula")]
        public string? Identification { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalRecords { get; set; }
        public int TotalPages => (TotalRecords + PageSize - 1) / PageSize;
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
        public List<CreditCardViewModel> Cards { get; set; } = new();
    }
}

