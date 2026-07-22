using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Scheduling;

public class MachineScheduleCandidate
{
    public MachineTimeline MachineTimeline { get; set; }


    public ScheduledOperation Operation { get; set; }


    public int EndSlot { get; set; }
}
