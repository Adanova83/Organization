namespace Library.Contract;

public class Active
{
    public DateTime From { get; set; } = DateTime.UtcNow;
    public DateTime? Until { get; set; } = null;
    public bool IsActive => !Until.HasValue || Until >= DateTime.UtcNow;

    public Active() { }

    public Active(DateTime from)
        : this(from, null)
    { }

    public Active(DateTime from, DateTime? until)
    {
        string error = CheckData(from, until);
        if (error != string.Empty)
        {
            throw new ArgumentException(error);
        }
        
        From = from;
        Until = until;
    }

    private string CheckData(DateTime from, DateTime? until)
    {
        if (until.HasValue && until <= from)
        {
            return "The 'until' date must be greater than the 'from' date.";
        }

        return string.Empty;
    }
}