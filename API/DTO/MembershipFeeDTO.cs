namespace Models;
public class MembershipFeeDTO
{
    public uint id { get; set; }
    public uint company_id { get; set; }
    public uint fee_id { get; set; }
    public byte status { get; set; }
    public DateTime? payment_date { get; set; }
    public string? payment_method { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public uint year { get; set; }
    public decimal fee_amount { get; set; }
    public string ten_doanh_nghiep { get; set; }
    public string mst { get; set; }
    public string diachi { get; set; }

}