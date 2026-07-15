namespace ProductionScheduling.Timeline;

/// <summary>
///     时间轴构建异常
/// </summary>
public class TimelineBuildException : Exception
{
    public TimelineBuildException(string message)
        : base(message)
    {
    }
}