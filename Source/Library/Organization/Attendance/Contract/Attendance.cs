namespace Library.Contract;

/// <summary>Présence d'un membre à une séance.</summary>
public enum AttendanceStatus
{
    Present,
    Absent,
    /// <summary>Absence signalée à l'avance par le membre.</summary>
    Excused
}

/// <summary>Comment la présence a été enregistrée.</summary>
public enum AttendanceMethod
{
    /// <summary>Appel fait par un animateur de la séance.</summary>
    RollCall,
    /// <summary>QR code scanné par le membre.</summary>
    QrCode,
    /// <summary>Absence signalée par le membre.</summary>
    MemberNotice,
    /// <summary>Saisie ou correction par un responsable.</summary>
    StaffCorrection
}

/// <summary>Une ligne de la feuille d'émargement : un participant et sa présence (absente si rien n'est enregistré).</summary>
public sealed class AttendanceRow
{
    public Guid MemberId { get; set; }
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = "";
    public DateTime? Birthdate { get; set; }
    /// <summary>Faux : présence enregistrée pour un membre qui n'est plus inscrit.</summary>
    public bool IsSubscribed { get; set; }
    public AttendanceStatus? Status { get; set; }
    public AttendanceMethod? Method { get; set; }
    public string? Comment { get; set; }
    public DateTime? RecordedAt { get; set; }
}

/// <summary>Feuille d'émargement d'une séance (créneau d'atelier à une date, ou événement).</summary>
public sealed class AttendanceSheet
{
    public SubscriptionTargetKind Kind { get; set; }
    public Guid TargetId { get; set; }
    public DateOnly Date { get; set; }
    public string Title { get; set; } = "";
    /// <summary>Horaires de la séance (heure de Paris).</summary>
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? SpaceName { get; set; }
    public List<string> Animators { get; set; } = [];
    public List<AttendanceRow> Rows { get; set; } = [];

    public int Present => Rows.Count(row => row.Status == AttendanceStatus.Present);
    public int Absent => Rows.Count(row => row.Status == AttendanceStatus.Absent);
    public int Excused => Rows.Count(row => row.Status == AttendanceStatus.Excused);
    public int NotRecorded => Rows.Count(row => row.Status is null);
}

/// <summary>Présence à enregistrer pour un membre (appel).</summary>
public sealed class AttendanceEntry
{
    public Guid MemberId { get; set; }
    public AttendanceStatus Status { get; set; }
}

/// <summary>Une séance d'un créneau ou d'un événement, avec le décompte de ses présences.</summary>
public sealed class AttendanceOccurrence
{
    public DateOnly Date { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Excused { get; set; }
    /// <summary>Vrai si au moins une présence a été enregistrée.</summary>
    public bool IsRecorded => Present + Absent + Excused > 0;
}

/// <summary>Séances d'un créneau ou d'un événement.</summary>
public sealed class AttendanceOccurrences
{
    public SubscriptionTargetKind Kind { get; set; }
    public Guid TargetId { get; set; }
    public string Title { get; set; } = "";
    public List<AttendanceOccurrence> Occurrences { get; set; } = [];
}

/// <summary>Fréquentation d'un créneau ou d'un événement sur une période.</summary>
public sealed class AttendanceTargetStats
{
    public SubscriptionTargetKind Kind { get; set; }
    public Guid TargetId { get; set; }
    public string Title { get; set; } = "";
    /// <summary>Séances dont l'appel a été fait.</summary>
    public int Sessions { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Excused { get; set; }
    public double? Rate => Present + Absent + Excused == 0 ? null : (double)Present / (Present + Absent + Excused);
}

/// <summary>Assiduité d'un membre sur une période.</summary>
public sealed class AttendanceMemberStats
{
    public Guid MemberId { get; set; }
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = "";
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Excused { get; set; }
    public double? Rate => Present + Absent + Excused == 0 ? null : (double)Present / (Present + Absent + Excused);
}

/// <summary>Statistiques de fréquentation sur une période (dates incluses).</summary>
public sealed class AttendanceStats
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public List<AttendanceTargetStats> Targets { get; set; } = [];
    public List<AttendanceMemberStats> Members { get; set; } = [];
}
