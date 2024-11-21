namespace App.Schema;

public class WarConfig
{
    public int StartingPoints { get; set; }
    public int StartingAttackValue { get; set; }
    public int StartingDefenseValue { get; set; }
    public int PointsGainedForSuccessfulHack { get; set; }
    public int PointsLostForUnsuccessfulHack { get; set; }
    public int AttackValueGainedForSuccessfulHack { get; set; }
    public int AttackValueLostForUnsuccessfulHack { get; set; }
    public int PointsGainedForSuccessfulDefense { get; set; }
    public int PointsLostForUnsuccessfulDefense { get; set; }
    public int DefenseValueGainedForSuccessfulDefense { get; set; }
    public int DefenseValueLostForUnsuccessfulDefense { get; set; }
    public int NumberOfSuccessfulHacksForExtraDefense { get; set; }
    public int NumberOfDefensePointsGainedForExtraDefense { get; set; }
    public int DisabledStateDurationSeconds { get; set; }
}