using System;
using System.Collections.Generic;

namespace QL_chuoi_cung_ung_nong_san.Models;

public partial class Retailer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public string? ContactInfo { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<RetailerStock> RetailerStocks { get; set; } = new List<RetailerStock>();
}
