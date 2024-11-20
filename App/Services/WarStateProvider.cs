using App.Schema;

namespace App.Services;

public class WarStateProvider
{
    public WarStateProvider(IConfiguration configuration)
    {
        var section = configuration.GetSection("War");

        Data = new StatusModel
        {
            Attack = double.Parse(section["StartingAttackValue"]!),
            Defense = double.Parse(section["StartingDefenseValue"]!),
            Points = int.Parse(section["StartingPoints"]!),
            State = WarState.Running
        };
    }

    public StatusModel Data { get; set; }
}