using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Optimization.Pipeline;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Application.Result;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Application;

public class SchedulingEngine
{
    private readonly ScheduleEvaluator evaluator;
    private readonly OptimizationPipelineRunner pipelineRunner;
    private readonly SchedulingResultConverter resultConverter;
    private readonly IScheduler scheduler;
    private readonly SchedulingSolutionValidator validator;
    private readonly TimelineInitializer timelineInitializer;
    private readonly JobTicketIndex jobTicketIndex;
    private readonly SchedulingResourceIndex resourceIndex;

    public SchedulingEngine(
        TimelineInitializer timelineInitializer,
        IScheduler scheduler,
        ScheduleEvaluator evaluator,
        SchedulingResultConverter resultConverter,
        OptimizationPipelineRunner pipelineRunner,
        SchedulingSolutionValidator validator,
        JobTicketIndex jobTicketIndex,
        SchedulingResourceIndex resourceIndex)
    {
        this.timelineInitializer = timelineInitializer;
        this.scheduler = scheduler;
        this.evaluator = evaluator;
        this.resultConverter = resultConverter;
        this.pipelineRunner = pipelineRunner;
        this.validator = validator;
        this.jobTicketIndex = jobTicketIndex;
        this.resourceIndex = resourceIndex;
    }

    public SchedulingResult Execute(
        SchedulingContext context)
    {
        try
        {
            /*
             * 0.
             * 初始化算法索引
             *
             * 所有算法共享:
             * - JobTicket查询
             * - 设备能力查询
             */
            jobTicketIndex.Build(
                context.Orders);

            resourceIndex.Build(
                context.Machines);

            /*
             * 1.
             * 初始化资源Timeline
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

            Console.WriteLine(
                $"Greedy结果数量:{solution.Operations.Count}");

            /*
             * 3.
             * 初始方案校验
             *
             * Validator失败:
             * 不阻断返回
             *
             * 因为:
             * 部分排产结果也有价值
             */
            TryValidate(
                solution,
                context,
                timelines);

            /*
             * 4.
             * 执行优化流水线
             */
            if(context.ExecutionOptions.EnableOptimization)
            {
                Console.WriteLine(
                    $"Engine收到算法:{FormatAlgorithms(context)}");

                var optimizeResult =
                    pipelineRunner.Run(
                        solution,
                        context,
                        timelines,
                        evaluator,
                        context.ExecutionOptions.Algorithms);

                solution =
                    optimizeResult.Solution;

                timelines =
                    optimizeResult.Timelines;

                /*
                 * 优化后再次校验
                 */
                TryValidate(
                    solution,
                    context,
                    timelines);
            }

            /*
             * 5.
             * 最终评价
             */
            var evaluation =
                evaluator.Evaluate(
                    solution,
                    timelines,
                    context);

            /*
             * 6.
             * 转换接口结果
             */
            var result =
                resultConverter.Convert(
                    solution,
                    timelines,
                    context);

            result.Evaluation =
                evaluation;

            foreach(var warning in evaluation.DelayMessages)
            {
                if(!result.Warnings.Contains(
                       warning))
                {
                    result.Warnings.Add(
                        warning);
                }
            }

            /*
             * Success:
             *
             * 表示排产流程正常执行完成
             *
             * 不代表:
             * 所有工单均已排产
             */
            result.Success =
                true;

            /*
             * IsFeasible:
             *
             * 表示是否满足全部排产约束
             */
            result.IsFeasible =
                solution.IsFeasible;

            var delayMessage =
                evaluation.DelayCount > 0
                    ? $",延期订单:{evaluation.DelayCount}"
                    : string.Empty;

            result.Message =
                context.ExecutionOptions.EnableOptimization
                    ? $"排产完成(已优化)，完工时间:{evaluation.EndTime:yyyy-MM-dd HH:mm},设备利用率:{evaluation.MachineUtilization:P2}{delayMessage}"
                    : $"排产完成，完工时间:{evaluation.EndTime:yyyy-MM-dd HH:mm},设备利用率:{evaluation.MachineUtilization:P2}{delayMessage}";

            return result;
        }
        catch(Exception ex)
        {
            return new SchedulingResult
            {
                Success = false,
                IsFeasible = false,
                Message = ex.Message
            };
        }
    }

    private void TryValidate(
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
        }
        catch(Exception ex)
        {
            solution.IsFeasible =
                false;

            Console.WriteLine(
                $"排产校验警告:{ex.Message}");
        }
    }

    private static string FormatAlgorithms(
        SchedulingContext context)
    {
        var algorithms =
            context.ExecutionOptions.Algorithms;

        if(algorithms == null)
            return "未传入";

        if(algorithms.Count == 0)
            return "空集合";

        return string.Join(
            ",",
            algorithms);
    }
}
