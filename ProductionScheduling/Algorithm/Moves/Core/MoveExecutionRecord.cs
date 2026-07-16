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


    /// <summary>
    ///     第一个任务编码
    /// </summary>
    public string? JobTicketCode { get; set; }


    /// <summary>
    ///     原设备
    /// </summary>
    public string? OldMachineCode { get; set; }


    /// <summary>
    ///     新设备
    /// </summary>
    public string? NewMachineCode { get; set; }


    /// <summary>
    ///     原开始Slot
    /// </summary>
    public int OldStartSlot { get; set; }


    /// <summary>
    ///     新开始Slot
    /// </summary>
    public int NewStartSlot { get; set; }


    /// <summary>
    ///     原持续Slot数量
    /// </summary>
    public int OldDurationSlots { get; set; }


    /// <summary>
    ///     新持续Slot数量
    /// </summary>
    public int NewDurationSlots { get; set; }


    /*
     * =========================
     * 第二个任务
     * Swap使用
     * =========================
     */


    /// <summary>
    ///     第二个任务编码
    /// </summary>
    public string? SecondJobTicketCode { get; set; }


    /// <summary>
    ///     第二个任务原设备
    /// </summary>
    public string? SecondOldMachineCode { get; set; }


    /// <summary>
    ///     第二个任务新设备
    /// </summary>
    public string? SecondNewMachineCode { get; set; }


    /// <summary>
    ///     第二个任务原开始Slot
    /// </summary>
    public int SecondOldStartSlot { get; set; }


    /// <summary>
    ///     第二个任务新开始Slot
    /// </summary>
    public int SecondNewStartSlot { get; set; }


    /// <summary>
    ///     第二个任务持续Slot数量
    /// </summary>
    public int SecondDurationSlots { get; set; }
}