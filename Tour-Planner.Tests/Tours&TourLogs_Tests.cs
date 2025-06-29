using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Tour_Planner.ViewModels;
using Tour_Planner.db;
using Microsoft.EntityFrameworkCore;

namespace Tour_Planner.Tests
{
    public class MainViewModelTests
    {
        private readonly TourPlannerDbContext _dbContext;
        private readonly MainViewModel _viewModel;

        public MainViewModelTests()
        {
            var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new TourPlannerDbContext(options);

            _dbContext.Tours.RemoveRange(_dbContext.Tours);
            _dbContext.TourLogs.RemoveRange(_dbContext.TourLogs);
            _dbContext.SaveChanges();

            _dbContext.Tours.AddRange(new List<Tour>
            {
                new Tour { Id = 1, Name = "Tour 1", Description = "Description 1", ImagePath = "Path1" },
                new Tour { Id = 2, Name = "Tour 2", Description = "Description 2", ImagePath = "Path2" }
            });

            _dbContext.TourLogs.AddRange(new List<TourLog>
            {
                new TourLog { Id = 1, TourId = 1, Comment = "Log 1", Difficulty = 3, TotalDistance = 10, TotalTime = TimeSpan.FromMinutes(60), Rating = 4 },
                new TourLog { Id = 2, TourId = 1, Comment = "Log 2", Difficulty = 2, TotalDistance = 15, TotalTime = TimeSpan.FromMinutes(90), Rating = 5 }
            });

            _dbContext.SaveChanges();

            _viewModel = new MainViewModel(_dbContext);
        }

        [Fact]
        public void isPopulated_database()
        {
            // Act
            var tours = _viewModel.Tours;

            // Assert
            Assert.NotNull(tours);
            Assert.Equal(2, tours.Count);
            Assert.Contains(tours, t => t.Name == "Tour 1");
            Assert.Contains(tours, t => t.Name == "Tour 2");
        }

        [Fact]
        public void isEmpty_database()
        {
            // Arrange
            _dbContext.Tours.RemoveRange(_dbContext.Tours);
            _dbContext.SaveChanges();

            // Act
            _viewModel.Tours = new ObservableCollection<Tour>(_dbContext.Tours.ToList());

            // Assert
            Assert.NotNull(_viewModel.Tours);
            Assert.Empty(_viewModel.Tours);
        }

        [Fact]
        public void isLoading_Tours()
        {
            // Arrange
            var tour = _viewModel.Tours!.First();

            // Act
            _viewModel.SelectedTour = tour;

            // Assert
            Assert.NotNull(_viewModel.TourLogs);
            Assert.Equal(2, _viewModel.TourLogs.Count);
            Assert.Contains(_viewModel.TourLogs, log => log.Comment == "Log 1");
            Assert.Contains(_viewModel.TourLogs, log => log.Comment == "Log 2");
        }

        [Fact]
        public void count_when_addTours()
        {
            // Arrange
            var initialCount = _viewModel.Tours!.Count;

            // Act
            _dbContext.Tours.Add(new Tour { Name = "New Tour", Description = "New Description", ImagePath = "NewPath" });
            _dbContext.SaveChanges();
            _viewModel.Tours = new ObservableCollection<Tour>(_dbContext.Tours.ToList());

            // Assert
            Assert.Equal(initialCount + 1, _viewModel.Tours.Count);
            Assert.Contains(_viewModel.Tours, t => t.Name == "New Tour");
        }

        [Fact]
        public void isDeleted_Tours()
        {
            // Arrange
            var tourToDelete = _viewModel.Tours!.First();
            _viewModel.SelectedTour = tourToDelete;

            // Act
            _dbContext.Tours.Remove(tourToDelete);
            _dbContext.SaveChanges();
            _viewModel.Tours = new ObservableCollection<Tour>(_dbContext.Tours.ToList());

            // Assert
            Assert.DoesNotContain(_viewModel.Tours, t => t.Id == tourToDelete.Id);
        }

        [Fact]
        public void count_for_TourLogs()
        {
            // Arrange
            var initialCount = _dbContext.TourLogs.Count();
            var newLog = new TourLog
            {
                TourId = 1,
                Comment = "New Log",
                Difficulty = 4,
                TotalDistance = 20,
                TotalTime = TimeSpan.FromMinutes(120),
                Rating = 5
            };

            // Act
            _dbContext.TourLogs.Add(newLog);
            _dbContext.SaveChanges();
            _viewModel.TourLogs = new ObservableCollection<TourLog>(_dbContext.TourLogs.Where(tl => tl.TourId == 1).ToList());

            // Assert
            Assert.Equal(initialCount + 1, _dbContext.TourLogs.Count());
            Assert.Contains(_viewModel.TourLogs, log => log.Comment == "New Log");
        }

        [Fact]
        public void isDeleted_TourLogs()
        {
            // Arrange
            var logToDelete = _dbContext.TourLogs.First();
            _viewModel.SelectedTour = _viewModel.Tours!.First();
            _viewModel.SelectedTourLog = logToDelete;

            // Act
            _dbContext.TourLogs.Remove(logToDelete);
            _dbContext.SaveChanges();
            _viewModel.TourLogs = new ObservableCollection<TourLog>(_dbContext.TourLogs.Where(tl => tl.TourId == 1).ToList());

            // Assert
            Assert.DoesNotContain(_viewModel.TourLogs, log => log.Id == logToDelete.Id);
        }
    }
}
