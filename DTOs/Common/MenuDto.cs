using System.Collections.Generic;

namespace QUANTM.DTOs.Common
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public int? ParentId { get; set; }
        public List<MenuDto> Childs { get; set; } = new List<MenuDto>();
    }
}
