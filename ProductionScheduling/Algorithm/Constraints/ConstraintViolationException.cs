namespace ProductionScheduling.Algorithm.Constraints;

public sealed class ConstraintViolationException
    : InvalidOperationException
{
    public ConstraintViolationException(
        string constraintName,
        string message)
        : base(
            $"[{constraintName}]{message}")
    {
        ConstraintName =
            constraintName;
    }

    public string ConstraintName { get; }
}
