using System;
using System.Windows;

namespace Tour_Planner.Views
{
    public partial class AddTourLogWindow : Window
    {
        public DateTime LogDate { get; set; }
        public string? Comment { get; set; }
        public double Difficulty { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TotalTime { get; set; }
        public double Rating { get; set; }

        public AddTourLogWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void LoadTourLogData()
        {
            LogDatePicker.SelectedDate = LogDate;
            CommentTextBox.Text = Comment;
            DifficultyTextBox.Text = Difficulty.ToString();
            TotalDistanceTextBox.Text = TotalDistance.ToString();
            TotalTimeTextBox.Text = TotalTime.ToString();
            RatingTextBox.Text = Rating.ToString();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            LogDate = DateTime.SpecifyKind((LogDatePicker.SelectedDate ?? DateTime.Now).Date, DateTimeKind.Utc);
            Comment = CommentTextBox.Text;

            if (!double.TryParse(DifficultyTextBox.Text, out var difficulty) || difficulty < 1 || difficulty > 10)
            {
                MessageBox.Show("Please enter a valid difficulty between 1 and 10.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Difficulty = difficulty;

            if (!double.TryParse(TotalDistanceTextBox.Text, out var totalDistance))
            {
                MessageBox.Show("Please enter a valid number for total distance.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            TotalDistance = totalDistance;

            if (!double.TryParse(RatingTextBox.Text, out var rating))
            {
                MessageBox.Show("Please enter a valid number for rating.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Rating = rating;

            if (!TimeSpan.TryParse(TotalTimeTextBox.Text, out var totalTime))
            {
                MessageBox.Show("Please enter a valid time for total time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            TotalTime = totalTime;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
    
}