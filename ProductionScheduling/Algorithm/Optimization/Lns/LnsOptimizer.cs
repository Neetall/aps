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



    public LnsOptimizer(
        SolutionCloner cloner,
        IDestroyOperator destroyOperator,
        IRepairOperator repairOperator,
        ILnsAcceptance acceptance,
        SchedulingSolutionValidator validator,
        LnsOptions options)
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
    }



    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContext timeline,
        ScheduleEvaluator evaluator)
    {
        var current =
            new LnsState
            {
                Solution =
                    cloner.CloneSolution(
                        solution),

                Timeline =
                    timeline,

                Evaluation =
                    evaluator.Evaluate(
                        solution,
                        timeline,
                        context)
            };



        var best =
            cloner.Clone(
                current);



        for(var i = 0;
            i < options.Iterations;
            i++)
        {
            var candidate =
                cloner.Clone(
                    current);



            /*
             * Destroy
             *
             * 删除部分工单
             * 同时释放设备资源
             */
            var removed =
                destroyOperator.Destroy(
                    candidate.Solution,
                    candidate.Timeline,
                    options.DestroyRate);



            if(removed.Count == 0)
                continue;



            /*
             * Repair
             *
             * 重新插入工单
             */
            repairOperator.Repair(
                candidate.Solution,
                candidate.Timeline,
                removed);



            /*
             * 验证Solution合法性
             *
             * 检查:
             * 1. JobTicket重复
             * 2. 设备存在
             * 3. 时间范围
             * 4. 设备任务冲突
             */
            validator.Validate(
                candidate.Solution,
                candidate.Timeline);



            /*
             * 重新评价
             */
            candidate.Evaluation =
                evaluator.Evaluate(
                    candidate.Solution,
                    candidate.Timeline,
                    context);



            /*
             * 接受策略
             */
            if(!acceptance.Accept(
                    current.Evaluation!,
                    candidate.Evaluation))
            {
                continue;
            }



            current =
                candidate;



            /*
             * 更新最优
             */
            if(current.Evaluation!.Score <
               best.Evaluation!.Score)
            {
                best =
                    cloner.Clone(
                        current);
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
}