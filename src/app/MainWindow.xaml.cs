using System.Windows;

namespace MangaCollection.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void SearchResults_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && DataContext is MainViewModel viewModel && e.AddedItems[0] is CatalogManga result)
            viewModel.SelectCatalogResult(result);
    }
}