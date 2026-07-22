namespace ProductionScheduling.Application.Result;

public sealed class OptimizationSummary
{
    public bool Attempted { get; set; }

    public bool Effective { get; set; }

    public bool TimedOut { get; set; }

    public double BeforeScore { get; set; }

    public double AfterScore { get; set; }

    public double Improvement { get; set; }

    public double ImprovementRate { get; set; }

    public double MinimumEffectiveImprovement { get; set; }

    public double MinimumEffectiveImprovementRate { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public double ElapsedMilliseconds { get; set; }

    public List<OptimizationAlgorithmResult> AlgorithmResults { get; set; } = [];
}
