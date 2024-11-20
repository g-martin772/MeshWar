namespace App.Schema;

public class StatusModel
{
    public required int Points { get; set; }
    public required double Attack { get; set; }
    public required double Defense { get; set; }
    public required string State { get; set; }
}