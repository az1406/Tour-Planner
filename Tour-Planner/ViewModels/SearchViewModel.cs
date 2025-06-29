using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Tour_Planner.ViewModels
{
    public class SearchViewModel : ObservableObject
    {
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public IRelayCommand SearchCommand { get; }

        public SearchViewModel()
        {
            SearchCommand = new RelayCommand(PerformSearch);
        }

        private void PerformSearch()
        {
            Console.WriteLine($"Searching for: {SearchText}");
        }
    }
}
