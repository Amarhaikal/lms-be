using System.Text.Json;
using AutoMapper;
using LMS.Controllers.Common;
using LMS.Data;
using LMS.DTOs.Parameter;
using LMS.Model.Common;
using LMS.Models.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers.Parameter
{
    [Authorize]
    [Route("api/parameter")]
    [ApiController]
    public class ParameterController : BaseApiController
    {
        private readonly ILogger<ParameterController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ParameterController(ILogger<ParameterController> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("codeType")]
        public async Task<IActionResult> GetCodeTypeAndSystemCode()
        {
            try
            {
                var codeType = await _context.CodeTypes.Include(x => x.SystemCodes).ToListAsync();
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
                var codeType = await _context.SystemCodes.FindAsync(id);

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
        public async Task<IActionResult> GetSystemCode(int codeTypeId)
        {
            try
            {
                if (codeTypeId == 0)
                {
                    var systemCode = await _context.SystemCodes.ToListAsync();
                    var systemCodeDto = _mapper.Map<List<SystemCodeDto>>(systemCode);
                    return CResponseGetListSuccessful(systemCodeDto);
                }
                else
                {
                    var systemCode = await _context.SystemCodes.Where(x => x.CodeTypeId == codeTypeId).ToListAsync();
                    _logger.LogInformation("SystemCode: {SystemCode}", JsonSerializer.Serialize(systemCode));
                    if (systemCode.Count == 0)
                    {
                        return CResponseNotFound();
                    }
                    var systemCodeDto = _mapper.Map<List<SystemCodeDto>>(systemCode);
                    return CResponseGetListSuccessful(systemCodeDto);
                }
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

                var isSystemCodeExist = await _context.SystemCodes.AnyAsync(x => x.Code == body.Code);
                if (isSystemCodeExist)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "System code already exists"
                    };
                    return BadRequest(response);
                }

                var systemCode = _mapper.Map<SystemCode>(body);
                systemCode.CodeTypeId = body.CodeTypeId;
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
        [HttpPut("systemCode/{id}")]
        public async Task<IActionResult> UpdateSystemCode(int id, [FromBody] SystemCodeUpdateDto body)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CResponseInvalidDataToSave();
                }

                var systemCode = await _context.SystemCodes.FindAsync(id);
                if (systemCode == null)
                {
                    return CResponseNotFound();
                }

                // Validate that CodeTypeId exists if it's being changed
                if (body.CodeTypeId != 0 && body.CodeTypeId != systemCode.CodeTypeId)
                {
                    var codeTypeExists = await _context.CodeTypes.AnyAsync(x => x.Id == body.CodeTypeId);
                    if (!codeTypeExists)
                    {
                        var response = new ApiResponse<string>
                        {
                            Status = 400,
                            Message = "Code type does not exist"
                        };
                        return BadRequest(response);
                    }
                }

                // log systemCode using logger
                _logger.LogInformation("Old SystemCode: {SystemCode}", JsonSerializer.Serialize(systemCode));

                systemCode.Code = body.Code ?? systemCode.Code;
                systemCode.Description = body.Description ?? systemCode.Description;

                // Only update CodeTypeId if a valid one is provided
                if (body.CodeTypeId != 0)
                {
                    systemCode.CodeTypeId = body.CodeTypeId;
                }

                // log systemCode using logger
                _logger.LogInformation("New SystemCode: {SystemCode}", JsonSerializer.Serialize(systemCode));

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


    }
}