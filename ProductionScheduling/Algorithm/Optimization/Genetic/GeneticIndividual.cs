using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.Genetic;

/// <summary>
/// 遗传算法个体
///
/// 染色体:
/// 1. 工单排序基因
/// 2. 设备选择基因
/// </summary>
public sealed class GeneticIndividual
{
    /// <summary>
    /// 工单排序基因
    /// 数值越小，排产优先级越高
    /// </summary>
    public Dictionary<string,double> PriorityGenes { get; }
        = new();

    /// <summary>
    /// 设备选择基因
    /// JobTicketCode -> MachineCode
    /// </summary>
    public Dictionary<string,string> MachineGenes { get; }
        = new();

    /// <summary>
    /// 解码后的排产方案
    /// </summary>
    public SchedulingSolution? Solution { get; set; }

    /// <summary>
    /// 解码后的时间轴
    /// </summary>
    public TimelineContextGroup? Timelines { get; set; }

    /// <summary>
    /// 评价结果
    /// </summary>
    public EvaluationResult? Evaluation { get; set; }

    /// <summary>
    /// 当前Score越小越好，
    /// 因此适应度使用负Score
    /// </summary>
    public double Fitness =>
        Evaluation == null
            ? double.MinValue
            : -Evaluation.Score;

    /// <summary>
    /// 只复制染色体
    ///
    /// 用于:
    /// 交叉
    /// 变异
    /// </summary>
    public GeneticIndividual CloneGenes()
    {
        var clone =
            new GeneticIndividual();

        foreach(var pair in PriorityGenes)
        {
            clone.PriorityGenes[
                pair.Key] =
                pair.Value;
        }

        foreach(var pair in MachineGenes)
        {
            clone.MachineGenes[
                pair.Key] =
                pair.Value;
        }

        return clone;
    }

    /// <summary>
    /// 复制完整个体
    ///
    /// 用于:
    /// 精英保留
    /// 最优个体保存
    /// </summary>
    public GeneticIndividual CloneEvaluated(
        SolutionCloner cloner)
    {
        var clone =
            CloneGenes();

        clone.Solution =
            Solution == null
                ? null
                : cloner.CloneSolution(
                    Solution);

        clone.Timelines =
            Timelines == null
                ? null
                : cloner.CloneTimelines(
                    Timelines);

        clone.Evaluation =
            cloner.CloneEvaluation(
                Evaluation);

        return clone;
    }

    /// <summary>
    /// 清除旧的解码结果
    ///
    /// 基因变化后必须调用
    /// </summary>
    public void Invalidate()
    {
        Solution = null;
        Timelines = null;
        Evaluation = null;
    }
}