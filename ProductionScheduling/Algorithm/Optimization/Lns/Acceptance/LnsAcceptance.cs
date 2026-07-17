using ProductionScheduling.Algorithm.Optimization.Lns.Core;

namespace ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;

public class LnsAcceptance : ILnsAcceptance
{
    private readonly double worseAcceptRate;

    private readonly Random random;



    public LnsAcceptance(
        double worseAcceptRate = 0.05,
        Random? random = null)
    {
        this.worseAcceptRate =
            worseAcceptRate;

        this.random =
            random ??
            new Random();
    }



    public bool Accept(
        EvaluationResult current,
        EvaluationResult candidate)
    {
        /*
         * 更优直接接受
         */
        if(candidate.Score <
           current.Score)
        {
            return true;
        }


        /*
         * 小概率接受差解
         *
         * 防止陷入局部最优
         */
        return random.NextDouble()
               < worseAcceptRate;
    }
}