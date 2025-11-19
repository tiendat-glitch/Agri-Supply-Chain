using System;
using System.Collections.Generic;

namespace QL_chuoi_cung_ung_nong_san.Models;

public partial class Product
{
    public Product()
    {
        Batches = new HashSet<Batch>();
    }
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Sku { get; set; }

    public string? Category { get; set; }

    public string? StorageInstructions { get; set; }

    public int? TypicalShelfLifeDays { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
