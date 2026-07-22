namespace ProductionScheduling.Algorithm.Configuration;

public class TabuSearchOptions
{
    public int Iterations { get; set; } = 300;

    public int TabuTenure { get; set; } = 25;

    public bool AllowWorseMoves { get; set; } = true;

    public int MaxNeighborhoodSize { get; set; } = 45;

    public int NoImprovementLimit { get; set; } = 60;
}
