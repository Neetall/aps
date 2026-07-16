using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestTimelineFactory
{
    /// <summary>
    /// 创建测试时间轴
    /// </summary>
    public static TimelineContext Create(
        SchedulingContext context)
    {
        var initializer =
            new TimelineInitializer();


        return initializer.Initialize(
            context);
    }
}