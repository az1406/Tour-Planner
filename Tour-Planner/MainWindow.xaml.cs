using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Tour_Planner.ViewModels;
using Tour_Planner.db;

namespace Tour_Planner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                var mainViewModel = ((App)Application.Current).Host.Services.GetRequiredService<MainViewModel>();
                DataContext = mainViewModel;
                Console.WriteLine("MainWindow DataContext set to MainViewModel.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing MainWindow: {ex.Message}");
                throw;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                MessageBox.Show($"Search: {viewModel.SearchText}", "Search results");
            }
        }


    }
}