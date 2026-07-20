namespace ProductionScheduling.Algorithm.Configuration;

public sealed class AlgorithmDebugOptions
{
    /// <summary>
    /// 是否开启算法调试日志
    /// </summary>
    public bool EnableDebugLog { get; set; }


    /// <summary>
    /// 是否输出Move详细日志
    /// </summary>
    public bool EnableMoveLog { get; set; }


    /// <summary>
    /// 是否输出Iteration日志
    /// </summary>
    public bool EnableIterationLog { get; set; }


    /// <summary>
    /// 是否输出Greedy过程日志
    /// </summary>
    public bool EnableSchedulerLog { get; set; }


    /// <summary>
    /// 是否输出Pipeline日志
    /// </summary>
    public bool EnablePipelineLog { get; set; }
}