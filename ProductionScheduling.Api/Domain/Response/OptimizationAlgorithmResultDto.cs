using ProductionScheduling.Algorithm.Configuration;

namespace ProductionScheduling.Api.Domain.Response;

public sealed class OptimizationAlgorithmResultDto
{
    public OptimizationAlgorithmType Algorithm { get; set; }

    public bool Success { get; set; }

    public bool Accepted { get; set; }

    public bool TimedOut { get; set; }

    public double BeforeScore { get; set; }

    public double? AfterScore { get; set; }

    public double? Improvement { get; set; }

    public double? ImprovementRate { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime EndedAt { get; set; }

    public double ElapsedMilliseconds { get; set; }

    public string? Message { get; set; }
}
