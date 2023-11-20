namespace RandomMessageApp.Application.Models;

public class RandomMessageModel
{
    public int Count { get; set; }

    public List<RandomMessageEntryModel> Entries { get; set; }
}
