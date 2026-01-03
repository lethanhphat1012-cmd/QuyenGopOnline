public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; } // Ai quyên góp
    public int PostId { get; set; } // Quyên góp cho hoàn cảnh nào
    public decimal Amount { get; set; } // Số tiền
    public DateTime DonationDate { get; set; } // Ngày quyên góp
    public string Note { get; set; } // Lời nhắn

    // Liên kết (Navigation Properties)
    public virtual User User { get; set; }
}