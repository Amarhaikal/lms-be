using AutoMapper;
using QUANTM.Controllers.Common;
using QUANTM.Data;
using QUANTM.DTOs.Rate;
using QUANTM.Model.Common;
using QUANTM.Models.Rate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANTM.Services.Auth;

namespace QUANTM.Controllers.Rate
{
    [Authorize]
    [Route("api/rate")]
    [ApiController]
    public class RateController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IdentityService _identityService;
        private readonly AuditService _auditService;
        private readonly ILogger<RateController> _logger;

        public RateController(
            ApplicationDbContext context,
            IMapper mapper,
            IdentityService identityService,
            AuditService auditService,
            ILogger<RateController> logger)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetRates(
            [FromQuery(Name = "page_no")] int pageNo = 1,
            [FromQuery(Name = "page_size")] int pageSize = 10,
            [FromQuery(Name = "code")] string? code = null,
            [FromQuery(Name = "description")] string? description = null,
            [FromQuery(Name = "rate")] decimal? rate = null,
            [FromQuery(Name = "rate_type_code")] string? rateTypeCode = null,
            [FromQuery(Name = "sort_by")] string? sortBy = "created_at",
            [FromQuery(Name = "sort_order")] string? sortOrder = "desc")
        {
            try
            {
                var query = _context.Rates.AsQueryable();

                if (!string.IsNullOrEmpty(code))
                {
                    query = query.Where(x => x.Code != null && x.Code.Contains(code));
                }

                if (!string.IsNullOrEmpty(description))
                {
                    query = query.Where(x => x.Description != null && x.Description.Contains(description));
                }

                if (rate.HasValue)
                {
                    query = query.Where(x => x.RateValue == rate.Value);
                }

                if (!string.IsNullOrEmpty(rateTypeCode))
                {
                    query = query.Where(x => x.RateType != null && x.RateType.Code == rateTypeCode);
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    query = sortBy.ToLower() switch
                    {
                        "code" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(s => s.Code) : query.OrderBy(s => s.Code),
                        "description" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(s => s.Description) : query.OrderBy(s => s.Description),
                        "rate" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(s => s.RateValue) : query.OrderBy(s => s.RateValue),
                        "rate_type" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(s => s.RateType!.Description) : query.OrderBy(s => s.RateType!.Description),
                        "created_by" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(s => s.CreatedBy) : query.OrderBy(s => s.CreatedBy),
                        "updated_by" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(s => s.UpdatedBy) : query.OrderBy(s => s.UpdatedBy),
                        "created_at" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
                        "updated_at" => sortOrder?.ToLower() == "desc" ? query.OrderByDescending(s => s.UpdatedAt) : query.OrderBy(s => s.UpdatedAt),
                        _ => query.OrderByDescending(s => s.CreatedAt)
                    };
                }

                var totalCount = await query.CountAsync();

                var rates = await query
                    .Include(x => x.Creator)
                    .Include(x => x.Updater)
                    .Include(x => x.RateType)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var rateDtos = _mapper.Map<List<RateDto>>(rates);

                return CResponseGetListSuccessful(rateDtos, totalCount, pageNo, pageSize);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }



        [Authorize(Roles = "SA,ADM")]
        [HttpPost]
        public async Task<IActionResult> CreateRates([FromBody] List<RateCreateDto> body)
        {
            try
            {
                if (body == null || !body.Any() || !ModelState.IsValid)
                {
                    return CResponseInvalidDataToSave();
                }

                var requestedRateTypes = body
                    .Where(b => b.RateType != null && !string.IsNullOrEmpty(b.RateType.Code))
                    .Select(b => b.RateType.Code!)
                    .Distinct()
                    .ToList();

                _logger.LogInformation("mylogs Requested Rate Types: {Requested}", string.Join(", ", requestedRateTypes));

                var rateTypes = await _context.SystemCodes
                    .Include(sc => sc.CodeType)
                    .Where(sc => requestedRateTypes.Contains(sc.Code) && sc.CodeType!.Code == "RATE_TYPE")
                    .ToListAsync();

                _logger.LogInformation("mylogs Found Rate Types from DB: {Found}", string.Join(", ", rateTypes.Select(rt => $"{rt.Code}({rt.Id})")));

                var foundCodes = rateTypes.Select(ct => ct.Code).Distinct().ToList();
                var missingTypes = requestedRateTypes.Where(r => !foundCodes.Contains(r)).ToList();

                if (missingTypes.Any())
                {
                    _logger.LogWarning("mylogs Missing Rate Types: {Missing}", string.Join(", ", missingTypes));
                    return CResponseException($"The following rate types do not exist: {string.Join(", ", missingTypes)}");
                }

                // Use ToDictionary with a grouping check to handle potential duplicates in DB safely
                var rateTypeMap = rateTypes
                    .GroupBy(ct => ct.Code)
                    .ToDictionary(g => g.Key, g => g.First().Id);

                // Check for duplicates in the incoming array
                var duplicateCodesInBody = body.GroupBy(x => x.Code)
                                             .Where(g => g.Count() > 1)
                                             .Select(y => y.Key)
                                             .ToList();

                if (duplicateCodesInBody.Any())
                {
                    return CResponseException($"Duplicate rate codes in request: {string.Join(", ", duplicateCodesInBody)}");
                }

                foreach (var item in body)
                {
                    var isExist = await _context.Rates.AnyAsync(x => x.Code == item.Code);
                    if (isExist)
                    {
                        return CResponseException($"Rate code '{item.Code}' already exists");
                    }
                }

                var rates = new List<Models.Rate.Rate>();
                var currentUserId = _identityService.GetUserId();
                var now = DateTime.UtcNow;

                foreach (var item in body)
                {
                    var rate = _mapper.Map<Models.Rate.Rate>(item);
                    rate.RateTypeId = rateTypeMap[item.RateType!.Code!];
                    rate.CreatedAt = now;
                    rate.CreatedBy = currentUserId;
                    rates.Add(rate);
                }

                _context.Rates.AddRange(rates);
                await _context.SaveChangesAsync();

                var rateDtos = _mapper.Map<List<RateDto>>(rates);

                await _auditService.LogAsync(
                    action: "Create Rates (Batch)",
                    module: "Rate Management",
                    entityType: "Rate",
                    newValues: rateDtos
                );

                return CResponseCreateSuccessful(rateDtos);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }



        [Authorize(Roles = "SA,ADM")]
        [HttpPut]
        public async Task<IActionResult> UpdateRates([FromBody] List<RateUpdateDto> body)
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

                var rates = await _context.Rates.Where(x => inputIds.Contains(x.Id)).ToListAsync();
                if (rates.Count != inputIds.Count)
                {
                    var foundIds = rates.Select(x => x.Id).ToList();
                    var missingIds = inputIds.Except(foundIds);
                    return CResponseException($"Rates not found: {string.Join(", ", missingIds)}");
                }

                var oldRatesDto = _mapper.Map<List<RateDto>>(rates);

                var requestedRateTypes = body
                    .Where(b => b.RateType != null && !string.IsNullOrEmpty(b.RateType.Code))
                    .Select(b => b.RateType!.Code)
                    .Distinct()
                    .ToList();

                Dictionary<string, int> rateTypeMap = new();
                if (requestedRateTypes.Any())
                {
                    var rateTypes = await _context.SystemCodes
                        .Include(sc => sc.CodeType)
                        .Where(ct => requestedRateTypes.Contains(ct.Code) && ct.CodeType!.Code == "RATE_TYPE")
                        .ToListAsync();

                    var foundCodes = rateTypes.Select(ct => ct.Code).Distinct().ToList();
                    var missingTypes = requestedRateTypes.Where(r => !foundCodes.Contains(r!)).ToList();

                    if (missingTypes.Any())
                    {
                        return CResponseException($"The following rate types do not exist: {string.Join(", ", missingTypes)}");
                    }

                    rateTypeMap = rateTypes
                        .GroupBy(ct => ct.Code)
                        .ToDictionary(g => g.Key, g => g.First().Id);
                }

                var currentUserId = _identityService.GetUserId();
                var now = DateTime.UtcNow;

                foreach (var dto in body)
                {
                    var entity = rates.First(x => x.Id == dto.Id);

                    var newCode = dto.Code ?? entity.Code;
                    var newRateTypeId = dto.RateType != null && !string.IsNullOrEmpty(dto.RateType.Code)
                        ? rateTypeMap[dto.RateType.Code]
                        : entity.RateTypeId;

                    var isExistInDb = await _context.Rates.AnyAsync(x =>
                        x.Id != dto.Id &&
                        x.Code == newCode);

                    if (isExistInDb)
                    {
                        return CResponseException($"Rate code '{newCode}' already exists");
                    }

                    var isDuplicateInBatch = body.Any(b =>
                        b.Id != dto.Id &&
                        (b.Code ?? rates.First(s => s.Id == b.Id).Code) == newCode);

                    if (isDuplicateInBatch)
                    {
                        return CResponseException($"Duplicate rate code '{newCode}' found within the request batch");
                    }

                    entity.Code = dto.Code ?? entity.Code;
                    entity.Description = dto.Description ?? entity.Description;
                    entity.RateValue = dto.Rate ?? entity.RateValue;
                    entity.RateTypeId = newRateTypeId;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = currentUserId;
                }

                await _context.SaveChangesAsync();

                var ratesDto = _mapper.Map<List<RateDto>>(rates);

                await _auditService.LogAsync(
                    action: "Update Rates (Batch)",
                    module: "Rate Management",
                    entityType: "Rate",
                    oldValues: oldRatesDto,
                    newValues: ratesDto
                );

                return CResponseUpdateSuccessful(ratesDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }



        [Authorize(Roles = "SA,ADM")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteRates([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return CResponseInvalidDataToSave();
                }

                var rates = await _context.Rates.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (rates.Count != ids.Distinct().Count())
                {
                    var foundIds = rates.Select(x => x.Id).ToList();
                    var missingIds = ids.Except(foundIds);
                    return CResponseException($"Rates not found: {string.Join(", ", missingIds)}");
                }

                var oldRatesDto = _mapper.Map<List<RateDto>>(rates);

                _context.Rates.RemoveRange(rates);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    action: "Delete Rates (Batch)",
                    module: "Rate Management",
                    entityType: "Rate",
                    oldValues: oldRatesDto
                );

                return CResponseDeleteSuccessful();
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }
    }
}
