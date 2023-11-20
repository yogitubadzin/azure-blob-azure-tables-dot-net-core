namespace RandomMessageApp.Interfaces.RandomMessages;

public sealed record RandomMessage
{
    public int Count { get; set; }

    public List<RandomMessageEntry> Entries { get; set; }
}
