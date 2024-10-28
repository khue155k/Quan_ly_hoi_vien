using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembershipFeeController : ControllerBase
{
    private readonly AppDbContext _context;

    public MembershipFeeController(AppDbContext context)
    {
        _context = context;
    }
    // API lấy ds công ty đã đóng phí theo năm và tên
    [HttpGet("company-list-by-name-year")]
    public async Task<IActionResult> GetCompanyListByName_Year(string? name = null, int? year = null, int page = 1, int pageSize = 10)
    {
        var query = _context.MembershipFees.Include(mf => mf.Fee).Include(mf => mf.Company).AsQueryable();

        if (name != "" && name != null)
        {
            query = query.Where(mf => mf.Company.ten_doanh_nghiep.Contains(name));
        }

        if (year != null)
        {
            query = query.Where(mf => mf.Fee.year == year);
        }

        var CompanyList = await query
            .Where(mf => mf.Company.status == 2 || mf.Company.status == 3)
            .OrderByDescending(mf => mf.Company.id)
            .Select(mf => new MembershipFeeDTO
            {
                id = mf.id,
                company_id = mf.company_id,
                fee_id = mf.fee_id,
                status = 1,
                payment_date = mf.payment_date,
                payment_method = mf.payment_method,
                created_at = mf.created_at,
                updated_at = mf.updated_at,
                year = mf.Fee.year,
                fee_amount = mf.Fee.fee_amount,
                ten_doanh_nghiep = mf.Company.ten_doanh_nghiep,
                mst = mf.Company.mst,
                diachi = mf.Company.diachi
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCompanies = await query.CountAsync();

        var result = new
        {
            TotalCompanies = totalCompanies,
            Page = page,
            PageSize = pageSize,
            Items = CompanyList
        };

        return Ok(result);
    }

    [HttpGet("company-list-by-name-year-notpay")]
    public async Task<IActionResult> GetCompanyListByName_YearNotPay(string? name = null, int? year = null, int page = 1, int pageSize = 10)
    {
        var query = _context.MembershipFees.Include(mf => mf.Fee).AsQueryable();

        if (year != null)
        {
            query = query.Where(mf => mf.Fee.year == year);
        }

        var queryCompaniesNotPay = _context.Companies
            .Where(company => !query.Any(mf => mf.company_id == company.id));

        if (name != "" && name != null)
        {
            queryCompaniesNotPay = queryCompaniesNotPay.Where(c => c.ten_doanh_nghiep.Contains(name));
        }

        var CompanyList = await queryCompaniesNotPay
            .Where(c => c.status == 2 || c.status == 3)
            .OrderByDescending(c => c.id)
            .Select(c => new CompanyDTO
            {
                id = c.id,
                ten_doanh_nghiep = c.ten_doanh_nghiep,
                mst = c.mst,
                diachi = c.diachi,
                status = 0
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCompanies = await queryCompaniesNotPay.CountAsync();

        var result = new
        {
            TotalCompanies = totalCompanies,
            Page = page,
            PageSize = pageSize,
            Items = CompanyList
        };

        return Ok(result);
    }

    [HttpPost("pay")]
    public async Task<IActionResult> payMembershipFee([FromBody] PayMembershipFeeRequestDTO request)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newMembershipFee = new MembershipFee
        {
            company_id = request.company_id,
            fee_id = request.fee_id,
            status = request.status,
            payment_date = DateTime.Now,
            payment_method = request.payment_method,
            created_at = DateTime.Now,
            updated_at = DateTime.Now
        };

        _context.MembershipFees.Add(newMembershipFee);
        await _context.SaveChangesAsync();

        return Ok(newMembershipFee);
    }

    [HttpGet("paid-count")]
    public async Task<IActionResult> GetPaidCompaniesCount(uint year)
    {
        var fee = await _context.Fees.FirstOrDefaultAsync(f => f.year == year);
        if (fee == null)
        {
            return NotFound("Không tìm thấy thông tin hội phí cho năm này.");
        }

        var paidCount = await _context.MembershipFees
            .Where(mf => mf.fee_id == fee.id)
            .CountAsync();

        var paidCompanyIds = await _context.MembershipFees
            .Where(mf => mf.fee_id == fee.id)
            .Select(mf => mf.company_id)
            .ToListAsync();

        var unPaidCount = await _context.Companies
            .Where(c => !paidCompanyIds.Contains(c.id) && (c.status == 2 || c.status == 3))
            .CountAsync();

        return Ok(new
        {
            Year = year,
            PaidCount = paidCount,
            UnpaidCount = unPaidCount
        });
    }

}
