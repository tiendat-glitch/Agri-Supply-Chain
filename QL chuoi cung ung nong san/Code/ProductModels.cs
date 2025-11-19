using System;
using System.Collections.Generic;
namespace QL_chuoi_cung_ung_nong_san.Code
{
    public class ProductModels
    {
        public class ProductCreateModel
        {
            public string Name { get; set; } = null!;
            public string? Sku { get; set; }
            public string? Category { get; set; }
            public string? StorageInstructions { get; set; }
            public int? TypicalShelfLifeDays { get; set; }
        }
        public class ProductUpdateModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string? Sku { get; set; }
            public string? Category { get; set; }
            public string? StorageInstructions { get; set; }
            public int? TypicalShelfLifeDays { get; set; }
        }
    }
}
