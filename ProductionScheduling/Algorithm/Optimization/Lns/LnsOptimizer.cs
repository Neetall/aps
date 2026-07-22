using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.core;
using ProductionScheduling.Algorithm.Optimization.Lns.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Lns;

public class LnsOptimizer : ISolutionOptimizer
{
    private readonly SolutionCloner cloner;

    private readonly IDestroyOperator destroyOperator;

    private readonly IRepairOperator repairOperator;

    private readonly ILnsAcceptance acceptance;

    private readonly SchedulingSolutionValidator validator;

    private readonly LnsOptions options;

    private readonly AlgorithmDebugOptions debugOptions;



    public LnsOptimizer(
        SolutionCloner cloner,
        IDestroyOperator destroyOperator,
        IRepairOperator repairOperator,
        ILnsAcceptance acceptance,
        SchedulingSolutionValidator validator,
        LnsOptions options,
        AlgorithmDebugOptions debugOptions)
    {
        this.cloner =
            cloner;

        this.destroyOperator =
            destroyOperator;

        this.repairOperator =
            repairOperator;

        this.acceptance =
            acceptance;

        this.validator =
            validator;

        this.options =
            options;

        this.debugOptions =
            debugOptions;
    }



    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ScheduleEvaluator evaluator)
    {
        var currentSolution =
            cloner.CloneSolution(
                solution);


        var currentTimelines =
            cloner.CloneTimelines(
                timelines);



        var current =
            new LnsState
            {
                Solution =
                    currentSolution,

                Timelines =
                    currentTimelines,

                Evaluation =
                    evaluator.Evaluate(
                        currentSolution,
                        currentTimelines,
                        context)
            };



        PipelineLog(
            $"LNS开始 Score:{current.Evaluation.Score}");



        var best =
            cloner.Clone(
                current);



        var acceptCount =
            0;


        var invalidCount =
            0;


        var bestCount =
            0;

        var noImprovementCount =
            0;



        for(var i = 0;
            i < options.Iterations;
            i++)
        {
            var candidate =
                cloner.Clone(
                    current);



            var removed =
                destroyOperator.Destroy(
                    candidate.Solution,
                    candidate.Timelines,
                    context,
                    options.DestroyRate);



            if(removed.Count == 0)
            {
                Debug(
                    $"Iteration:{i}, Destroy没有移除任务");

                continue;
            }



            IterationLog(
                $"Iteration:{i}, Destroy数量:{removed.Count}");



            repairOperator.Repair(
                candidate.Solution,
                candidate.Timelines,
                removed);



            if(!IsValid(
                    candidate.Solution,
                    context,
                    candidate.Timelines))
            {
                invalidCount++;


                Debug(
                    $"Iteration:{i}, Repair后方案非法");

                continue;
            }



            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timelines,
                    context);



            IterationLog(
                $"Iteration:{i}, " +
                $"Before:{current.Evaluation!.Score}, " +
                $"After:{candidate.Evaluation.Score}");



            if(!acceptance.Accept(
                    current.Evaluation!,
                    candidate.Evaluation))
            {
                IterationLog(
                    $"Iteration:{i}, 拒绝候选方案");

                continue;
            }



            current =
                candidate;


            acceptCount++;



            IterationLog(
                $"Iteration:{i}, 接受候选方案");



            if(current.Evaluation!.Score <
               best.Evaluation!.Score)
            {
                best =
                    cloner.Clone(
                        current);


                bestCount++;

                noImprovementCount =
                    0;


                PipelineLog(
                    $"LNS发现更优方案 Score:{best.Evaluation.Score}");
            }
            else
            {
                noImprovementCount++;

                if(noImprovementCount >=
                   options.NoImprovementLimit)
                {
                    PipelineLog(
                        $"LNS提前结束，无改善轮数:{noImprovementCount}");

                    break;
                }
            }
        }



        PipelineLog(
            $"LNS结束 " +
            $"Score:{best.Evaluation.Score}, " +
            $"接受:{acceptCount}, " +
            $"最优次数:{bestCount}, " +
            $"非法:{invalidCount}");



        return new OptimizationResult
        {
            Solution =
                best.Solution,

            Timelines =
                best.Timelines,

            Evaluation =
                best.Evaluation
        };
    }



    private bool IsValid(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines)
    {
        try
        {
            validator.Validate(
                solution,
                context,
                timelines);

            return true;
        }
        catch(Exception ex)
        {
            Debug(
                $"Validator失败:{ex.Message}");

            return false;
        }
    }



    private void PipelineLog(
        string message)
    {
        if(debugOptions.EnablePipelineLog)
        {
            Console.WriteLine(
                message);
        }
    }



    private void IterationLog(
        string message)
    {
        if(debugOptions.EnableIterationLog)
        {
            Console.WriteLine(
                message);
        }
    }



    private void Debug(
        string message)
    {
        if(debugOptions.EnableDebugLog)
        {
            Console.WriteLine(
                message);
        }
    }
}
