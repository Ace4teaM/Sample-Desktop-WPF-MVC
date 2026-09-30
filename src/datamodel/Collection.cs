namespace DataModel;

public class Collection
{
    public Guid CollectionId { get; set; }

    public ICollection<Manga> Mangas { get; set; } = new List<Manga>();
}
