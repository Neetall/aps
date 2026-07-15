using ProductionScheduling.Domain.Scheduling;

namespace ProductionScheduling.Timeline;

/// <summary>
///     时间轴初始化器
///     负责创建完整的排产时间环境
/// </summary>
public class TimelineInitializer
{
    private readonly TimelineOccupancyBuilder occupancyBuilder;
    private readonly TimelineBuilder timelineBuilder;


    public TimelineInitializer()
    {
        timelineBuilder =
            new TimelineBuilder();


        occupancyBuilder =
            new TimelineOccupancyBuilder();
    }


    public TimelineContext Initialize(
        SchedulingContext context)
    {
        var timelineContext =
            timelineBuilder.Build(context);


        occupancyBuilder.Build(
            timelineContext,
            context.MachineCalendars);


        return timelineContext;
    }
}