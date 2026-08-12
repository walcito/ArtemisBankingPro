using ArtemisBankingPro.Core.Domain.Common.Enums.Card;


namespace ArtemisBankingPro.Core.Application.Dtos.Card
{
    public class CardConsumptionDto
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string MaskedCardNumber { get; set; } = string.Empty;
        public int CommerceId { get; set; }
        public string CommerceName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public ConsumptionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved => Status == ConsumptionStatus.Approved;
    }
    public class CardConsumptionFilterDto
    {
        public int? CardId { get; set; }
        public int? CommerceId { get; set; }
        public ConsumptionStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class CardConsumptionPagedResultDto
    {
        public List<CardConsumptionDto> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public decimal TotalApprovedAmount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
    }
}
