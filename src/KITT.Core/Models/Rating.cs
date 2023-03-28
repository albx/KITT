namespace KITT.Core.Models;

public class Rating
{
    public Guid Id { get; protected set; }

    public string Website { get; protected set; }

    public string PageUrl { get; protected set; }

    public int NumberOfLikes { get; protected set; }

    public int NumberOfDislikes { get; protected set; }
}
