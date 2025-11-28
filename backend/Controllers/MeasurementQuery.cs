using Models;

public class MeasurementQuery
{
    public string? SensorId { get; set; }
    public SensorType? SensorType { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? SortBy { get; set; }
    public bool Desc { get; set; }
}
