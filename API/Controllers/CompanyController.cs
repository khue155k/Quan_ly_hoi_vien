using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Text.RegularExpressions;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompanyController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("company-list")]
    public async Task<IActionResult> GetCompanyList(int page = 1, int pageSize = 10)
    {
        var CompanyList = await _context.Companies
            .OrderByDescending(c => c.id)
            .Select(c => new CompanyDTO
            {
                id = c.id,
                ten_doanh_nghiep = c.ten_doanh_nghiep,
                mst = c.mst ?? "",
                diachi = c.diachi ?? "",
                status = c.status
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCompanies = await _context.Companies.CountAsync();

        var result = new
        {
            TotalCompanies = totalCompanies,
            Page = page,
            PageSize = pageSize,
            Items = CompanyList
        };

        return Ok(result);
    }

    [HttpGet("company-list-by-status")]
    public async Task<IActionResult> GetCompanyListByStatus(byte status, int page = 1, int pageSize = 10)
    {
        var CompanyList = await _context.Companies
            .Where(c => c.status == status)
            .OrderByDescending(c => c.id)
            .Select(c => new CompanyDTO
            {
                id = c.id,
                ten_doanh_nghiep = c.ten_doanh_nghiep,
                mst = c.mst ?? "",
                diachi = c.diachi ?? "",
                status = c.status
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCompanies = await _context.Companies.CountAsync(c => c.status == status);

        var result = new
        {
            TotalCompanies = totalCompanies,
            Page = page,
            PageSize = pageSize,
            Items = CompanyList
        };

        return Ok(result);
    }

    [HttpGet("company-list-by-name-status")]
    public async Task<IActionResult> GetCompanyListByNameStatus(string? name, byte? status = null, int page = 1, int pageSize = 10)
    {

        var query = _context.Companies.AsQueryable();

        if (name != "" && name != null)
        {
            query = query.Where(c => c.ten_doanh_nghiep.Contains(name));
        }

        if (status.HasValue && status != (byte)4)
        {
            query = query.Where(c => c.status == status.Value);
        }

        var CompanyList = await query
            .OrderByDescending(c => c.id)
            .Select(c => new CompanyDTO
            {
                id = c.id,
                ten_doanh_nghiep = c.ten_doanh_nghiep,
                mst = c.mst,
                diachi = c.diachi,
                status = c.status
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompanyDetail(uint id)
    {
        var company = await _context.Companies.FindAsync(id);
        // .Where(c => c.id == id)
        // .Select(c => new Company{
        //     id = c.id,
        //     ten_doanh_nghiep = c.ten_doanh_nghiep,
        //     mst = c.mst,
        //     diachi = c.diachi,
        //     status = c.status
        // })
        // .FirstOrDefaultAsync();

        if (company == null)
        {
            return NotFound(new { Message = "Company not found." });
        }
        return Ok(company);
    }

    [HttpPut("update-status/{id}")]
    public async Task<IActionResult> UpdateCompanyStatus(uint id, [FromBody] byte status)
    {
        Company company = await _context.Companies.FindAsync(id);

        if (company == null)
        {
            return NotFound(new { message = "Không tìm thấy doanh nghiệp!" });
        }

        company.status = status;
        company.updated_at = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Trạng thái doanh nghiệp đã được cập nhật" });
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateCompany([FromBody] Company company)
    {
        bool mstExists = await _context.Companies.AnyAsync(c => c.mst == company.mst);
        if (mstExists)
        {
            return Conflict(new { Message = "Mã số thuế đã tồn tại." });
        }
        if (company.ten_doanh_nghiep == null || company.ten_doanh_nghiep == "")
        {
            return BadRequest(new { Message = "Tên doanh nghiệp không hợp lệ." });
        }
        else if (company.mst == null || company.mst == "")
        {
            return BadRequest(new { Message = "Mã số thuế không hợp lệ." });
        }
        else if (company.diachi == null || company.diachi == "")
        {
            return BadRequest(new { Message = "Địa chỉ không hợp lệ." });
        }
        else if (company.so_dt == null || company.so_dt == "")
        {
            return BadRequest(new { Message = "Số điện thoại doanh nghiệp không hợp lệ." });
        }
        else if (company.zalo == null || company.zalo == "")
        {
            return BadRequest(new { Message = "Zalo doanh nghiệp không hợp lệ." });
        }
        else if (company.email == null || company.email == "" || !IsValidEmail(company.email))
        {
            return BadRequest(new { Message = "Email doanh nghiệp không hợp lệ." });
        }
        else if (company.hoten_ndd == null || company.hoten_ndd == "")
        {
            return BadRequest(new { Message = "Tên người đại diện không hợp lệ." });
        }
        else if (company.so_dt_ndd == null || company.so_dt_ndd == "")
        {
            return BadRequest(new { Message = "Số điện thoại người đại diện không hợp lệ." });
        }
        else if (company.email_ndd == null || company.email_ndd == "" || !IsValidEmail(company.email_ndd))
        {
            return BadRequest(new { Message = "Email người đại diện không hợp lệ." });
        }
        else if (company.hoten_nlh == null || company.hoten_nlh == "")
        {
            return BadRequest(new { Message = "Họ tên người liên hệ không hợp lệ." });
        }

        else if (company.so_dt_nlh == null || company.so_dt_nlh == "")
        {
            return BadRequest(new { Message = "Số điện thoại người liên hệ không hợp lệ." });
        }
        else if (company.email_nlh == null || company.email_nlh == "" || !IsValidEmail(company.email_nlh))
        {
            return BadRequest(new { Message = "Email người liên hệ không hợp lệ." });
        }


        company.status = (byte)1;
        company.dongy = (byte)0;
        company.created_at = DateTime.Now;
        company.updated_at = DateTime.Now;

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Đã gửi đăng ký thông tin hội viên.", CompanyId = company.id });
    }

    private bool IsValidEmail(string email)
    {
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }
}
