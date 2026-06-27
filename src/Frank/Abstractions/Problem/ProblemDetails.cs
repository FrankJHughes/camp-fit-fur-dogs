namespace Frank.Abstractions.Exceptions;

public class ProblemDetails
{
    public string Title { get; set; } = default!;
    public string Detail { get; set; } = default!;
    public int? Status { get; set; }
    public string Type { get; set; } = default!;
    public Dictionary<string, string[]>? Errors { get; set; }
}
