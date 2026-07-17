using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Pipeline;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Application.Result;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Application;

/// <summary>
/// 排产引擎入口
/// </summary>
public class SchedulingEngine
{
    private readonly ScheduleEvaluator evaluator;
    private readonly OptimizationPipelineRunner pipelineRunner;
    private readonly SchedulingResultConverter resultConverter;
    private readonly IScheduler scheduler;
    private readonly TimelineInitializer timelineInitializer;


    public SchedulingEngine(
        TimelineInitializer timelineInitializer,
        IScheduler scheduler,
        ScheduleEvaluator evaluator,
        SchedulingResultConverter resultConverter,
        OptimizationPipelineRunner pipelineRunner)
    {
        this.timelineInitializer =
            timelineInitializer;

        this.scheduler =
            scheduler;

        this.evaluator =
            evaluator;

        this.resultConverter =
            resultConverter;

        this.pipelineRunner =
            pipelineRunner;
    }


    /// <summary>
    /// 执行排产
    /// </summary>
    public SchedulingResult Execute(
        SchedulingContext context)
    {
        try
        {
            /*
             * 1.
             * 初始化多工厂时间资源
             */
            var timelines =
                timelineInitializer.Initialize(
                    context);



            /*
             * 2.
             * 初始排产
             */
            var solution =
                scheduler.Schedule(
                    context,
                    timelines);



            /*
             * 3.
             * 优化
             */
            var optimizeResult =
                pipelineRunner.Run(
                    solution,
                    context,
                    timelines,
                    evaluator);



            solution =
                optimizeResult.Solution;


            timelines =
                optimizeResult.Timelines;



            /*
             * 4.
             * 最终评价
             */
            var evaluation =
                evaluator.Evaluate(
                    solution,
                    timelines,
                    context);



            /*
             * 5.
             * 转业务结果
             */
            var result =
                resultConverter.Convert(
                    solution,
                    timelines,
                    context);



            result.Evaluation =
                evaluation;


            result.Message =
                $"排产完成，完工时间:{evaluation.EndTime:yyyy-MM-dd HH:mm},设备利用率:{evaluation.MachineUtilization:P2}";


            result.Success =
                solution.IsFeasible;


            return result;
        }
        catch(Exception ex)
        {
            return new SchedulingResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}