namespace ProductionScheduling.Algorithm.Moves.Core;

/// <summary>
///     排产方案移动操作
/// </summary>
public interface IMove
{
    /// <summary>
    ///     移动名称
    /// </summary>
    string Name { get; }


    /// <summary>
    ///     执行移动
    ///     成功后必须写入:
    ///     context.ExecutionRecord
    /// </summary>
    bool Apply(
        MoveContext context);


    /// <summary>
    ///     根据执行记录撤销
    /// </summary>
    void Undo(
        MoveContext context);
}