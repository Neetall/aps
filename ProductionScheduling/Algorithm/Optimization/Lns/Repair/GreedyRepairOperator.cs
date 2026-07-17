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
        TimelineContext timeline,
        List<ScheduledOperation> removed)
    {
        foreach(var operation in removed)
        {
            var ticket =
                jobTicketIndex.Get(
                    operation.JobTicketCode);



            if(ticket == null)
                continue;



            var capabilities =
                resourceIndex.GetCapabilities(
                    ticket.Code);



            RepairCandidate? best =
                null;



            foreach(var capability in capabilities)
            {
                if(!timeline.Machines
                    .TryGetValue(
                        capability.MachineCode,
                        out var machine))
                    continue;



                var duration =
                    durationCalculator.Calculate(
                        ticket,
                        capability);


                var start =
                    timeline.TimeModel
                        .FindEarliestAvailable(
                            machine,
                            duration);


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



            /*
             * 占用新的时间
             */
            var machineTimeline =
                timeline.Machines[
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



            /*
             * 更新原Operation
             *
             * 不创建新的
             */
            operation.MachineCode =
                best.MachineCode;


            operation.StartSlot =
                best.Start;


            operation.DurationSlots =
                best.Duration;



            /*
             * 放回Solution
             */
            solution.Operations.Add(
                operation);
        }
    }



    private class RepairCandidate
    {
        public string MachineCode { get; set; } = null!;


        public int Start { get; set; }


        public int Duration { get; set; }


        public int End { get; set; }
    }
}