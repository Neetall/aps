namespace ProductionScheduling.Algorithm.Configuration;

public class LnsOptions
{
    /// <summary>
    /// 迭代次数
    /// </summary>
    public int Iterations { get; set; } = 100;


    /// <summary>
    /// 每次破坏比例
    /// 例如0.2表示移除20%的任务
    /// </summary>
    public double DestroyRate { get; set; } = 0.2;


    /// <summary>
    /// 是否接受差解
    /// 后续可以加入SA概率
    /// </summary>
    public bool AllowWorseMoves { get; set; } = true;
}