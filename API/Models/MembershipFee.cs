namespace Models;
public class MembershipFee
{
    public uint id { get; set; }
    public uint company_id { get; set; }
    public uint fee_id { get; set; }
    public byte status { get; set; }
    public DateTime? payment_date { get; set; }
    public string? payment_method { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public Fee Fee { get; set; }
    public Company Company { get; set; }
}


