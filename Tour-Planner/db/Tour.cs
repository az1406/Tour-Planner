using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Tour : INotifyPropertyChanged
{
    public int Id { get; set; }

    private string? _name;
    public string? Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    private string? _imagePath;
    public string? ImagePath
    {
        get => _imagePath;
        set { _imagePath = value; OnPropertyChanged(); }
    }

    private string? _from;
    public string? From
    {
        get => _from;
        set { _from = value; OnPropertyChanged(); }
    }

    private string? _to;
    public string? To
    {
        get => _to;
        set { _to = value; OnPropertyChanged(); }
    }

    private string? _transportType;
    public string? TransportType
    {
        get => _transportType;
        set { _transportType = value; OnPropertyChanged(); }
    }

    private double _tourDistance;
    public double TourDistance
    {
        get => _tourDistance;
        set { _tourDistance = value; OnPropertyChanged(); }
    }

    private TimeSpan _estimatedTime;
    public TimeSpan EstimatedTime
    {
        get => _estimatedTime;
        set { _estimatedTime = value; OnPropertyChanged(); }
    }

    private string? _routeInformation;
    public string? RouteInformation
    {
        get => _routeInformation;
        set { _routeInformation = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}