using System;
using System.Collections.Generic;

namespace QL_chuoi_cung_ung_nong_san.Models;

public partial class Farm
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? OwnerName { get; set; }

    public string? Location { get; set; }

    public string? ContactInfo { get; set; }

    public string? Certifications { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
