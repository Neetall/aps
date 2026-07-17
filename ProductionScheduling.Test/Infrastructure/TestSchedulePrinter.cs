using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using Xunit.Abstractions;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestSchedulePrinter
{
    public static void Print(
        string title,
        OptimizationResult result)
    {
        PrintInternal(
            Console.WriteLine,
            title,
            result);
    }


    public static void Print(
        ITestOutputHelper output,
        string title,
        OptimizationResult result)
    {
        PrintInternal(
            output.WriteLine,
            title,
            result);
    }


    private static void PrintInternal(
        Action<string> write,
        string title,
        OptimizationResult result)
    {
        write("");

        write("==============================");
        write(title);
        write("==============================");


        write("");

        write("Operations:");

        foreach (var operation in result.Solution.Operations)
            write(
                $"Job:{operation.JobTicketCode} " +
                $"Machine:{operation.MachineCode} " +
                $"Start:{operation.StartSlot} " +
                $"Duration:{operation.DurationSlots}");


        PrintEvaluation(
            write,
            result.Evaluation);
    }


    private static void PrintEvaluation(
        Action<string> write,
        EvaluationResult? evaluation)
    {
        write("");

        write("Evaluation:");

        if (evaluation == null)
        {
            write(
                "Evaluation = null");

            return;
        }


        write(
            $"Score:{evaluation.Score}");


        write(
            $"EndTime:{evaluation.EndTime}");


        write(
            $"ProductionHours:{evaluation.ProductionHours}");


        write(
            $"MachineUtilization:{evaluation.MachineUtilization:P2}");


        write(
            $"DelayCount:{evaluation.DelayCount}");
    }
}