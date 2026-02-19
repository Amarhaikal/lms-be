using System.Text.Json;
using AutoMapper;
using QUANTM.Controllers.Common;
using QUANTM.Data;
using QUANTM.DTOs.Parameter;
using QUANTM.Model.Common;
using QUANTM.Models.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANTM.Services.Auth;

namespace QUANTM.Controllers.Parameter
{
    [Authorize]
    [Route("api/parameter")]
    [ApiController]
    public class ParameterController : BaseApiController
    {
        private readonly ILogger<ParameterController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IdentityService _identityService;

        public ParameterController(ILogger<ParameterController> logger, ApplicationDbContext context, IMapper mapper, IdentityService identityService)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
        }

        [HttpGet("codeType")]
        public async Task<IActionResult> GetCodeTypeAndSystemCode()
        {
            try
            {
                var codeType = await _context.CodeTypes
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.SystemCodes)
                        .ThenInclude(sc => sc.CreatedByUser)
                    .Include(x => x.SystemCodes)
                        .ThenInclude(sc => sc.UpdatedByUser)
                    .ToListAsync();
                var codeTypeDto = _mapper.Map<List<CodeTypeDto>>(codeType);
                return CResponseGetListSuccessful(codeTypeDto);

            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [HttpGet("codeType/{id}")]
        public async Task<IActionResult> GetCodeType(int id)
        {
            try
            {
                var codeType = await _context.CodeTypes
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.SystemCodes)
                        .ThenInclude(sc => sc.CreatedByUser)
                    .Include(x => x.SystemCodes)
                        .ThenInclude(sc => sc.UpdatedByUser)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (codeType == null)
                {
                    return CResponseNotFound();
                }

                var codeTypeDto = _mapper.Map<CodeTypeDto>(codeType);
                return CResponseGetSuccessful(codeTypeDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpPost("codeType")]
        public async Task<IActionResult> CreateCodeType([FromBody] CodeTypeCreateDto body)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return CResponseInvalidDataToSave();
                }

                var isCodeTypeExist = await _context.CodeTypes.AnyAsync(x => x.Code == body.Code);
                if (isCodeTypeExist)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Code type already exists"
                    };
                    return BadRequest(response);
                }

                var codeType = _mapper.Map<CodeType>(body);
                _context.CodeTypes.Add(codeType);
                await _context.SaveChangesAsync();

                var codeTypeDto = _mapper.Map<CodeTypeDto>(codeType);
                return CResponseCreateSuccessful(codeTypeDto);

            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpPut("codeType/{id}")]
        public async Task<IActionResult> UpdateCodeType(int id, [FromBody] CodeTypeUpdateDto body)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CResponseInvalidDataToSave();
                }

                var codeType = await _context.CodeTypes.FindAsync(id);
                if (codeType == null)
                {
                    return CResponseNotFound();
                }

                var isCodeExisting = await _context.CodeTypes.AnyAsync(x => x.Code == body.Code);
                if (isCodeExisting)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Code type already exists"
                    };
                    return BadRequest(response);
                }

                codeType.Code = body.Code ?? codeType.Code;
                codeType.Description = body.Description ?? codeType.Description;
                await _context.SaveChangesAsync();

                var codeTypeDto = _mapper.Map<CodeTypeDto>(codeType);
                return CResponseUpdateSuccessful(codeTypeDto);

            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpDelete("codeType/{id}")]
        public async Task<IActionResult> DeleteCodeType(int id)
        {
            try
            {
                var codeType = await _context.CodeTypes.FindAsync(id);
                if (codeType == null)
                {
                    return CResponseNotFound();
                }

                _context.CodeTypes.Remove(codeType);
                await _context.SaveChangesAsync();

                return CResponseDeleteSuccessful();
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [HttpGet("systemCode")]
        public async Task<IActionResult> GetSystemCode([FromQuery] SystemCodeListParamsDto systemCodeListParamsDto)
        {
            try
            {
                var query = _context.SystemCodes.AsQueryable();

                if (!string.IsNullOrEmpty(systemCodeListParamsDto.CodeTypeCode))
                {
                    query = query.Where(x => x.CodeType != null && x.CodeType.Code == systemCodeListParamsDto.CodeTypeCode);
                }

                if (!string.IsNullOrEmpty(systemCodeListParamsDto.Code))
                {
                    query = query.Where(x => x.Code != null && x.Code.Contains(systemCodeListParamsDto.Code));
                }

                if (!string.IsNullOrEmpty(systemCodeListParamsDto.Description))
                {
                    query = query.Where(x => x.Description != null && x.Description.Contains(systemCodeListParamsDto.Description));
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var systemCodes = await query
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.UpdatedByUser)
                    .Include(x => x.CodeType)
                    .Skip((systemCodeListParamsDto.PageNo - 1) * systemCodeListParamsDto.PageSize)
                    .Take(systemCodeListParamsDto.PageSize)
                    .ToListAsync();

                var systemCodeDto = _mapper.Map<List<SystemCodeDto>>(systemCodes);

                return CResponseGetListSuccessful(systemCodeDto, totalCount, systemCodeListParamsDto.PageNo, systemCodeListParamsDto.PageSize);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpPost("systemCode")]
        public async Task<IActionResult> CreateSystemCode([FromBody] SystemCodeCreateDto body)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CResponseInvalidDataToSave();
                }

                var codeType = await _context.CodeTypes.FirstOrDefaultAsync(x => x.Code == body.CodeTypeCode);
                if (codeType == null)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Code type does not exist"
                    };
                    return BadRequest(response);
                }

                var isSystemCodeInSameCodeTypeExist = await _context.SystemCodes.AnyAsync(x => x.Code == body.Code && x.CodeType != null && x.CodeType.Code == body.CodeTypeCode);
                if (isSystemCodeInSameCodeTypeExist)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "System code already exists"
                    };
                    return BadRequest(response);
                }

                var systemCode = _mapper.Map<SystemCode>(body);
                systemCode.CodeTypeId = codeType?.Id ?? 0;
                systemCode.CreatedAt = DateTime.UtcNow;
                systemCode.CreatedBy = _identityService.GetUserId();
                _context.SystemCodes.Add(systemCode);
                await _context.SaveChangesAsync();

                var systemCodeDto = _mapper.Map<SystemCodeDto>(systemCode);
                return CResponseCreateSuccessful(systemCodeDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpPost("systemCodes")]
        public async Task<IActionResult> CreateSystemCodes([FromBody] List<SystemCodeCreateDto> body)
        {
            try
            {
                if (body == null || !body.Any() || !ModelState.IsValid)
                {
                    return CResponseInvalidDataToSave();
                }

                // Get unique code types from body
                var requestedCodeTypes = body.Select(b => b.CodeTypeCode).Distinct().ToList();

                // Fetch all matching code types from DB
                var codeTypes = await _context.CodeTypes
                    .Where(ct => requestedCodeTypes.Contains(ct.Code))
                    .ToListAsync();

                // Validate all code types exist
                if (codeTypes.Count != requestedCodeTypes.Count)
                {
                    var missingTypes = requestedCodeTypes.Except(codeTypes.Select(ct => ct.Code)).ToList();
                    return CResponseException($"The following code types do not exist: {string.Join(", ", missingTypes)}");
                }

                var codeTypeMap = codeTypes.ToDictionary(ct => ct.Code, ct => ct.Id);

                // Check for duplicates in the database
                foreach (var item in body)
                {
                    var isExist = await _context.SystemCodes.AnyAsync(x =>
                        x.Code == item.Code &&
                        x.CodeTypeId == codeTypeMap[item.CodeTypeCode]);

                    if (isExist)
                    {
                        return CResponseException($"System code '{item.Code}' already exists for code type '{item.CodeTypeCode}'");
                    }
                }

                var systemCodes = new List<SystemCode>();
                var currentUserId = _identityService.GetUserId();
                var now = DateTime.UtcNow;

                foreach (var item in body)
                {
                    var systemCode = _mapper.Map<SystemCode>(item);
                    systemCode.CodeTypeId = codeTypeMap[item.CodeTypeCode];
                    systemCode.CreatedAt = now;
                    systemCode.CreatedBy = currentUserId;
                    systemCodes.Add(systemCode);
                }

                _context.SystemCodes.AddRange(systemCodes);
                await _context.SaveChangesAsync();

                var systemCodesDto = _mapper.Map<List<SystemCodeDto>>(systemCodes);
                return CResponseCreateSuccessful(systemCodesDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpPut("systemCode/{id}")]
        public async Task<IActionResult> UpdateSystemCode(int id, [FromBody] SystemCodeUpdateDto body)
        {
            try
            {
                if (body == null || !ModelState.IsValid)
                {
                    return CResponseInvalidDataToSave();
                }

                var systemCode = await _context.SystemCodes.FindAsync(id);
                if (systemCode == null)
                {
                    return CResponseNotFound();
                }

                // If CodeTypeCode is provided, look up the CodeType and update CodeTypeId
                if (!string.IsNullOrEmpty(body.CodeTypeCode))
                {
                    var codeType = await _context.CodeTypes.FirstOrDefaultAsync(x => x.Code == body.CodeTypeCode);
                    if (codeType == null)
                    {
                        var response = new ApiResponse<string>
                        {
                            Status = 400,
                            Message = "Code type does not exist"
                        };
                        return BadRequest(response);
                    }
                    systemCode.CodeTypeId = codeType.Id;
                }

                systemCode.Code = body.Code ?? systemCode.Code;
                systemCode.Description = body.Description ?? systemCode.Description;

                var isSystemCodeInSameCodeTypeExist = await _context.SystemCodes.AnyAsync(x => x.Id != id && x.Code == systemCode.Code && x.CodeTypeId == systemCode.CodeTypeId);
                if (isSystemCodeInSameCodeTypeExist)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "System code already exists"
                    };
                    return BadRequest(response);
                }

                systemCode.UpdatedAt = DateTime.UtcNow;
                systemCode.UpdatedBy = _identityService.GetUserId();

                await _context.SaveChangesAsync();

                var systemCodeDto = _mapper.Map<SystemCodeDto>(systemCode);
                return CResponseUpdateSuccessful(systemCodeDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpPut("systemCodes")]
        public async Task<IActionResult> UpdateSystemCodes([FromBody] List<SystemCodeBatchUpdateDto> body)
        {
            try
            {
                if (body == null || !body.Any() || !ModelState.IsValid)
                {
                    return CResponseInvalidDataToSave();
                }

                var inputIds = body.Select(x => x.Id).ToList();
                if (inputIds.Count != inputIds.Distinct().Count())
                {
                    return CResponseException("Duplicate IDs in input list");
                }

                // Fetch existing entities
                var systemCodes = await _context.SystemCodes.Where(x => inputIds.Contains(x.Id)).ToListAsync();
                if (systemCodes.Count != inputIds.Count)
                {
                    var foundIds = systemCodes.Select(x => x.Id).ToList();
                    var missingIds = inputIds.Except(foundIds);
                    return CResponseException($"System codes not found: {string.Join(", ", missingIds)}");
                }

                // Handle CodeTypeCode lookups if any are provided
                var requestedCodeTypes = body
                    .Where(b => !string.IsNullOrEmpty(b.CodeTypeCode))
                    .Select(b => b.CodeTypeCode!)
                    .Distinct()
                    .ToList();

                Dictionary<string, int> codeTypeMap = new();
                if (requestedCodeTypes.Any())
                {
                    var codeTypes = await _context.CodeTypes
                        .Where(ct => requestedCodeTypes.Contains(ct.Code))
                        .ToListAsync();

                    if (codeTypes.Count != requestedCodeTypes.Count)
                    {
                        var missingTypes = requestedCodeTypes.Except(codeTypes.Select(ct => ct.Code)).ToList();
                        return CResponseException($"The following code types do not exist: {string.Join(", ", missingTypes)}");
                    }
                    codeTypeMap = codeTypes.ToDictionary(ct => ct.Code, ct => ct.Id);
                }

                var currentUserId = _identityService.GetUserId();
                var now = DateTime.UtcNow;

                // Validate duplicates in batch and DB
                foreach (var dto in body)
                {
                    var entity = systemCodes.First(x => x.Id == dto.Id);

                    var newCode = dto.Code ?? entity.Code;
                    var newCodeTypeId = !string.IsNullOrEmpty(dto.CodeTypeCode)
                        ? codeTypeMap[dto.CodeTypeCode]
                        : entity.CodeTypeId;

                    // Check for duplicates in DB (excluding current ID)
                    var isExistInDb = await _context.SystemCodes.AnyAsync(x =>
                        x.Id != dto.Id &&
                        x.Code == newCode &&
                        x.CodeTypeId == newCodeTypeId);

                    if (isExistInDb)
                    {
                        return CResponseException($"System code '{newCode}' already exists for the target code type");
                    }

                    // Check for duplicates within the current batch
                    var isDuplicateInBatch = body.Any(b =>
                        b.Id != dto.Id &&
                        (b.Code ?? systemCodes.First(s => s.Id == b.Id).Code) == newCode &&
                        (!string.IsNullOrEmpty(b.CodeTypeCode) ? codeTypeMap[b.CodeTypeCode] : systemCodes.First(s => s.Id == b.Id).CodeTypeId) == newCodeTypeId);

                    if (isDuplicateInBatch)
                    {
                        return CResponseException($"Duplicate system code '{newCode}' found within the request batch");
                    }

                    // Apply updates
                    entity.Code = dto.Code ?? entity.Code;
                    entity.Description = dto.Description ?? entity.Description;
                    entity.CodeTypeId = newCodeTypeId;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = currentUserId;
                }

                await _context.SaveChangesAsync();

                var systemCodesDto = _mapper.Map<List<SystemCodeDto>>(systemCodes);
                return CResponseUpdateSuccessful(systemCodesDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpDelete("systemCode/{id}")]
        public async Task<IActionResult> DeleteSystemCode(int id)
        {
            try
            {
                var systemCode = await _context.SystemCodes.FindAsync(id);
                if (systemCode == null)
                {
                    return CResponseNotFound();
                }

                _context.SystemCodes.Remove(systemCode);
                await _context.SaveChangesAsync();

                return CResponseDeleteSuccessful();
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [Authorize(Roles = "ADM,SA")]
        [HttpPost("systemCodes/delete")]
        public async Task<IActionResult> DeleteSystemCodes([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return CResponseInvalidDataToSave();
                }

                var systemCodes = await _context.SystemCodes.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (systemCodes.Count != ids.Distinct().Count())
                {
                    var foundIds = systemCodes.Select(x => x.Id).ToList();
                    var missingIds = ids.Except(foundIds);
                    return CResponseException($"System codes not found: {string.Join(", ", missingIds)}");
                }

                _context.SystemCodes.RemoveRange(systemCodes);
                await _context.SaveChangesAsync();

                return CResponseDeleteSuccessful();
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

    }
}