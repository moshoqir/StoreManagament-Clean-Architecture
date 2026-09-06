using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Product;

namespace Application.DTOs.Category
{
    public class CategoryDetailsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<ProdcutDto> Products { get; set; } = [];
    }
}
