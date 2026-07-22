using ProductionScheduling.Algorithm.Configuration;

public class SchedulingExecutionOptionsDto
{
    public bool EnableOptimization { get; set; }

    public List<OptimizationAlgorithmType> Algorithms { get; set; } = [];
}