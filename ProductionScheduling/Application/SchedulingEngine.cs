using ProductionScheduling.Algorithm;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization;
using ProductionScheduling.Application.Result;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Application;

/// <summary>
/// 排产引擎入口
/// </summary>
public class SchedulingEngine
{
    private readonly TimelineInitializer timelineInitializer;

    private readonly IScheduler scheduler;

    private readonly ISolutionOptimizer? optimizer;

    private readonly ScheduleEvaluator evaluator;

    private readonly SchedulingResultConverter resultConverter;



    public SchedulingEngine(
        TimelineInitializer timelineInitializer,
        IScheduler scheduler,
        ScheduleEvaluator evaluator,
        SchedulingResultConverter resultConverter,
        ISolutionOptimizer? optimizer = null)
    {
        this.timelineInitializer =
            timelineInitializer;

        this.scheduler =
            scheduler;

        this.evaluator =
            evaluator;

        this.resultConverter =
            resultConverter;

        this.optimizer =
            optimizer;
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
             * 初始化时间资源
             */
            var timeline =
                timelineInitializer
                    .Initialize(context);



            /*
             * 2.
             * 生成初始方案
             *
             * 当前:
             * Greedy
             */
            var solution =
                scheduler.Schedule(
                    context,
                    timeline);



            /*
             * 3.
             * 优化方案
             *
             * 后续:
             * SA
             * GA
             * LNS
             */
            if(optimizer != null)
            {
                var optimizeResult =
                    optimizer.Optimize(
                        solution,
                        context,
                        timeline,
                        evaluator);


                solution =
                    optimizeResult.Solution;


                timeline =
                    optimizeResult.Timeline;
            }



            /*
             * 4.
             * 评价最终方案
             */
            var evaluation =
                evaluator.Evaluate(
                    solution,
                    timeline,
                    context);



            /*
             * 5.
             * 转换业务结果
             */
            var result =
                resultConverter.Convert(
                    solution);



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