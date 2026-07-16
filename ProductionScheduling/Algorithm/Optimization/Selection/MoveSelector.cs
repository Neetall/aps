using ProductionScheduling.Algorithm.Moves.Core;

namespace ProductionScheduling.Algorithm.Optimization.Selection;

/// <summary>
///     Move选择器
///     负责从多个邻域操作中选择一种
/// </summary>
public class MoveSelector
{
    private readonly List<MoveEntry> moves = [];

    private readonly Random random;


    public MoveSelector(
        Random? random = null)
    {
        this.random =
            random
            ??
            new Random();
    }


    /// <summary>
    ///     注册Move
    ///     weight:
    ///     权重越高概率越大
    /// </summary>
    public void Register(
        IMove move,
        int weight = 1)
    {
        if (weight <= 0)
            throw new ArgumentException(
                "权重必须大于0");


        moves.Add(
            new MoveEntry
            {
                Move = move,

                Weight = weight
            });
    }


    /// <summary>
    ///     获取一个随机Move
    /// </summary>
    public IMove Select()
    {
        if (moves.Count == 0)
            throw new InvalidOperationException(
                "没有注册任何Move");


        var totalWeight =
            moves.Sum(x => x.Weight);


        var value =
            random.Next(
                1,
                totalWeight + 1);


        var current = 0;


        foreach (var item in moves)
        {
            current +=
                item.Weight;


            if (value <= current) return item.Move;
        }


        return moves[^1].Move;
    }


    private class MoveEntry
    {
        public IMove Move { get; set; } = null!;


        public int Weight { get; set; }
    }
}