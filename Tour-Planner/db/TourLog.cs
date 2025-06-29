using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class TourLog : INotifyPropertyChanged
{
    public int Id { get; set; }
    public int TourId { get; set; }

    private DateTime _date;
    public DateTime Date
    {
        get => _date;
        set { _date = value; OnPropertyChanged(); }
    }

    private string? _comment;
    public string? Comment
    {
        get => _comment;
        set { _comment = value; OnPropertyChanged(); }
    }

    private double _difficulty;
    public double Difficulty
    {
        get => _difficulty;
        set { _difficulty = value; OnPropertyChanged(); }
    }

    private double _totalDistance;
    public double TotalDistance
    {
        get => _totalDistance;
        set { _totalDistance = value; OnPropertyChanged(); }
    }

    private TimeSpan _totalTime;
    public TimeSpan TotalTime
    {
        get => _totalTime;
        set { _totalTime = value; OnPropertyChanged(); }
    }

    private double _rating;
    public double Rating
    {
        get => _rating;
        set { _rating = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}