using ProductionScheduling.Algorithm.Evaluation;

namespace ProductionScheduling.Algorithm.Optimization.Lns.Core;

public interface ILnsAcceptance
{
    bool Accept(
        EvaluationResult current,
        EvaluationResult candidate);
}