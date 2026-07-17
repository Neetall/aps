using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
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
         * 优化流水线
         */
        var pipelineRunner =
            TestAlgorithmFactory
                .CreatePipelineRunner(
                    context,
                    options);



        return new SchedulingEngine(
            timelineInitializer,
            scheduler,
            evaluator,
            resultConverter,
            pipelineRunner);
    }
}