namespace ProductionScheduling.Algorithm.Configuration;

public class TabuSearchOptions
{
    public int Iterations { get; set; } = 120;

    public int TabuTenure { get; set; } = 15;

    public bool AllowWorseMoves { get; set; } = true;
}
