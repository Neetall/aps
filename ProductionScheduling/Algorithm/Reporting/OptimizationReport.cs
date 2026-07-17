using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Optimization.Core;

namespace ProductionScheduling.Algorithm.Reporting;

public class OptimizationReport
{
    public double Score { get; set; }

    public DateTime? EndTime { get; set; }

    public int OperationCount { get; set; }

    public int MachineCount { get; set; }

    public List<MachineReport> Machines { get; set; } = [];
}


public class MachineReport
{
    public string MachineCode { get; set; } = null!;

    public List<OperationReport> Operations { get; set; } = [];
}


public class OperationReport
{
    public string JobTicketCode { get; set; } = null!;

    public int StartSlot { get; set; }

    public int DurationSlots { get; set; }
}