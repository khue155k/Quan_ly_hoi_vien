using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Data;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;
    public LoginController(AppDbContext context)
        {
            _context = context;
        }
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginUser){
        var passEncode = GenerateSHA256(loginUser.password);
        
        var user = (from u in _context.User 
                    where u.email==loginUser.email && u.password==passEncode
                    select u).FirstOrDefault();

        if (user==null){
            return Unauthorized(new{ message = "Tài khoản hoặc mật khẩu không chính xác!"});
        }

        return Ok(new {
            message = "Login complete!",
            email = user.email,
            isAdmin = user.admin == "true",
            name = user.name,
            id = user.id
        });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request){
        var user = await _context.User.FirstOrDefaultAsync(u => u.email == request.email);

        if (user==null){
            return NotFound(new{ message = "Không tồn tại tên người dùng"});
        }

        var oldPassEncode = GenerateSHA256(request.oldPassword);
        if (user.password != oldPassEncode){
            return Unauthorized (new {message = "Mật khẩu cũ không chính xác"});
        }

        var newPassEncode = GenerateSHA256(request.newPassword);
        user.password = newPassEncode;

        await _context.SaveChangesAsync();

        return Ok(new {
            message = "Đổi mật khẩu thành công!",
        });
    }

    public static string GenerateSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
