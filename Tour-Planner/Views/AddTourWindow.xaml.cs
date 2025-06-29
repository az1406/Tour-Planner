using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace Tour_Planner.Views
{
    public partial class AddTourWindow : Window
    {
        public string WindowTitle => IsEditMode ? "Edit Tour" : "Add Tour";

        public string? TourName { get; set; }
        public string? TourDescription { get; set; }
        public string? TourImagePath { get; set; }
        public bool IsEditMode { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? TransportType { get; set; }
        public double? TourDistance { get; set; }
        public TimeSpan? EstimatedTime { get; set; }
        public string? RouteInformation { get; set; }

        public AddTourWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void LoadTourData()
        {
            TourNameTextBox.Text = TourName ?? string.Empty;
            TourDescriptionTextBox.Text = TourDescription ?? string.Empty;
            TourImageTextBox.Text = TourImagePath ?? string.Empty;
            FromTextBox.Text = From ?? string.Empty;
            ToTextBox.Text = To ?? string.Empty;
            TransportTypeTextBox.Text = TransportType ?? string.Empty;
            TourDistanceTextBox.Text = TourDistance.HasValue ? TourDistance.Value.ToString() : string.Empty;
            EstimatedTimeTextBox.Text = EstimatedTime.HasValue ? EstimatedTime.Value.ToString(@"hh\:mm") : string.Empty;
            RouteInfoTextBox.Text = RouteInformation ?? string.Empty;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TourName = string.IsNullOrWhiteSpace(TourNameTextBox.Text) ? null : TourNameTextBox.Text;
            TourDescription = string.IsNullOrWhiteSpace(TourDescriptionTextBox.Text) ? null : TourDescriptionTextBox.Text;

            var imagePath = TourImageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                try
                {
                    var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var imgFolderPath = Path.Combine(projectDirectory, "img");

                    if (!Directory.Exists(imgFolderPath))
                    {
                        Directory.CreateDirectory(imgFolderPath);
                    }

                    var fileName = Path.GetFileName(imagePath);
                    var destinationPath = Path.Combine(imgFolderPath, fileName);

                    if (!File.Exists(destinationPath))
                    {
                        byte[] fileBytes = File.ReadAllBytes(imagePath);
                        File.WriteAllBytes(destinationPath, fileBytes);
                    }

                    TourImagePath = Path.Combine("img", fileName);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            From = string.IsNullOrWhiteSpace(FromTextBox.Text) ? null : FromTextBox.Text;
            To = string.IsNullOrWhiteSpace(ToTextBox.Text) ? null : ToTextBox.Text;
            TransportType = string.IsNullOrWhiteSpace(TransportTypeTextBox.Text) ? null : TransportTypeTextBox.Text;
            if (double.TryParse(TourDistanceTextBox.Text, out var distance))
            {
                TourDistance = distance;
            }
            if (TimeSpan.TryParse(EstimatedTimeTextBox.Text, out var time))
            {
                EstimatedTime = time;
            }
            RouteInformation = string.IsNullOrWhiteSpace(RouteInfoTextBox.Text) ? null : RouteInfoTextBox.Text;

            if (!IsEditMode && (string.IsNullOrWhiteSpace(TourName) || string.IsNullOrWhiteSpace(TourDescription)))
            {
                MessageBox.Show("Tour Name and Description are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*",
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                TourImageTextBox.Text = openFileDialog.FileName;
            }
        }

        public void UpdateButtonContent()
        {
            AddButton.Content = IsEditMode ? "Save" : "Add";
        }
    }
}