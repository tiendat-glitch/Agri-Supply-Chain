using System;
using System.Collections.Generic;

namespace QL_chuoi_cung_ung_nong_san.Models;

public partial class Inspection
{
    public int Id { get; set; }

    public int BatchId { get; set; }

    public int? InspectorUserId { get; set; }

    public DateTime InspectionDate { get; set; }

    public decimal? Humidity { get; set; }

    public decimal? Temperature { get; set; }

    public decimal? ChemicalResidue { get; set; }

    public int? QualityScore { get; set; }

    public string? ReportFile { get; set; }

    public int? SignatureId { get; set; }

    public string? Notes { get; set; }

    public virtual Batch Batch { get; set; } = null!;

    public virtual User? InspectorUser { get; set; }

    public virtual DigitalSignature? Signature { get; set; }
}
