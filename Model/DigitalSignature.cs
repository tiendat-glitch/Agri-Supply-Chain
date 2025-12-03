using System;
using System.Collections.Generic;

namespace Model;

public partial class DigitalSignature
{
    public int Id { get; set; }

    public int? SignerUserId { get; set; }

    public string? SignatureMethod { get; set; }

    public string? SignatureValue { get; set; }

    public DateTime SignedAt { get; set; }

    public string? ReferenceDocument { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();

    public virtual User? SignerUser { get; set; }
}
