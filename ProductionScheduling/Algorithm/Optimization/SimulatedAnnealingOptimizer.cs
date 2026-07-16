using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization;

/// <summary>
/// 模拟退火优化器
///
/// 在局部搜索基础上允许接受一定概率的差解
/// 用于跳出局部最优
/// </summary>
public class SimulatedAnnealingOptimizer : ISolutionOptimizer
{
    private readonly SchedulingResourceIndex resourceIndex;

    private readonly JobTicketIndex jobTicketIndex;

    private readonly OperationSelector operationSelector;

    private readonly MoveSelector moveSelector;

    private readonly SolutionCloner cloner;

    private readonly Random random;

    private readonly int iterations;

    private readonly double initialTemperature;

    private readonly double coolingRate;



    public SimulatedAnnealingOptimizer(
        SchedulingResourceIndex resourceIndex,
        JobTicketIndex jobTicketIndex,
        OperationSelector operationSelector,
        MoveSelector moveSelector,
        SolutionCloner cloner,
        int iterations = 10000,
        double initialTemperature = 1000,
        double coolingRate = 0.95,
        Random? random = null)
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

        this.iterations =
            iterations;

        this.initialTemperature =
            initialTemperature;

        this.coolingRate =
            coolingRate;

        this.random =
            random
            ??
            new Random();
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
        /*
         * 当前状态
         */
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



        /*
         * 保存历史最好解
         */
        var best =
            cloner.Clone(
                current);



        var temperature =
            initialTemperature;



        for(var i = 0; i < iterations; i++)
        {
            /*
             * 创建候选状态
             */
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
                        operation
                };



            var success =
                move.Apply(
                    moveContext);



            if(!success)
            {
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



            var accept =
                Accept(
                    current.Evaluation!.Score,
                    candidate.Evaluation.Score,
                    temperature);



            if(accept)
            {
                current =
                    candidate;
            }



            /*
             * 保存全局最好
             */
            if(current.Evaluation.Score <
               best.Evaluation!.Score)
            {
                best =
                    cloner.Clone(
                        current);
            }



            temperature =
                CoolDown(
                    temperature);



            if(temperature < 0.1)
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



    /// <summary>
    /// 判断是否接受差解
    /// </summary>
    private bool Accept(
        double currentScore,
        double candidateScore,
        double temperature)
    {
        /*
         * 更优直接接受
         */
        if(candidateScore < currentScore)
        {
            return true;
        }



        if(temperature <= 0)
        {
            return false;
        }



        var probability =
            Math.Exp(
                -(candidateScore - currentScore)
                /
                temperature);



        return random.NextDouble()
               <
               probability;
    }



    /// <summary>
    /// 降温
    /// </summary>
    private double CoolDown(
        double temperature)
    {
        return temperature *
               coolingRate;
    }
}