using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Timeline;

public class TimelineContext
{
    private readonly Dictionary<string, MachineTimeline> machines = [];


    public TimelineContext(
        ITimeModel timeModel)
    {
        TimeModel =
            timeModel;
    }


    public ITimeModel TimeModel { get; }


    public int SlotCount =>
        TimeModel.SlotCount;


    public IReadOnlyDictionary<string, MachineTimeline> Machines =>
        machines;


    public void AddMachineTimeline(
        MachineTimeline timeline)
    {
        machines[timeline.MachineCode] =
            timeline;
    }


    public bool TryGetMachine(
        string machineCode,
        out MachineTimeline timeline)
    {
        return machines.TryGetValue(
            machineCode,
            out timeline!);
    }
}