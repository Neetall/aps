using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Timeline;

/// <summary>
/// 时间轴初始化器
/// </summary>
public class TimelineInitializer
{
    private readonly TimelineBuilder timelineBuilder;
    private readonly TimelineOccupancyBuilder occupancyBuilder;


    public TimelineInitializer()
    {
        timelineBuilder =
            new TimelineBuilder();

        occupancyBuilder =
            new TimelineOccupancyBuilder();
    }


    /// <summary>
    /// 初始化多工厂时间资源
    /// </summary>
    public TimelineContextGroup Initialize(
        SchedulingContext context)
    {
        var timelines =
            timelineBuilder.Build(
                context);



        occupancyBuilder.Build(
            timelines,
            context.MachineCalendars);



        return timelines;
    }
}