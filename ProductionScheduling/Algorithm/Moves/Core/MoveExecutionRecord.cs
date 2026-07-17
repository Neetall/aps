namespace ProductionScheduling.Algorithm.Moves.Core;

/// <summary>
///     Move执行记录
///     保存一次邻域移动前后的状态
///     用于:
///     1. Undo
///     2. Simulated Annealing接受判断
///     3. Tabu记录
///     4. Move统计
/// </summary>
public class MoveExecutionRecord
{
    /// <summary>
    ///     Move名称
    /// </summary>
    public string MoveName { get; set; } = string.Empty;


    /// <summary>
    ///     Tabu唯一标识
    ///     用于TabuSearch判断禁忌
    /// </summary>
    public string? TabuKey { get; set; }


    /// <summary>
    ///     是否执行成功
    /// </summary>
    public bool Success { get; set; }


    /// <summary>
    ///     是否被优化器接受
    /// </summary>
    public bool Accepted { get; set; }


    /// <summary>
    ///     移动前评分
    /// </summary>
    public double OldScore { get; set; }


    /// <summary>
    ///     移动后评分
    /// </summary>
    public double NewScore { get; set; }


    /*
     * =========================
     * 第一个任务
     * =========================
     */


    public string? JobTicketCode { get; set; }


    public string? OldMachineCode { get; set; }


    public string? NewMachineCode { get; set; }


    public int OldStartSlot { get; set; }


    public int NewStartSlot { get; set; }


    public int OldDurationSlots { get; set; }


    public int NewDurationSlots { get; set; }


    /*
     * =========================
     * 第二个任务
     * Swap使用
     * =========================
     */


    public string? SecondJobTicketCode { get; set; }


    public string? SecondOldMachineCode { get; set; }


    public string? SecondNewMachineCode { get; set; }


    public int SecondOldStartSlot { get; set; }


    public int SecondNewStartSlot { get; set; }


    public int SecondDurationSlots { get; set; }
}