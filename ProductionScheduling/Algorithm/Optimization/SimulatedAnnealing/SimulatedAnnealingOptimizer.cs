using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;

/// <summary>
/// 模拟退火优化器
///
/// 在局部搜索基础上允许接受一定概率的差解
/// 用于跳出局部最优
/// </summary>
public class SimulatedAnnealingOptimizer : ISolutionOptimizer
{
    private readonly AcceptanceCriteria acceptanceCriteria;

    private readonly SolutionCloner cloner;

    private readonly SimulatedAnnealingOptions options;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly MoveSelector moveSelector;

    private readonly OperationSelector operationSelector;

    private readonly SchedulingResourceIndex resourceIndex;

    private readonly SchedulingSolutionValidator validator;


    public SimulatedAnnealingOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelector moveSelector,
        SolutionCloner cloner,
        AcceptanceCriteria acceptanceCriteria,
        SchedulingSolutionValidator validator,
        SimulatedAnnealingOptions options)
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

        this.acceptanceCriteria =
            acceptanceCriteria;

        this.validator =
            validator;

        this.options =
            options;
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



        var best =
            cloner.Clone(
                current);



        var temperature =
            options.InitialTemperature;



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
                continue;



            var move =
                moveSelector.Select();



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
                        operation,

                    ExecutionRecord =
                        null
                };



            var oldScore =
                current.Evaluation!.Score;



            var success =
                move.Apply(
                    moveContext);



            if(!success)
            {
                candidate.History.Add(
                    new MoveExecutionRecord
                    {
                        MoveName =
                            move.Name,

                        Success =
                            false,

                        JobTicketCode =
                            operation.JobTicketCode
                    });


                temperature =
                    CoolDown(
                        temperature);

                continue;
            }



            if(!IsValid(
                    candidate.Solution,
                    context,
                    candidate.Timelines))
            {
                temperature =
                    CoolDown(
                        temperature);

                continue;
            }



            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timelines,
                    context);



            var newScore =
                candidate.Evaluation.Score;



            var accepted =
                acceptanceCriteria.Accept(
                    oldScore,
                    newScore,
                    temperature);



            var execution =
                moveContext.ExecutionRecord;



            candidate.History.Add(
                new MoveExecutionRecord
                {
                    MoveName =
                        move.Name,

                    Success =
                        true,

                    Accepted =
                        accepted,

                    OldScore =
                        oldScore,

                    NewScore =
                        newScore,

                    JobTicketCode =
                        operation.JobTicketCode,

                    OldMachineCode =
                        execution?.OldMachineCode,

                    NewMachineCode =
                        execution?.NewMachineCode,

                    OldStartSlot =
                        execution?.OldStartSlot
                        ?? 0,

                    NewStartSlot =
                        execution?.NewStartSlot
                        ?? 0,

                    OldDurationSlots =
                        execution?.OldDurationSlots
                        ?? 0,

                    NewDurationSlots =
                        execution?.NewDurationSlots
                        ?? 0
                });



            if(accepted)
            {
                current =
                    candidate;
            }



            if(current.Evaluation!.Score <
               best.Evaluation!.Score)
            {
                best =
                    cloner.Clone(
                        current);
            }



            temperature =
                CoolDown(
                    temperature);



            if(temperature <
               options.MinimumTemperature)
            {
                break;
            }
        }



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
        catch
        {
            return false;
        }
    }



    private double CoolDown(
        double temperature)
    {
        return temperature *
               options.CoolingRate;
    }
}