using ProductionScheduling.Algorithm.Configuration;

namespace ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;

/// <summary>
///     SA接受准则
/// </summary>
public class AcceptanceCriteria
{
    private readonly Random random;

    private readonly AcceptanceOptions options;



    public AcceptanceCriteria(
        AcceptanceOptions options,
        Random? random = null)
    {
        this.options =
            options;


        this.random =
            random
            ??
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
        if(newScore < currentScore)
        {
            return true;
        }



        /*
         * 温度过低
         */
        if(temperature <= 0)
        {
            return false;
        }



        var delta =
            (newScore - currentScore)
            *
            options.ScoreScale;



        var probability =
            Math.Exp(
                -delta /
                temperature);



        /*
         * 限制最大概率
         */
        probability =
            Math.Min(
                probability,
                options.MaximumProbability);



        return random.NextDouble()
               <
               probability;
    }
}