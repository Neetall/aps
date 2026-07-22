using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Scheduling;

public sealed class SchedulePlacementService
{
    private readonly ScheduleDurationCalculator durationCalculator;
    private readonly SchedulingResourceIndex resourceIndex;

    public SchedulePlacementService(
        ScheduleDurationCalculator durationCalculator,
        SchedulingResourceIndex resourceIndex)
    {
        this.durationCalculator =
            durationCalculator;

        this.resourceIndex =
            resourceIndex;
    }

    public MachineScheduleCandidate? FindBestMachine(
        JobTicket ticket,
        TimelineContextGroup timelines,
        int earliestStartSlot)
    {
        MachineScheduleCandidate? best =
            null;

        var factoryTimeline =
            timelines.Get(
                ticket.FactoryCode);

        var capabilities =
            resourceIndex
                .GetCapabilities(
                    ticket.Code);

        foreach(var capability in capabilities)
        {
            var candidate =
                CreateCandidate(
                    ticket,
                    factoryTimeline,
                    capability.MachineCode,
                    earliestStartSlot);

            if(candidate == null)
                continue;

            if(best == null ||
               candidate.EndSlot < best.EndSlot)
            {
                best =
                    candidate;
            }
        }

        return best;
    }

    public MachineScheduleCandidate? FindPreferredMachine(
        JobTicket ticket,
        TimelineContextGroup timelines,
        string? preferredMachineCode,
        int earliestStartSlot)
    {
        if(string.IsNullOrWhiteSpace(
               preferredMachineCode))
        {
            return FindBestMachine(
                ticket,
                timelines,
                earliestStartSlot);
        }

        var factoryTimeline =
            timelines.Get(
                ticket.FactoryCode);

        var candidate =
            CreateCandidate(
                ticket,
                factoryTimeline,
                preferredMachineCode,
                earliestStartSlot);

        return candidate ??
               FindBestMachine(
                   ticket,
                   timelines,
                   earliestStartSlot);
    }

    public void Commit(
        SchedulingSolution solution,
        MachineScheduleCandidate candidate)
    {
        solution.Operations.Add(
            candidate.Operation);

        candidate.MachineTimeline.Occupy(
            candidate.Operation.StartSlot,
            candidate.Operation.DurationSlots);
    }

    private MachineScheduleCandidate? CreateCandidate(
        JobTicket ticket,
        FactoryTimeline factoryTimeline,
        string machineCode,
        int earliestStartSlot)
    {
        var capability =
            resourceIndex.GetCapability(
                ticket.Code,
                machineCode);

        if(capability == null)
            return null;

        if(!factoryTimeline.Machines.TryGetValue(
               machineCode,
               out var machineTimeline))
        {
            return null;
        }

        var duration =
            durationCalculator.Calculate(
                ticket,
                capability);

        var startSlot =
            factoryTimeline.TimeModel
                .FindEarliestAvailable(
                    machineTimeline,
                    duration,
                    earliestStartSlot);

        if(startSlot < 0)
            return null;

        return new MachineScheduleCandidate
        {
            MachineTimeline =
                machineTimeline,

            EndSlot =
                startSlot + duration,

            Operation =
                new ScheduledOperation
                {
                    OrderCode =
                        ticket.OrderCode,

                    FactoryCode =
                        ticket.FactoryCode,

                    JobTicketCode =
                        ticket.Code,

                    MachineCode =
                        machineCode,

                    StartSlot =
                        startSlot,

                    DurationSlots =
                        duration
                }
        };
    }
}
