using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Reporting;

public class SchedulingSolutionReporter
{
    public OptimizationReport Build(
        SchedulingSolution solution,
        TimelineContext timeline,
        EvaluationResult? evaluation)
    {
        var report =
            new OptimizationReport
            {
                Score =
                    evaluation?.Score ?? 0,

                EndTime =
                    evaluation?.EndTime,

                OperationCount =
                    solution.Operations.Count
            };


        var groups =
            solution.Operations
                .GroupBy(x =>
                    x.MachineCode);


        foreach(var group in groups)
        {
            var machine =
                new MachineReport
                {
                    MachineCode =
                        group.Key
                };


            foreach(var operation in group.OrderBy(x=>x.StartSlot))
            {
                machine.Operations.Add(
                    new OperationReport
                    {
                        JobTicketCode =
                            operation.JobTicketCode,

                        StartSlot =
                            operation.StartSlot,

                        DurationSlots =
                            operation.DurationSlots
                    });
            }


            report.Machines.Add(
                machine);
        }


        report.MachineCount =
            report.Machines.Count;


        return report;
    }
}