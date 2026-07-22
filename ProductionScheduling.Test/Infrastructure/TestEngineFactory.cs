using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Core;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Application;
using ProductionScheduling.Application.Result;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Test.Infrastructure;

public static class TestEngineFactory
{
    /// <summary>
    /// 创建完整排产Engine
    /// </summary>
    public static SchedulingEngine Create(
        SchedulingContext context,
        SchedulingAlgorithmOptions? options = null)
    {
        options ??=
            new SchedulingAlgorithmOptions();



        /*
         * 时间轴
         */
        var timelineInitializer =
            new TimelineInitializer();



        /*
         * 初始排产算法
         */
        var scheduler =
            TestAlgorithmFactory
                .CreateGreedyScheduler(
                    context);



        /*
         * 评价器
         */
        var evaluator =
            new ScheduleEvaluator();



        /*
         * 结果转换
         */
        var resultConverter =
            new SchedulingResultConverter();



        /*
         * 索引
         */
        var resourceIndex =
            TestAlgorithmFactory
                .CreateResourceIndex(
                    context);



        var ticketIndex =
            TestAlgorithmFactory
                .CreateJobTicketIndex(
                    context);


        var debugOptions =
            TestAlgorithmFactory
                .CreateDebugOptions();



        /*
         * Move
         */
        var moveSelector =
            new MoveSelector(
                new Random(1));


        moveSelector.Register(
            new ChangeMachineMove(
                new ScheduleDurationCalculator(),
                debugOptions),
            options.Moves.ChangeMachineWeight);


        moveSelector.Register(
            new ShiftTimeMove(
                debugOptions),
            options.Moves.ShiftTimeWeight);


        moveSelector.Register(
            new SwapOperationMove(
                debugOptions),
            options.Moves.SwapOperationWeight);



        /*
         * LNS组件
         */
        var destroyOperator =
            new RandomDestroyOperator();



        var repairOperator =
            new GreedyRepairOperator(
                resourceIndex,
                ticketIndex,
                new ScheduleDurationCalculator());



        var lnsAcceptance =
            new LnsAcceptance();



        var neighborhoodGenerator =
            new MoveNeighborhoodGenerator(
                moveSelector);



        /*
         * 优化流水线
         */
        var pipelineRunner =
            TestAlgorithmFactory
                .CreatePipelineRunner(
                    context,
                    options,
                    destroyOperator,
                    repairOperator,
                    lnsAcceptance,
                    neighborhoodGenerator);



        return new SchedulingEngine(
            timelineInitializer,
            scheduler,
            evaluator,
            resultConverter,
            pipelineRunner,
            new SchedulingSolutionValidator(),
            ticketIndex,
            resourceIndex,
            options.Effectiveness);
    }
}
