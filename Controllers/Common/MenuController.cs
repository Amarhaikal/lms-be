using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANTM.Controllers.Common;
using QUANTM.Data;

namespace QUANTM.Controllers.Common
{
    [ApiController]
    [Route("api/menu")]
    public class MenuController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            try
            {
                var roleCode = User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(roleCode))
                    return CResponseUnauthorized("Role not found in token");

                var menus = await _context.MenuRoles
                    .Where(mr => mr.Role.Code == roleCode)
                    .Select(mr => mr.Menu)
                    .OrderBy(m => m.SortOrder)
                    .ToListAsync();

                // Build hierarchy
                var menuDtos = menus.Select(m => new QUANTM.DTOs.Common.MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    Url = m.Url,
                    Icon = m.Icon,
                    ParentId = m.ParentId,
                }).ToList();

                var menuMap = menuDtos.ToDictionary(m => m.Id);
                var rootMenus = new List<QUANTM.DTOs.Common.MenuDto>();

                foreach (var menu in menuDtos)
                {
                    if (menu.ParentId.HasValue && menuMap.ContainsKey(menu.ParentId.Value))
                    {
                        menuMap[menu.ParentId.Value].Childs.Add(menu);
                    }
                    else
                    {
                        rootMenus.Add(menu);
                    }
                }

                return CResponseGetListSuccessful(rootMenus);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }
    }
}