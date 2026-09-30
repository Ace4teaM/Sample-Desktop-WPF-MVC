namespace DataModel;

public class Genre
{
    public Guid GenreId { get; set; }

    public required string Name { get; set; }

    public ICollection<Manga> Mangas { get; set; } = new List<Manga>();
}
