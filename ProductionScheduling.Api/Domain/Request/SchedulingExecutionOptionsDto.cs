using ProductionScheduling.Algorithm.Configuration;

public class SchedulingExecutionOptionsDto
{
    public bool EnableOptimization { get; set; } = true;

    public List<OptimizationAlgorithmType> Algorithms { get; set; } = [];

    public int TimeoutSeconds { get; set; }
}
