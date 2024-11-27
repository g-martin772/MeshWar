using App.Schema;

namespace App.Services;

public class WarStateProvider
{
    public delegate void StateHasChangedEvent();

    public StateHasChangedEvent StateHasChanged = delegate { };

    public WarStateProvider(IConfiguration configuration)
    {
        var section = configuration.GetSection("War");

        Status = new StatusModel
        {
            Attack = double.Parse(section["StartingAttackValue"]!),
            Defense = double.Parse(section["StartingDefenseValue"]!),
            Points = int.Parse(section["StartingPoints"]!),
            State = WarState.Running
        };
    }

    public StatusModel Status { get; set; }

    public int DefenseCount { get; set; }
    public int DefenseSuccessCount { get; set; }
    public int AttackCount { get; set; }
    public int AttackSuccessCount { get; set; }
    public int AttackStreak { get; set; } = 0;
}