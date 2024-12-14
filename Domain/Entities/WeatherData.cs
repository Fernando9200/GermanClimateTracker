using Domain.Entities;

public class WeatherData : BaseEntity 
{
    public required string City { get; set; }
    public double Temperature { get; set; }
    public int Humidity { get; set; }
    public required string Description { get; set; }
    public double WindSpeed { get; set; }
    public DateTime Timestamp { get; set; }
    public required string Icon { get; set; }
    public int Pressure { get; set; }
}