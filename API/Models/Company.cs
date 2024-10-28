namespace Models;

public class Company
{
    public uint id { get; set; }
    public string ten_doanh_nghiep { get; set; }
    public string mst { get; set; }
    public string diachi { get; set; }
    public string so_dt { get; set; }
    public string? zalo { get; set; }
    public string? email { get; set; }
    public string? website { get; set; }
    public string? hoten_ndd { get; set; }
    public string? chucvu_ndd { get; set; }
    public DateOnly? ngay_sinh_ndd { get; set; }
    public string? so_dt_ndd { get; set; }
    public string? zalo_ndd { get; set; }
    public string? email_ndd { get; set; }
    public string? hoten_nlh { get; set; }
    public string? chucvu_nlh { get; set; }
    public DateOnly? ngay_sinh_nlh { get; set; }
    public string? so_dt_nlh { get; set; }
    public string? zalo_nlh { get; set; }
    public string? email_nlh { get; set; }
    public byte? dongy { get; set; }
    public byte status { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}
