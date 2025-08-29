using System;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Domain.Entities
{
    public class Donation : IEntity<Guid>
    {
        public Guid Id { get; set; }

        [Required]
        public Guid DisasterId { get; set; }
        public Disaster Disaster { get; set; }

        [Required]
        public string DonorUserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string DonorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DonorEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string DonorPhone { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(50)]
        public string? TransactionId { get; set; }

        [MaxLength(20)]
        public string? PaymentStatus { get; set; }

        public DateTime DonationDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public enum PaymentMethod
    {
        bKash,
        Nagad,
        BankTransfer,
        CreditCard
    }
}