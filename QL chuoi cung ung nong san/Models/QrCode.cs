using System;
using System.Collections.Generic;

namespace QL_chuoi_cung_ung_nong_san.Models;

public partial class QrCode
{
    public int Id { get; set; }

    public int BatchId { get; set; }

    public string Token { get; set; } = null!;

    public string? Url { get; set; }

    public DateTime GeneratedAt { get; set; }

    public virtual Batch Batch { get; set; } = null!;
}
