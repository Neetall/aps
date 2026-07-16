namespace ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;

/// <summary>
///     SA接受准则
/// </summary>
public class AcceptanceCriteria
{
    private readonly Random random;


    public AcceptanceCriteria(
        Random? random = null)
    {
        this.random =
            random ??
            new Random();
    }


    /// <summary>
    ///     判断是否接受新方案
    /// </summary>
    public bool Accept(
        double currentScore,
        double newScore,
        double temperature)
    {
        /*
         * 更优直接接受
         */
        if (newScore < currentScore) return true;


        /*
         * 温度过低
         */
        if (temperature <= 0) return false;


        var probability =
            Math.Exp(
                -(newScore - currentScore)
                /
                temperature);


        return random.NextDouble()
               <
               probability;
    }
}