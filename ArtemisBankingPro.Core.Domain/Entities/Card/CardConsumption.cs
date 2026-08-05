using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBankingPro.Core.Domain.Entities.Card
{
    public class CardConsumption
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int CommerceId { get; set; }
        public decimal Amount { get; set; }
        public ConsumptionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public int? ProcessingLogId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
