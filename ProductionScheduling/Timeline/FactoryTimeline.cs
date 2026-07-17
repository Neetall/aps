using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Timeline;

public class FactoryTimeline
{
    private readonly Dictionary<string, MachineTimeline> machines = [];


    public FactoryTimeline(
        string factoryCode,
        ITimeModel timeModel)
    {
        FactoryCode =
            factoryCode;

        TimeModel =
            timeModel;
    }


    public string FactoryCode { get; }


    public ITimeModel TimeModel { get; }


    public IReadOnlyDictionary<string, MachineTimeline> Machines =>
        machines;


    public void AddMachine(
        MachineTimeline machine)
    {
        machines[machine.MachineCode] =
            machine;
    }


    public bool TryGetMachine(
        string machineCode,
        out MachineTimeline machine)
    {
        return machines.TryGetValue(
            machineCode,
            out machine!);
    }
}