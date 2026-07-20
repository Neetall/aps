using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.LocalSearch;

/// <summary>
/// 局部搜索优化器
///
/// 基于邻域移动不断寻找更优排产方案
/// </summary>
public class LocalSearchOptimizer : ISolutionOptimizer
{
    private readonly SolutionCloner cloner;

    private readonly LocalSearchOptions options;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly MoveSelector moveSelector;

    private readonly OperationSelector operationSelector;

    private readonly SchedulingResourceIndex resourceIndex;

    private readonly SchedulingSolutionValidator validator;

    private readonly AlgorithmDebugOptions debugOptions;


    public LocalSearchOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelector moveSelector,
        SolutionCloner cloner,
        SchedulingSolutionValidator validator,
        LocalSearchOptions options,
        AlgorithmDebugOptions debugOptions)
    {
        this.resourceIndex =
            resourceIndex;

        this.jobTicketIndex =
            jobTicketIndex;

        this.operationSelector =
            operationSelector;

        this.moveSelector =
            moveSelector;

        this.cloner =
            cloner;

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
        var current =
            cloner.Clone(
                new ScheduleState
                {
                    Solution =
                        solution,

                    Timelines =
                        timelines
                });



        current.Evaluation =
            evaluator.Evaluate(
                current.Solution,
                current.Timelines,
                context);



        PipelineLog(
            $"LocalSearch开始 Score:{current.Evaluation.Score}");



        var acceptCount =
            0;


        var failMoveCount =
            0;



        for(var i = 0;
            i < options.Iterations;
            i++)
        {
            var candidate =
                cloner.Clone(
                    current);



            var operation =
                operationSelector.Select(
                    candidate.Solution);



            if(operation == null)
            {
                IterationLog(
                    $"Iteration:{i}, 无可选Operation");

                continue;
            }



            var move =
                moveSelector.Select();



            IterationLog(
                $"Iteration:{i}, " +
                $"Move:{move.GetType().Name}, " +
                $"Operation:{operation.JobTicketCode}, " +
                $"Machine:{operation.MachineCode}");



            var beforeScore =
                current.Evaluation!.Score;



            var moveContext =
                new MoveContext
                {
                    SchedulingContext =
                        context,

                    Solution =
                        candidate.Solution,

                    Timelines =
                        candidate.Timelines,

                    ResourceIndex =
                        resourceIndex,

                    JobTicketIndex =
                        jobTicketIndex,

                    CurrentOperation =
                        operation
                };



            var success =
                move.Apply(
                    moveContext);



            if(!success)
            {
                failMoveCount++;


                IterationLog(
                    $"Move失败:{move.GetType().Name}");

                continue;
            }



            if(!IsValid(
                    candidate.Solution,
                    context,
                    candidate.Timelines))
            {
                IterationLog(
                    $"Move后方案非法:{move.GetType().Name}");

                continue;
            }



            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timelines,
                    context);



            var afterScore =
                candidate.Evaluation.Score;



            IterationLog(
                $"Move结果:{move.GetType().Name}, " +
                $"Before:{beforeScore}, " +
                $"After:{afterScore}");



            if(afterScore <
               beforeScore)
            {
                current =
                    candidate;


                acceptCount++;


                IterationLog(
                    $"接受优化:{move.GetType().Name}");
            }
            else
            {
                IterationLog(
                    $"拒绝优化:{move.GetType().Name}");
            }
        }



        PipelineLog(
            $"LocalSearch结束 " +
            $"Score:{current.Evaluation.Score}, " +
            $"接受次数:{acceptCount}, " +
            $"失败Move:{failMoveCount}");



        return new OptimizationResult
        {
            Solution =
                current.Solution,

            Timelines =
                current.Timelines,

            Evaluation =
                current.Evaluation
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