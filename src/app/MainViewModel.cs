using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using DataModel;
using Microsoft.Win32;

namespace MangaCollection.App;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly HttpClient _httpClient = new();
    private Manga? _editingManga;
    private string _searchText = string.Empty;
    private string _statusMessage = "Votre collection est vide. Ajoutez un manga pour commencer.";
    private bool _isGridView;
    private bool _isSearching;

    public MainViewModel()
    {
        AddCommand = new RelayCommand(StartAdd);
        EditCommand = new RelayCommand(StartEdit, () => SelectedManga is not null);
        DeleteCommand = new RelayCommand(DeleteSelected, () => SelectedManga is not null);
        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(CancelEdit);
        SearchCommand = new RelayCommand(async () => await SearchAsync(), () => !IsSearching);
        ExportCommand = new RelayCommand(Export);
        ClearSearchCommand = new RelayCommand(() => SearchResults.Clear());
    }

    public ObservableCollection<Manga> Mangas { get; } = new();
    public ObservableCollection<CatalogManga> SearchResults { get; } = new();

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand ClearSearchCommand { get; }

    public Manga? SelectedManga
    {
        get;
        set
        {
            if (SetField(ref field, value))
            {
                ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public CatalogManga? SelectedSearchResult { get; set; }
    public bool IsEditing => _editingManga is not null;
    public bool IsGridView { get => _isGridView; set => SetField(ref _isGridView, value); }
    public bool IsSearching { get => _isSearching; private set => SetField(ref _isSearching, value); }
    public string StatusMessage { get => _statusMessage; private set => SetField(ref _statusMessage, value); }
    public string SearchText { get => _searchText; set => SetField(ref _searchText, value); }

    public string DraftTitle { get; set { SetField(ref field, value); ((RelayCommand)SaveCommand).RaiseCanExecuteChanged(); } } = string.Empty;
    public string DraftAuthor { get; set; } = string.Empty;
    public string DraftOwnedVolumes { get; set; } = "0";
    public string DraftTotalVolumes { get; set; } = string.Empty;
    public string DraftStatus { get; set; } = nameof(PublicationStatus.Unknown);
    public string DraftCoverUrl { get; set; } = string.Empty;
    public string DraftGenres { get; set; } = string.Empty;
    public string DraftNote { get; set; } = string.Empty;
    public string ValidationMessage { get; private set { SetField(ref field, value); } } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SelectCatalogResult(CatalogManga result)
    {
        SelectedSearchResult = result;
        StartAdd();
        DraftTitle = result.Title;
        DraftAuthor = result.Author ?? string.Empty;
        DraftTotalVolumes = result.TotalVolumes?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        DraftStatus = result.Status;
        DraftCoverUrl = result.CoverUrl ?? string.Empty;
    }

    private void StartAdd()
    {
        _editingManga = new Manga { MangaId = Guid.NewGuid(), CollectionId = Guid.Empty, Title = string.Empty };
        LoadDraft(_editingManga);
        OnPropertyChanged(nameof(IsEditing));
    }

    private void StartEdit()
    {
        if (SelectedManga is null) return;
        _editingManga = SelectedManga;
        LoadDraft(SelectedManga);
        OnPropertyChanged(nameof(IsEditing));
    }

    private void LoadDraft(Manga manga)
    {
        DraftTitle = manga.Title;
        DraftAuthor = manga.Author ?? string.Empty;
        DraftOwnedVolumes = manga.OwnedVolumeCount.ToString(CultureInfo.InvariantCulture);
        DraftTotalVolumes = manga.TotalVolumeCount?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        DraftStatus = manga.PublicationStatus.ToString();
        DraftCoverUrl = manga.CoverUrl ?? string.Empty;
        DraftGenres = string.Join(", ", manga.Genres.Select(g => g.Name));
        DraftNote = manga.PersonalNote ?? string.Empty;
        ValidationMessage = string.Empty;
    }

    private bool CanSave() => _editingManga is not null && !string.IsNullOrWhiteSpace(DraftTitle);

    private void Save()
    {
        if (_editingManga is null) return;
        if (!int.TryParse(DraftOwnedVolumes, NumberStyles.Integer, CultureInfo.InvariantCulture, out var owned) || owned < 0)
        {
            ValidationMessage = "Le nombre de volumes possédés doit être un entier supérieur ou égal à zéro.";
            return;
        }

        int? total = null;
        if (!string.IsNullOrWhiteSpace(DraftTotalVolumes))
        {
            if (!int.TryParse(DraftTotalVolumes, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedTotal) || parsedTotal < 0)
            {
                ValidationMessage = "Le nombre total de volumes doit être vide ou un entier positif.";
                return;
            }
            total = parsedTotal;
        }

        if (!Enum.TryParse<PublicationStatus>(DraftStatus, true, out var status))
            status = PublicationStatus.Unknown;

        var manga = _editingManga;
        manga.Title = DraftTitle.Trim();
        manga.Author = NullIfEmpty(DraftAuthor);
        manga.OwnedVolumeCount = owned;
        manga.TotalVolumeCount = total;
        manga.PublicationStatus = status;
        manga.CoverUrl = NullIfEmpty(DraftCoverUrl);
        manga.PersonalNote = NullIfEmpty(DraftNote);
        manga.Genres = DraftGenres.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase).Select(name => new Genre { GenreId = Guid.NewGuid(), Name = name }).ToList();

        if (!Mangas.Contains(manga)) Mangas.Add(manga);
        SelectedManga = manga;
        _editingManga = null;
        ValidationMessage = string.Empty;
        StatusMessage = $"{manga.Title} a été enregistré.";
        OnPropertyChanged(nameof(IsEditing));
    }

    private void CancelEdit()
    {
        _editingManga = null;
        ValidationMessage = string.Empty;
        OnPropertyChanged(nameof(IsEditing));
    }

    private void DeleteSelected()
    {
        if (SelectedManga is null) return;
        if (MessageBox.Show($"Supprimer « {SelectedManga.Title} » ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        Mangas.Remove(SelectedManga);
        SelectedManga = null;
        StatusMessage = "Le manga a été supprimé.";
        if (Mangas.Count == 0) StatusMessage = "Votre collection est vide. Ajoutez un manga pour commencer.";
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText)) return;
        IsSearching = true;
        SearchResults.Clear();
        try
        {
            using var stream = await _httpClient.GetStreamAsync($"https://api.jikan.moe/v4/manga?q={Uri.EscapeDataString(SearchText.Trim())}&limit=20");
            using var document = await JsonDocument.ParseAsync(stream);
            foreach (var item in document.RootElement.GetProperty("data").EnumerateArray())
            {
                var titles = item.GetProperty("title");
                SearchResults.Add(new CatalogManga(
                    titles.GetString() ?? "Sans titre",
                    item.TryGetProperty("authors", out var authors) ? authors.EnumerateArray().FirstOrDefault().GetProperty("name").GetString() : null,
                    item.TryGetProperty("volumes", out var volumes) && volumes.ValueKind == JsonValueKind.Number ? volumes.GetInt32() : null,
                    item.TryGetProperty("status", out var publication) ? publication.GetString() ?? "unknown" : "unknown",
                    item.TryGetProperty("images", out var images) && images.GetProperty("jpg").TryGetProperty("image_url", out var url) ? url.GetString() : null));
            }
            StatusMessage = SearchResults.Count == 0 ? "Aucun résultat pour cette recherche." : $"{SearchResults.Count} résultat(s) trouvé(s).";
        }
        catch (HttpRequestException)
        {
            StatusMessage = "Le catalogue est indisponible. Vérifiez votre connexion et réessayez.";
        }
        catch (JsonException)
        {
            StatusMessage = "La réponse du catalogue est invalide. Réessayez.";
        }
        finally
        {
            IsSearching = false;
        }
    }

    private void Export()
    {
        var dialog = new SaveFileDialog { Filter = "Fichier CSV (*.csv)|*.csv", FileName = "ma-collection.csv" };
        if (dialog.ShowDialog() != true) return;
        try
        {
            var lines = new List<string> { "Titre,Auteur,Volumes possédés,Volumes total,Statut,Genres,Note" };
            lines.AddRange(Mangas.Select(m => string.Join(",", new[]
            {
                Csv(m.Title), Csv(m.Author), m.OwnedVolumeCount.ToString(CultureInfo.InvariantCulture),
                Csv(m.TotalVolumeCount?.ToString(CultureInfo.InvariantCulture)), Csv(m.PublicationStatus.ToString()),
                Csv(string.Join(", ", m.Genres.Select(g => g.Name))), Csv(m.PersonalNote)
            })));
            File.WriteAllLines(dialog.FileName, lines, new UTF8Encoding(false));
            StatusMessage = "La collection a été exportée avec succès.";
        }
        catch (IOException) { StatusMessage = "L’export a échoué : le fichier ne peut pas être écrit."; }
        catch (UnauthorizedAccessException) { StatusMessage = "L’export a échoué : accès refusé."; }
    }

    private static string Csv(string? value) => string.IsNullOrEmpty(value) ? string.Empty : $"\"{value.Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ")}\"";
    private static string? NullIfEmpty(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private bool SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }
    private void OnPropertyChanged(string? name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed record CatalogManga(string Title, string? Author, int? TotalVolumes, string Status, string? CoverUrl);
