using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Scheduling;
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



    public SimulatedAnnealingOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelector moveSelector,
        SolutionCloner cloner,
        AcceptanceCriteria acceptanceCriteria,
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

        this.options =
            options;
    }



    /// <summary>
    /// 执行模拟退火
    /// </summary>
    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContext timeline,
        ScheduleEvaluator evaluator)
    {
        var current =
            cloner.Clone(
                new ScheduleState
                {
                    Solution =
                        solution,

                    Timeline =
                        timeline
                });



        current.Evaluation =
            evaluator.Evaluate(
                current.Solution,
                current.Timeline,
                context);



        var best =
            cloner.Clone(
                current);



        var temperature =
            options.InitialTemperature;



        for(var i = 0; i < options.Iterations; i++)
        {
            var candidate =
                cloner.Clone(
                    current);



            var operation =
                operationSelector.Select(
                    candidate.Solution);



            if(operation == null)
            {
                continue;
            }



            var move =
                moveSelector.Select();



            var moveContext =
                new MoveContext
                {
                    SchedulingContext =
                        context,

                    Solution =
                        candidate.Solution,

                    Timeline =
                        candidate.Timeline,

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



            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timeline,
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

            Timeline =
                best.Timeline,

            Evaluation =
                best.Evaluation
        };
    }



    private double CoolDown(
        double temperature)
    {
        return temperature *
               options.CoolingRate;
    }
}