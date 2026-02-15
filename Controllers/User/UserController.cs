using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANTM.Controllers.Common;
using QUANTM.Data;
using QUANTM.DTOs.User;
using QUANTM.Model.Common;

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
        private readonly Services.Common.IDocumentService _documentService;
        private readonly Services.Auth.IdentityService _identityService;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, IMapper mapper, Services.Auth.AuditService auditService, Services.Auth.EncryptionService encryptionService, Services.Common.IDocumentService documentService, Services.Auth.IdentityService identityService, ILogger<UserController> logger)
        {
            _context = context;
            _mapper = mapper;
            _auditService = auditService;
            _encryptionService = encryptionService;
            _documentService = documentService;
            _identityService = identityService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetUsers([FromQuery] UserListParamsDto userListParamsDto)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.Role)
                    // .Include(u => u.Gender)
                    .Include(u => u.Status)
                    // .Include(u => u.Department)
                    .Include(u => u.Creator)
                    .Include(u => u.Updater)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(userListParamsDto.Fullname))
                {
                    query = query.Where(u => u.Fullname.Contains(userListParamsDto.Fullname));
                }
                if (!string.IsNullOrEmpty(userListParamsDto.Username))
                {
                    query = query.Where(u => u.Username.Contains(userListParamsDto.Username));
                }
                if (!string.IsNullOrEmpty(userListParamsDto.RoleCode))
                {
                    query = query.Where(u => u.Role != null && u.Role.Code == userListParamsDto.RoleCode);
                }
                var users = await query.Skip((userListParamsDto.PageNo - 1) * userListParamsDto.PageSize).Take(userListParamsDto.PageSize).ToListAsync();
                var userDtos = _mapper.Map<List<UserDetailsDto>>(users);

                foreach (var userDto in userDtos)
                {
                    userDto.IdNo = _encryptionService.Decrypt(userDto.IdNo);
                }

                return CResponseGetListSuccessful(userDtos, users.Count, userListParamsDto.PageNo, userListParamsDto.PageSize);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Gender)
                    .Include(u => u.Status)
                    .Include(u => u.Department)
                    .Include(u => u.Address).ThenInclude(a => a!.State)
                    .Include(u => u.Address).ThenInclude(a => a!.Country)
                    .Include(u => u.Creator)
                    .Include(u => u.Updater)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return CResponseNotFound();
                }
                var userDto = _mapper.Map<UserDetailsDto>(user);
                userDto.IdNo = _encryptionService.Decrypt(userDto.IdNo);
                return CResponseGetSuccessful(userDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Gender)
                    .Include(u => u.Status)
                    .Include(u => u.Department)
                    .Include(u => u.Address).ThenInclude(a => a!.State)
                    .Include(u => u.Address).ThenInclude(a => a!.Country)
                    .Include(u => u.Creator)
                    .Include(u => u.Updater)
                    .FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return CResponseNotFound();
                }
                var userDto = _mapper.Map<UserDetailsDto>(user);
                userDto.IdNo = _encryptionService.Decrypt(userDto.IdNo);
                return CResponseGetSuccessful(userDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Gender)
                    .Include(u => u.Status)
                    .Include(u => u.Department)
                    .Include(u => u.Address).ThenInclude(a => a!.State)
                    .Include(u => u.Address).ThenInclude(a => a!.Country)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return CResponseNotFound();
                }

                // Map standard fields (non-navigation, non-encrypted)
                _mapper.Map(userUpdateDto, user);

                // Handle encryption for IdNo if it was updated
                if (!string.IsNullOrEmpty(userUpdateDto.IdNo))
                {
                    user.IdNo = _encryptionService.Encrypt(userUpdateDto.IdNo);
                    user.IdNoHash = _encryptionService.Hash(userUpdateDto.IdNo);
                }

                // Handle Gender Code Update
                if (userUpdateDto.Gender != null)
                {
                    if (string.IsNullOrEmpty(userUpdateDto.Gender.Code))
                    {
                        user.GenderId = null;
                    }
                    else
                    {
                        var gender = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == userUpdateDto.Gender.Code);
                        if (gender != null) user.GenderId = gender.Id;
                    }
                }

                // Handle Role Code Update
                if (userUpdateDto.Role?.Code != null)
                {
                    var role = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == userUpdateDto.Role.Code);
                    if (role != null) user.RoleId = role.Id;
                }

                // Handle Status Code Update
                if (userUpdateDto.Status?.Code != null)
                {
                    var status = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == userUpdateDto.Status.Code);
                    if (status != null) user.StatusId = status.Id;
                }

                // Handle Department Code Update
                if (userUpdateDto.Department != null)
                {
                    if (string.IsNullOrEmpty(userUpdateDto.Department.Code))
                    {
                        user.DepartmentId = null;
                    }
                    else
                    {
                        var dept = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == userUpdateDto.Department.Code);
                        if (dept != null) user.DepartmentId = dept.Id;
                    }
                }

                // Handle Address Update
                if (userUpdateDto.Address != null)
                {
                    if (user.Address == null)
                    {
                        user.Address = new Models.Common.Address();
                        _context.Addresses.Add(user.Address);
                    }

                    _mapper.Map(userUpdateDto.Address, user.Address);

                    // Resolve Address State Code
                    if (userUpdateDto.Address.State != null)
                    {
                        if (string.IsNullOrEmpty(userUpdateDto.Address.State.Code))
                        {
                            user.Address.StateId = null;
                        }
                        else
                        {
                            var state = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == userUpdateDto.Address.State.Code);
                            if (state != null) user.Address.StateId = state.Id;
                        }
                    }

                    // Resolve Address Country Code
                    if (userUpdateDto.Address.Country != null)
                    {
                        if (string.IsNullOrEmpty(userUpdateDto.Address.Country.Code))
                        {
                            user.Address.CountryId = null;
                        }
                        else
                        {
                            var country = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == userUpdateDto.Address.Country.Code);
                            if (country != null) user.Address.CountryId = country.Id;
                        }
                    }
                }

                user.UpdatedBy = _identityService.GetUserId();
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Re-fetch the user to get a fresh copy with updated navigation properties for the response
                var updatedUser = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Gender)
                    .Include(u => u.Status)
                    .Include(u => u.Department)
                    .Include(u => u.Address).ThenInclude(a => a!.State)
                    .Include(u => u.Address).ThenInclude(a => a!.Country)
                    .Include(u => u.Creator)
                    .Include(u => u.Updater)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                var userDto = _mapper.Map<UserDetailsDto>(updatedUser);
                userDto.IdNo = _encryptionService.Decrypt(userDto.IdNo);

                return CResponseUpdateSuccessful(userDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}/photo")]
        public async Task<IActionResult> UpdateUserImage(int id, IFormFile file)
        {
            try
            {
                var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(currentUserIdStr) || !int.TryParse(currentUserIdStr, out int currentUserId))
                {
                    return CResponseUnauthorized("User not found");
                }

                // Security Check: Allow if updating own profile OR is Admin/Super Admin
                bool isSelfUpdate = currentUserId == id;
                bool isAdmin = currentUserRole == "SA" || currentUserRole == "ADM";

                if (!isSelfUpdate && !isAdmin)
                {
                    return StatusCode(403, new { status = 403, message = "You are not authorized to update this user's profile image." });
                }

                var user = await _context.Users
                    .Include(u => u.ProfileImage)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return CResponseNotFound("User not found");
                }

                // Validation
                if (file == null || file.Length == 0)
                {
                    return CResponseBadRequest("No file provided.");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return CResponseBadRequest("Only .jpg and .png files are allowed.");
                }

                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxFileSize)
                {
                    return CResponseBadRequest("File size cannot exceed 5MB.");
                }

                var document = await _documentService.UploadFileAsync(file, id, "User Profile Image");

                user.ProfileImageId = document.Id;
                await _context.SaveChangesAsync();

                return CResponseCreateSuccessful(new { ProfileImageId = document.Id, FilePath = document.FilePath });
            }
            catch (ArgumentException ex)
            {
                return CResponseBadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}/photo")]
        public async Task<IActionResult> DeleteUserImage(int id)
        {
            try
            {
                var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(currentUserIdStr) || !int.TryParse(currentUserIdStr, out int currentUserId))
                {
                    return CResponseUnauthorized("User not found");
                }

                // Security Check: Allow if deleting own profile OR is Admin/Super Admin
                bool isSelfUpdate = currentUserId == id;
                bool isAdmin = currentUserRole == "SA" || currentUserRole == "ADM";

                if (!isSelfUpdate && !isAdmin)
                {
                    return StatusCode(403, new { status = 403, message = "You are not authorized to delete this user's profile image." });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return CResponseNotFound("User not found");
                }

                if (user.ProfileImageId.HasValue)
                {
                    var documentId = user.ProfileImageId.Value;
                    user.ProfileImageId = null;
                    await _context.SaveChangesAsync();

                    // Optional: Clean up the document record and physical file
                    await _documentService.DeleteFileAsync(documentId);
                }

                return CResponseDeleteSuccessful();
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("mini-profile")]
        public async Task<IActionResult> MiniProfile()
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

                // get user id from token
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    return CResponseUnauthorized("User not found");
                }

                // Fetch live data from DB to get the latest profile image
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return CResponseUnauthorized("User record not found");
                }

                var myMiniProfile = new
                {
                    Fullname = user.Fullname,
                    Shortname = user.Shortname,
                    Username = user.Username,
                    ProfileImageUrl = user.ProfileImageId.HasValue
                        ? $"/api/documents/{user.ProfileImageId}/content"
                        : null
                };

                return CResponseGetSuccessful(myMiniProfile);
            }
            catch (Exception ex)
            {

                return CResponseException(ex.Message);
            }

        }

        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> checkUsernameAvailability([FromRoute] string? username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return CResponseGetSuccessful(new { available = false });
                }

                _logger.LogInformation("Checking username availability for {Username}", username);

                var exists = await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == username.ToLower());

                _logger.LogInformation("Username {Username} exists: {Exists}", username, exists);

                return CResponseGetSuccessful(new
                {
                    available = !exists
                });
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }
    }
}