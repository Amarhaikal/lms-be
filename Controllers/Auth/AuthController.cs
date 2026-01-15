using System.Text.Json;
using AutoMapper;
using LMS.Controllers.Common;
using LMS.Data;
using LMS.DTOs.Auth;
using LMS.DTOs.User;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, IMapper mapper, ILogger<AuthController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var existingUserName = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                var existingUserEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.IdNo == request.IdNo);

                if (existingUserName != null)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Username already exists",
                        Data = null
                    };
                    return BadRequest(response);
                }

                if (existingUserEmail != null)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Email already exists",
                        Data = null
                    };
                    return BadRequest(response);
                }

                if (existingUser != null)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "ID Number already exists",
                        Data = null
                    };
                    return BadRequest(response);
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var statusNewUser = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == "NEW");

                var newUser = new User
                {
                    Fullname = request.Fullname,
                    IdNo = request.IdNo,
                    Username = request.Username,
                    Email = request.Email,
                    Password = hashedPassword,
                    RoleId = request.RoleId,
                    StatusId = statusNewUser?.Id ?? 0,
                    CreatedBy = null,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var createdUser = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == newUser.Id);
                var mappedUser = _mapper.Map<UserDetailsDto>(createdUser);

                return CResponseRegisterSuccessful(mappedUser);

            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }
    }
}