using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Domain.Scheduling;
using Xunit.Abstractions;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestScheduleSolutionPrinter
{
    public static void Print(
        SchedulingSolution solution)
    {
        PrintInternal(
            Console.WriteLine,
            solution);
    }


    public static void Print(
        ITestOutputHelper output,
        SchedulingSolution solution)
    {
        PrintInternal(
            output.WriteLine,
            solution);
    }


    private static void PrintInternal(
        Action<string> write,
        SchedulingSolution solution)
    {
        write("");

        write("==============================");
        write("Scheduling Solution");
        write("==============================");


        if (solution.Operations.Count == 0)
        {
            write("No Operations");
            return;
        }


        var groups =
            solution.Operations
                .GroupBy(x => x.MachineCode)
                .OrderBy(x => x.Key);


        foreach (var machineGroup in groups)
        {
            write("");

            write(
                $"Machine:{machineGroup.Key}");


            foreach (var operation in machineGroup
                         .OrderBy(x => x.StartSlot))
                write(
                    FormatOperation(operation));
        }
    }


    private static string FormatOperation(
        ScheduledOperation operation)
    {
        var end =
            operation.StartSlot +
            operation.DurationSlots;


        return
            $"  {operation.JobTicketCode} " +
            $"[{operation.StartSlot}-{end}] " +
            $"Duration:{operation.DurationSlots}";
    }
}