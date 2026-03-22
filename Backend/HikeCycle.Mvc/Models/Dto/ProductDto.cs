using System;
using System.Collections.Generic;

namespace HikeCycle.Mvc.Models.Dto
{
    public class ProductDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal? PricePerDay { get; set; }
        public int? Stock { get; set; }
        public string? Status { get; set; }
        public string? Level { get; set; }
        public decimal? Rating { get; set; }
        public int? ReviewCount { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? Specs { get; set; }

        public string? SuitableFor { get; set; }

        public string? Variants { get; set; }

        public List<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>();
    }
}
