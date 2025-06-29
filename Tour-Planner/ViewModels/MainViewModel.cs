using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using Tour_Planner.db;
using Tour_Planner.Views;
using System.IO;
using System.Windows;

namespace Tour_Planner.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly TourPlannerDbContext _dbContext;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private ObservableCollection<Tour>? _tours;
        public ObservableCollection<Tour>? Tours
        {
            get => _tours;
            set => SetProperty(ref _tours, value);
        }

        private Tour? _selectedTour;
        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                SetProperty(ref _selectedTour, value);
                Console.WriteLine($"SelectedTour.ImagePath: {_selectedTour?.ImagePath}");
                EditTourCommand.NotifyCanExecuteChanged();
                DeleteTourCommand.NotifyCanExecuteChanged();
                if (_selectedTour != null)
                {
                    LoadTourLogs(_selectedTour.Id);
                }
            }
        }

        private ObservableCollection<TourLog>? _tourLogs;
        public ObservableCollection<TourLog>? TourLogs
        {
            get => _tourLogs;
            set => SetProperty(ref _tourLogs, value);
        }

        private TourLog? _selectedTourLog;
        public TourLog? SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                SetProperty(ref _selectedTourLog, value);
                EditTourLogCommand.NotifyCanExecuteChanged();
                DeleteTourLogCommand.NotifyCanExecuteChanged();
            }
        }

        public IRelayCommand AddTourCommand { get; }
        public IRelayCommand EditTourCommand { get; }
        public IRelayCommand DeleteTourCommand { get; }
        public IRelayCommand AddTourLogCommand { get; }
        public IRelayCommand EditTourLogCommand { get; }
        public IRelayCommand DeleteTourLogCommand { get; }

        public MainViewModel(TourPlannerDbContext dbContext)
        {
            _dbContext = dbContext;
            LoadTours();

            AddTourCommand = new RelayCommand(AddTour);
            EditTourCommand = new RelayCommand(EditTour, CanEditOrDeleteTour);
            DeleteTourCommand = new RelayCommand(DeleteTour, CanEditOrDeleteTour);
            AddTourLogCommand = new RelayCommand(AddTourLog);
            EditTourLogCommand = new RelayCommand(EditTourLog, CanEditOrDeleteTourLog);
            DeleteTourLogCommand = new RelayCommand(DeleteTourLog, CanEditOrDeleteTourLog);
        }

        private void LoadTours()
        {
            Console.WriteLine("Loading tours from the database...");
            Tours = new ObservableCollection<Tour>(_dbContext.Tours.OrderBy(t => t.Id).ToList());
            if (Tours.Any())
            {
                foreach (var tour in Tours)
                {
                    if (!string.IsNullOrWhiteSpace(tour.ImagePath))
                    {
                        var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        var fullPath = Path.Combine(projectDirectory, tour.ImagePath);

                        if (File.Exists(fullPath))
                        {
                            tour.ImagePath = fullPath;
                        }
                        else
                        {
                            Console.WriteLine($"Image not found for tour '{tour.Name}': {fullPath}");
                        }
                    }

                    Console.WriteLine($"Loaded Tour: {tour.Name}, Description: {tour.Description}, ImagePath: {tour.ImagePath}");
                }
            }
            else
            {
                Console.WriteLine("No tours found in the database.");
            }
        }

        private void LoadTourLogs(int tourId)
        {
            TourLogs = new ObservableCollection<TourLog>(_dbContext.TourLogs.Where(tl => tl.TourId == tourId).OrderBy(tl => tl.Date).ToList());
        }

        private void AddTour()
        {
            var addTourWindow = new AddTourWindow();
            if (addTourWindow.ShowDialog() == true)
            {
                try
                {
                    var newTour = new Tour
                    {
                        Name = addTourWindow.TourName,
                        Description = addTourWindow.TourDescription,
                        ImagePath = addTourWindow.TourImagePath,
                        From = addTourWindow.From,
                        To = addTourWindow.To,
                        TransportType = addTourWindow.TransportType,
                        TourDistance = addTourWindow.TourDistance ?? 0,
                        EstimatedTime = addTourWindow.EstimatedTime ?? TimeSpan.Zero,
                        RouteInformation = addTourWindow.RouteInformation
                    };

                    _dbContext.Tours.Add(newTour);
                    _dbContext.SaveChanges();
                    LoadTours();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding tour: {ex.Message}");
                }
            }
        }

        private void EditTour()
        {
            try
            {
                if (SelectedTour != null)
                {
                    var editTourWindow = new AddTourWindow
                    {
                        TourName = SelectedTour.Name,
                        TourDescription = SelectedTour.Description,
                        TourImagePath = SelectedTour.ImagePath,
                        From = SelectedTour.From,
                        To = SelectedTour.To,
                        TransportType = SelectedTour.TransportType,
                        TourDistance = SelectedTour.TourDistance,
                        EstimatedTime = SelectedTour.EstimatedTime,
                        RouteInformation = SelectedTour.RouteInformation,
                        IsEditMode = true
                    };

                    editTourWindow.LoadTourData();
                    editTourWindow.UpdateButtonContent();

                    if (editTourWindow.ShowDialog() == true)
                    {
                        SelectedTour.Name = editTourWindow.TourName;
                        SelectedTour.Description = editTourWindow.TourDescription;
                        SelectedTour.ImagePath = editTourWindow.TourImagePath;
                        SelectedTour.From = editTourWindow.From;
                        SelectedTour.To = editTourWindow.To;
                        SelectedTour.TransportType = editTourWindow.TransportType;
                        SelectedTour.TourDistance = editTourWindow.TourDistance ?? 0;
                        SelectedTour.EstimatedTime = editTourWindow.EstimatedTime ?? TimeSpan.Zero;
                        SelectedTour.RouteInformation = editTourWindow.RouteInformation;

                        _dbContext.Tours.Update(SelectedTour);
                        _dbContext.SaveChanges();
                        LoadTours();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a tour to edit.", "Edit Tour", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing tour: {ex.Message}");
            }
        }

        private void DeleteTour()
        {
            try
            {
                if (SelectedTour != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete the tour '{SelectedTour.Name}' and all its logs?",
                        "Delete Tour", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        
                        var tourLogs = _dbContext.TourLogs.Where(tl => tl.TourId == SelectedTour.Id).ToList();
                        _dbContext.TourLogs.RemoveRange(tourLogs);

                        
                        _dbContext.Tours.Remove(SelectedTour);
                        _dbContext.SaveChanges();
                        LoadTours();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a tour to delete.", "Delete Tour", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting tour: {ex.Message}");
            }
        }

        private void AddTourLog()
        {
            if (SelectedTour == null)
            {
                MessageBox.Show("Please select a tour before adding a log.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addTourLogWindow = new AddTourLogWindow();
            if (addTourLogWindow.ShowDialog() == true)
            {
                try
                {
                    var newTourLog = new TourLog
                    {
                        TourId = SelectedTour.Id, 
                        Date = addTourLogWindow.LogDate,
                        Comment = addTourLogWindow.Comment,
                        Difficulty = addTourLogWindow.Difficulty,
                        TotalDistance = addTourLogWindow.TotalDistance,
                        TotalTime = addTourLogWindow.TotalTime,
                        Rating = addTourLogWindow.Rating
                    };

                    Console.WriteLine($"Adding TourLog: {newTourLog.Date}, {newTourLog.Comment}, {newTourLog.Difficulty}, {newTourLog.TotalDistance}, {newTourLog.TotalTime}, {newTourLog.Rating}");

                    _dbContext.TourLogs.Add(newTourLog);
                    _dbContext.SaveChanges();
                    LoadTourLogs(SelectedTour.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding tour log: {ex.Message}");
                }
            }
        }
        private void EditTourLog()
        {
            if (SelectedTourLog != null)
            {
                var editTourLogWindow = new AddTourLogWindow
                {
                    LogDate = SelectedTourLog.Date,
                    Comment = SelectedTourLog.Comment,
                    Difficulty = SelectedTourLog.Difficulty,
                    TotalDistance = SelectedTourLog.TotalDistance,
                    TotalTime = SelectedTourLog.TotalTime,
                    Rating = SelectedTourLog.Rating
                };

                editTourLogWindow.LoadTourLogData();

                if (editTourLogWindow.ShowDialog() == true)
                {
                    SelectedTourLog.Date = editTourLogWindow.LogDate;
                    SelectedTourLog.Comment = editTourLogWindow.Comment;
                    SelectedTourLog.Difficulty = editTourLogWindow.Difficulty;
                    SelectedTourLog.TotalDistance = editTourLogWindow.TotalDistance;
                    SelectedTourLog.TotalTime = editTourLogWindow.TotalTime;
                    SelectedTourLog.Rating = editTourLogWindow.Rating;

                    _dbContext.TourLogs.Update(SelectedTourLog);
                    _dbContext.SaveChanges();
                    LoadTourLogs(SelectedTour?.Id ?? 0);
                }
            }
        }

        private void DeleteTourLog()
        {
            if (SelectedTourLog != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete this tour log?", "Delete Tour Log", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _dbContext.TourLogs.Remove(SelectedTourLog);
                    _dbContext.SaveChanges();
                    LoadTourLogs(SelectedTour?.Id ?? 0);
                }
            }
        }

        private bool CanEditOrDeleteTourLog() => SelectedTourLog != null;
        private bool CanEditOrDeleteTour() => SelectedTour != null;
    }
}