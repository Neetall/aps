using ProductionScheduling.Algorithm.Optimization;

namespace ProductionScheduling.Algorithm.Moves;

/// <summary>
/// 排产方案移动操作
/// </summary>
public interface IMove
{
    /// <summary>
    /// 移动名称
    /// </summary>
    string Name { get; }



    /// <summary>
    /// 执行移动
    /// </summary>
    bool Apply(
        MoveContext context);



    /// <summary>
    /// 撤销移动
    /// </summary>
    void Undo(
        MoveContext context);
}