namespace DataModel;

public class Manga
{
    public Guid MangaId { get; set; }

    public Guid CollectionId { get; set; }

    public required string Title { get; set; }

    public string? Author { get; set; }

    public int OwnedVolumeCount { get; set; }

    public int? TotalVolumeCount { get; set; }

    public PublicationStatus PublicationStatus { get; set; } = PublicationStatus.Unknown;

    public string? CoverUrl { get; set; }

    public string? PersonalNote { get; set; }

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
