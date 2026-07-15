namespace ProductionScheduling.Algorithm;


using ProductionScheduling.Timeline;


internal class MachineScheduleCandidate
{

    public MachineTimeline MachineTimeline {get;set;}


    public ScheduledOperation Operation {get;set;}


    public int EndSlot {get;set;}

}