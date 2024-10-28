using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeeController : ControllerBase
{
    private readonly AppDbContext _context;

    public FeeController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFee(int id)
    {
        var fee = await _context.Fees.FindAsync(id);
        if (fee == null)
        {
            return NotFound();
        }
        return Ok(fee);
    }


    [HttpGet]
    public async Task<IActionResult> GetFeeList()
    {
        var FeeList = await _context.Fees
            .OrderByDescending(c => c.year)
            .ToListAsync();     

        var result = new
        {
            Items = FeeList
        };

        return Ok(result);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateFee([FromBody] Fee newFee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Fees.Add(newFee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFee), new { id = newFee.id }, newFee);
    }
}
