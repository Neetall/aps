using ProductionScheduling.Algorithm.Evaluation;
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


    public SchedulingEngine(
        TimelineInitializer timelineInitializer,
        IScheduler scheduler,
        ScheduleEvaluator evaluator,
        SchedulingResultConverter resultConverter,
        OptimizationPipelineRunner pipelineRunner,
        SchedulingSolutionValidator validator)
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

        this.validator =
            validator;
    }


    public SchedulingResult Execute(
        SchedulingContext context)
    {
        try
        {
            /*
             * 1.
             * 初始化资源
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
             */
            validator.Validate(
                solution,
                context,
                timelines);



            /*
             * 4.
             * 可选优化
             */
            if(context.ExecutionOptions.EnableOptimization)
            {
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
                 * 优化后再次校验
                 */
                validator.Validate(
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
             * 转换结果
             */
            var result =
                resultConverter.Convert(
                    solution,
                    timelines,
                    context);


            result.Evaluation =
                evaluation;


            result.Success =
                solution.IsFeasible;


            result.Message =
                context.ExecutionOptions.EnableOptimization
                    ?
                    $"排产完成(已优化)，完工时间:{evaluation.EndTime:yyyy-MM-dd HH:mm},设备利用率:{evaluation.MachineUtilization:P2}"
                    :
                    $"排产完成，完工时间:{evaluation.EndTime:yyyy-MM-dd HH:mm},设备利用率:{evaluation.MachineUtilization:P2}";


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