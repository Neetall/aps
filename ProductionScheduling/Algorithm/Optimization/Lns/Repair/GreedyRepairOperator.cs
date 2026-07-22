using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Time;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns.Repair;

public class GreedyRepairOperator : IRepairOperator
{
    private readonly SchedulingResourceIndex resourceIndex;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly ScheduleDurationCalculator durationCalculator;


    public GreedyRepairOperator(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        ScheduleDurationCalculator durationCalculator)
    {
        this.resourceIndex =
            resourceIndex;

        this.jobTicketIndex =
            jobTicketIndex;

        this.durationCalculator =
            durationCalculator;
    }


    public void Repair(
        SchedulingSolution solution,
        TimelineContextGroup timelines,
        List<ScheduledOperation> removed)
    {
        foreach(var operation in SortForRepair(
                    removed))
        {
            var ticket =
                jobTicketIndex.Get(
                    operation.JobTicketCode);


            if(ticket == null)
                continue;



            var factory =
                timelines.Get(
                    ticket.FactoryCode);



            var capabilities =
                resourceIndex.GetCapabilities(
                    ticket.Code);

            var earliestStart =
                GetEarliestStartByPrecedence(
                    solution,
                    ticket.OrderCode,
                    ticket.Sequence);



            RepairCandidate? best =
                null;



            foreach(var capability in capabilities)
            {
                if(!factory.TryGetMachine(
                        capability.MachineCode,
                        out var machine))
                    continue;



                var duration =
                    durationCalculator.Calculate(
                        ticket,
                        capability);



                var start =
                    factory.TimeModel
                        .FindEarliestAvailable(
                            machine,
                            duration,
                            earliestStart);



                if(start < 0)
                    continue;



                var end =
                    start +
                    duration;



                if(best == null ||
                   end < best.End)
                {
                    best =
                        new RepairCandidate
                        {
                            MachineCode =
                                capability.MachineCode,

                            Start =
                                start,

                            Duration =
                                duration,

                            End =
                                end
                        };
                }
            }



            if(best == null)
                continue;



            var machineTimeline =
                factory.Machines[
                    best.MachineCode];



            if(!machineTimeline.CanOccupy(
                    best.Start,
                    best.Duration))
            {
                continue;
            }



            machineTimeline.Occupy(
                best.Start,
                best.Duration);



            operation.FactoryCode =
                ticket.FactoryCode;


            operation.MachineCode =
                best.MachineCode;


            operation.StartSlot =
                best.Start;


            operation.DurationSlots =
                best.Duration;



            solution.Operations.Add(
                operation);
        }
    }

    private List<ScheduledOperation> SortForRepair(
        IEnumerable<ScheduledOperation> operations)
    {
        return operations
            .OrderBy(x =>
                jobTicketIndex.Get(
                    x.JobTicketCode)
                    ?.OrderCode
                ?? x.OrderCode,
                StringComparer.OrdinalIgnoreCase)
            .ThenBy(x =>
                jobTicketIndex.Get(
                    x.JobTicketCode)
                    ?.Sequence
                ?? int.MaxValue)
            .ThenBy(x =>
                x.JobTicketCode,
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private int GetEarliestStartByPrecedence(
        SchedulingSolution solution,
        string orderCode,
        int sequence)
    {
        var previous =
            solution.Operations
                .Select(x =>
                    new
                    {
                        Operation = x,
                        Ticket = jobTicketIndex.Get(
                            x.JobTicketCode)
                    })
                .Where(x =>
                    x.Ticket != null &&
                    x.Ticket.OrderCode == orderCode &&
                    x.Ticket.Sequence < sequence)
                .OrderByDescending(x =>
                    x.Ticket!.Sequence)
                .FirstOrDefault();

        return previous?.Operation.EndSlot
               ?? 0;
    }



    private class RepairCandidate
    {
        public string MachineCode { get; set; } = null!;


        public int Start { get; set; }


        public int Duration { get; set; }


        public int End { get; set; }
    }
}
