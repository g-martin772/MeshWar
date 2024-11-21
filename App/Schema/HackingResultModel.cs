namespace App.Schema;

public class HackingResultModel
{
    public required string HackingResult { get; set; }
    
    public static HackingResultModel Defended => new()
    {
        HackingResult = Schema.HackingResult.Defended
    };
    
    public static HackingResultModel Hacked => new()
    {
        HackingResult = Schema.HackingResult.Hacked
    };
}