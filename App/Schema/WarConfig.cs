namespace App.Schema;

public class WarConfig
{
    public int StartingPoints { get; set; } = 20;
    public int StartingAttackValue { get; set; } = 10;
    public int StartingDefenseValue { get; set; } = 10;
    public int PointsGainedForSuccessfulHack { get; set; } = 1;
    public int PointsLostForUnsuccessfulHack { get; set; } = 1;
    public int AttackValueGainedForSuccessfulHack { get; set; } = 1;
    public int AttackValueLostForUnsuccessfulHack { get; set; } = 1;
    public int PointsGainedForSuccessfulDefense { get; set; } = 1;
    public int PointsLostForUnsuccessfulDefense { get; set; } = 1;
    public int DefenseValueGainedForSuccessfulDefense { get; set; } = 1;
    public int DefenseValueLostForUnsuccessfulDefense { get; set; } = 1;
    public int NumberOfSuccessfulHacksForExtraDefense { get; set; } = 3;
    public int NumberOfDefensePointsGainedForExtraDefense { get; set; } = 1;
    public int DisabledStateDurationSeconds { get; set; } = 5;
}