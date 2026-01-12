using AutoMapper;
using Blog.Models;
using LMS.Controllers.Common;
using LMS.Data;
using LMS.Model.Common;
using LMS.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

                if (existingUser != null)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Username or Email already exists",
                        Data = null
                    };
                    return BadRequest(response);
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newUser = new User
                {
                    Username = request.Username,
                    Fullname = request.Fullname,
                    Email = request.Email,
                    Password = hashedPassword,
                    IdNo = request.IdNo,
                    PhoneNo = request.PhoneNo,
                    CreatedBy = null, // Self-registration, no creator
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return CResponseRegisterSuccessful();

            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }
    }
}