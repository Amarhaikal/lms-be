using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUANTM.Controllers.Common;
using QUANTM.Data;

namespace QUANTM.Controllers.User
{
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Services.Auth.AuditService _auditService;
        private readonly Services.Auth.EncryptionService _encryptionService;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, IMapper mapper, Services.Auth.AuditService auditService, Services.Auth.EncryptionService encryptionService, ILogger<UserController> logger)
        {
            _context = context;
            _mapper = mapper;
            _auditService = auditService;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("my-username")]
        public async Task<IActionResult> GetMyNameAndUsername()
        {
            try
            {
                // Log authentication status
                _logger.LogInformation("User.Identity.IsAuthenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);
                _logger.LogInformation("User.Claims count: {ClaimsCount}", User.Claims.Count());

                // Log all claims
                foreach (var claim in User.Claims)
                {
                    _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                }

                // get user id from token - use ClaimTypes (ASP.NET transforms claim types)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("User ID from NameIdentifier claim: {UserId}", userIdClaim);
                if (userIdClaim == null)
                {
                    return CResponseUnauthorized("User not found");
                }

                // get username and name from token
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var name = User.FindFirst(ClaimTypes.GivenName)?.Value;

                var myNameAndUsername = new
                {
                    Name = name,
                    Username = username,
                };

                return CResponseGetSuccessful(myNameAndUsername);
            }
            catch (Exception ex)
            {

                return CResponseException(ex.Message);
            }

        }
    }
}