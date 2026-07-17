namespace ProductionScheduling.Algorithm.Reporting;

public static class SchedulingReportPrinter
{
    public static void Print(
        OptimizationReport report)
    {
        Console.WriteLine(
            "========== Scheduling Result ==========");


        Console.WriteLine(
            $"Score: {report.Score}");


        Console.WriteLine(
            $"EndTime: {report.EndTime}");


        Console.WriteLine(
            $"Operations: {report.OperationCount}");


        Console.WriteLine(
            $"Machines: {report.MachineCount}");


        foreach (var machine in report.Machines)
        {
            Console.WriteLine();

            Console.WriteLine(
                $"Machine {machine.MachineCode}");


            foreach (var op in machine.Operations)
                Console.WriteLine(
                    $"  {op.JobTicketCode} " +
                    $"Start:{op.StartSlot} " +
                    $"Duration:{op.DurationSlots}");
        }
    }
}