namespace Models;
public class PayMembershipFeeRequestDTO
{
    public uint company_id { get; set; }
    public uint fee_id { get; set; }
    public string payment_method { get; set; }
    public byte status { get; set; }
}