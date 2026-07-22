namespace ProductionScheduling.Algorithm.Configuration;

public sealed class OptimizationEffectivenessOptions
{
    public double MinimumScoreImprovement { get; set; } = 1;

    public double MinimumScoreImprovementRate { get; set; } = 0.001;
}
