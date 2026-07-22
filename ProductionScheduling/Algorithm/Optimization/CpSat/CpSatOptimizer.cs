using Google.OrTools.Sat;
using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Scheduling;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Optimization.CpSat;

public sealed class CpSatOptimizer
    : ISolutionOptimizer
{
    private readonly CpSatOptions options;
    private readonly SchedulingResourceIndex resourceIndex;
    private readonly ScheduleDurationCalculator durationCalculator;
    private readonly TimelineInitializer timelineInitializer;
    private readonly SchedulingSolutionValidator validator;

    public CpSatOptimizer(
        SchedulingAlgorithmOptions options,
        SchedulingResourceIndex resourceIndex,
        ScheduleDurationCalculator durationCalculator,
        TimelineInitializer timelineInitializer,
        SchedulingSolutionValidator validator)
    {
        this.options =
            options.CpSat;

        this.resourceIndex =
            resourceIndex;

        this.durationCalculator =
            durationCalculator;

        this.timelineInitializer =
            timelineInitializer;

        this.validator =
            validator;
    }

    public OptimizationResult Optimize(
        SchedulingSolution solution,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ScheduleEvaluator evaluator)
    {
        var initialEvaluation =
            evaluator.Evaluate(
                solution,
                timelines,
                context);

        Console.WriteLine(
            $"CpSat开始 Score:{initialEvaluation.Score}");

        var cpTimelines =
            timelineInitializer.Initialize(
                context);

        var horizon =
            GetHorizon(
                cpTimelines);

        if(horizon <= 0)
            return Fallback(
                solution,
                timelines,
                initialEvaluation,
                "时间轴为空");

        var tickets =
            context.Orders
                .SelectMany(x =>
                    x.JobTickets)
                .ToList();

        if(tickets.Count == 0)
            return Fallback(
                solution,
                timelines,
                initialEvaluation,
                "没有可排产工单");

        var model =
            new CpModel();

        var variables =
            BuildOperationVariables(
                model,
                tickets,
                cpTimelines);

        if(variables.Count !=
           tickets.Count)
        {
            return Fallback(
                solution,
                timelines,
                initialEvaluation,
                "存在没有可用设备能力的工单");
        }

        AddMachineNoOverlap(
            model,
            variables,
            cpTimelines);

        AddPrecedence(
            model,
            variables,
            context);

        var makespan =
            model.NewIntVar(
                0,
                horizon,
                "makespan");

        model.AddMaxEquality(
            makespan,
            variables.Values.Select(x =>
                x.End));

        var objectiveTerms =
            new List<LinearExpr>
            {
                makespan * options.MakespanWeight
            };

        AddDueDateObjective(
            model,
            variables,
            context,
            cpTimelines,
            objectiveTerms);

        AddInitialSolutionHint(
            model,
            variables,
            solution);

        model.Minimize(
            LinearExpr.Sum(
                objectiveTerms));

        var solver =
            new CpSolver
            {
                StringParameters =
                    BuildSolverParameters()
            };

        var status =
            solver.Solve(
                model);

        Console.WriteLine(
            $"CpSat状态:{status}, Objective:{solver.ObjectiveValue}");

        if(status != CpSolverStatus.Optimal &&
           status != CpSolverStatus.Feasible)
        {
            return Fallback(
                solution,
                timelines,
                initialEvaluation,
                $"未找到可行解:{status}");
        }

        if(options.RequireOptimal &&
           status != CpSolverStatus.Optimal)
        {
            return Fallback(
                solution,
                timelines,
                initialEvaluation,
                $"未证明最优:{status}");
        }

        var decoded =
            DecodeSolution(
                solver,
                variables,
                cpTimelines);

        try
        {
            validator.Validate(
                decoded,
                context,
                cpTimelines);
        }
        catch(Exception ex)
        {
            return Fallback(
                solution,
                timelines,
                initialEvaluation,
                $"求解结果不满足约束:{ex.Message}");
        }

        var evaluation =
            evaluator.Evaluate(
                decoded,
                cpTimelines,
                context);

        Console.WriteLine(
            $"CpSat结束 Score:{evaluation.Score}");

        return new OptimizationResult
        {
            Solution =
                decoded,

            Timelines =
                cpTimelines,

            Evaluation =
                evaluation
        };
    }

    private Dictionary<string,CpSatOperationVariable> BuildOperationVariables(
        CpModel model,
        IReadOnlyList<JobTicket> tickets,
        TimelineContextGroup timelines)
    {
        var result =
            new Dictionary<string,CpSatOperationVariable>(
                StringComparer.OrdinalIgnoreCase);

        foreach(var ticket in tickets)
        {
            var variable =
                CreateOperationVariable(
                    model,
                    ticket,
                    timelines);

            var factory =
                timelines.Get(
                    ticket.FactoryCode);

            foreach(var capability in resourceIndex.GetCapabilities(
                        ticket.Code))
            {
                if(!factory.Machines.ContainsKey(
                       capability.MachineCode))
                {
                    continue;
                }

                var duration =
                    durationCalculator.Calculate(
                        ticket,
                        capability);

                if(duration > factory.TimeModel.SlotCount)
                    continue;

                var presence =
                    model.NewBoolVar(
                        $"{ticket.Code}_{capability.MachineCode}_selected");

                var interval =
                    model.NewOptionalIntervalVar(
                        variable.Start,
                        duration,
                        variable.End,
                        presence,
                        $"{ticket.Code}_{capability.MachineCode}_interval");

                variable.Assignments.Add(
                    new CpSatMachineAssignment
                    {
                        MachineCode =
                            capability.MachineCode,

                        Duration =
                            duration,

                        Presence =
                            presence,

                        Interval =
                            interval
                    });
            }

            if(variable.Assignments.Count == 0)
                continue;

            model.AddExactlyOne(
                variable.Assignments.Select(x =>
                    x.Presence));

            result[ticket.Code] =
                variable;
        }

        return result;
    }

    private CpSatOperationVariable CreateOperationVariable(
        CpModel model,
        JobTicket ticket,
        TimelineContextGroup timelines)
    {
        var factory =
            timelines.Get(
                ticket.FactoryCode);

        return new CpSatOperationVariable
        {
            Ticket =
                ticket,

            Start =
                model.NewIntVar(
                    0,
                    factory.TimeModel.SlotCount,
                    $"{ticket.Code}_start"),

            End =
                model.NewIntVar(
                    0,
                    factory.TimeModel.SlotCount,
                    $"{ticket.Code}_end")
        };
    }

    private void AddMachineNoOverlap(
        CpModel model,
        IReadOnlyDictionary<string,CpSatOperationVariable> variables,
        TimelineContextGroup timelines)
    {
        foreach(var factory in timelines.Factories.Values)
        {
            foreach(var machine in factory.Machines.Values)
            {
                var intervals =
                    variables.Values
                        .SelectMany(x =>
                            x.Assignments)
                        .Where(x =>
                            x.MachineCode ==
                            machine.MachineCode)
                        .Select(x =>
                            x.Interval)
                        .ToList();

                intervals.AddRange(
                    CreateUnavailableIntervals(
                        model,
                        machine));

                if(intervals.Count > 0)
                {
                    model.AddNoOverlap(
                        intervals);
                }
            }
        }
    }

    private IEnumerable<IntervalVar> CreateUnavailableIntervals(
        CpModel model,
        MachineTimeline machine)
    {
        var occupied =
            machine.GetOccupiedSlots()
                .OrderBy(x =>
                    x)
                .ToList();

        if(occupied.Count == 0)
            yield break;

        var start =
            occupied[0];

        var previous =
            occupied[0];

        for(var index = 1;
            index < occupied.Count;
            index++)
        {
            var current =
                occupied[index];

            if(current !=
               previous + 1)
            {
                yield return model.NewFixedSizeIntervalVar(
                    start,
                    previous - start + 1,
                    $"{machine.MachineCode}_unavailable_{start}");

                start =
                    current;
            }

            previous =
                current;
        }

        yield return model.NewFixedSizeIntervalVar(
            start,
            previous - start + 1,
            $"{machine.MachineCode}_unavailable_{start}");
    }

    private void AddPrecedence(
        CpModel model,
        IReadOnlyDictionary<string,CpSatOperationVariable> variables,
        SchedulingContext context)
    {
        foreach(var order in context.Orders)
        {
            var tickets =
                order.JobTickets
                    .OrderBy(x =>
                        x.Sequence)
                    .ThenBy(x =>
                        x.Code,
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();

            for(var index = 1;
                index < tickets.Count;
                index++)
            {
                if(!variables.TryGetValue(
                       tickets[index - 1].Code,
                       out var previous) ||
                   !variables.TryGetValue(
                       tickets[index].Code,
                       out var current))
                {
                    continue;
                }

                model.Add(
                    current.Start >= previous.End);
            }
        }
    }

    private void AddDueDateObjective(
        CpModel model,
        IReadOnlyDictionary<string,CpSatOperationVariable> variables,
        SchedulingContext context,
        TimelineContextGroup timelines,
        ICollection<LinearExpr> objectiveTerms)
    {
        foreach(var order in context.Orders)
        {
            if(order.DueDate == default)
                continue;

            var orderVariables =
                order.JobTickets
                    .Where(x =>
                        variables.ContainsKey(
                            x.Code))
                    .Select(x =>
                        variables[x.Code])
                    .ToList();

            if(orderVariables.Count == 0)
                continue;

            var factory =
                timelines.Get(
                    orderVariables[0].Ticket.FactoryCode);

            var dueSlot =
                GetDueSlot(
                    factory,
                    order.DueDate);

            var orderEnd =
                model.NewIntVar(
                    0,
                    factory.TimeModel.SlotCount,
                    $"{order.Code}_end");

            model.AddMaxEquality(
                orderEnd,
                orderVariables.Select(x =>
                    x.End));

            var tardy =
                model.NewBoolVar(
                    $"{order.Code}_tardy");

            model.Add(
                    orderEnd <= dueSlot)
                .OnlyEnforceIf(
                    tardy.Not());

            model.Add(
                    orderEnd >= dueSlot + 1)
                .OnlyEnforceIf(
                    tardy);

            var tardiness =
                model.NewIntVar(
                    0,
                    factory.TimeModel.SlotCount,
                    $"{order.Code}_tardiness");

            model.AddMaxEquality(
                tardiness,
                [
                    orderEnd - dueSlot,
                    LinearExpr.Constant(0)
                ]);

            objectiveTerms.Add(
                tardy * options.DelayCountWeight);

            objectiveTerms.Add(
                tardiness * options.TardinessWeight);
        }
    }

    private int GetDueSlot(
        FactoryTimeline factory,
        DateTime dueDate)
    {
        var slotCount =
            0;

        for(var slot = 0;
            slot < factory.TimeModel.SlotCount;
            slot++)
        {
            if(factory.TimeModel.GetSlotEnd(
                   slot) <= dueDate)
            {
                slotCount++;
            }
        }

        return slotCount;
    }

    private void AddInitialSolutionHint(
        CpModel model,
        IReadOnlyDictionary<string,CpSatOperationVariable> variables,
        SchedulingSolution solution)
    {
        if(!options.UseInitialSolutionHint)
            return;

        var operationMap =
            solution.Operations
                .GroupBy(
                    x => x.JobTicketCode,
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.First(),
                    StringComparer.OrdinalIgnoreCase);

        foreach(var variable in variables.Values)
        {
            if(!operationMap.TryGetValue(
                   variable.Ticket.Code,
                   out var operation))
            {
                continue;
            }

            model.AddHint(
                variable.Start,
                operation.StartSlot);

            model.AddHint(
                variable.End,
                operation.EndSlot);

            foreach(var assignment in variable.Assignments)
            {
                model.AddHint(
                    assignment.Presence,
                    assignment.MachineCode ==
                    operation.MachineCode);
            }
        }
    }

    private SchedulingSolution DecodeSolution(
        CpSolver solver,
        IReadOnlyDictionary<string,CpSatOperationVariable> variables,
        TimelineContextGroup timelines)
    {
        var solution =
            new SchedulingSolution();

        foreach(var variable in variables.Values)
        {
            var assignment =
                variable.Assignments
                    .First(x =>
                        solver.BooleanValue(
                            x.Presence));

            var start =
                (int)solver.Value(
                    variable.Start);

            var factory =
                timelines.Get(
                    variable.Ticket.FactoryCode);

            var machine =
                factory.Machines[
                    assignment.MachineCode];

            machine.Occupy(
                start,
                assignment.Duration);

            solution.Operations.Add(
                new ScheduledOperation
                {
                    OrderCode =
                        variable.Ticket.OrderCode,

                    FactoryCode =
                        variable.Ticket.FactoryCode,

                    JobTicketCode =
                        variable.Ticket.Code,

                    MachineCode =
                        assignment.MachineCode,

                    StartSlot =
                        start,

                    DurationSlots =
                        assignment.Duration
                });
        }

        solution.IsFeasible =
            true;

        return solution;
    }

    private int GetHorizon(
        TimelineContextGroup timelines)
    {
        return timelines.Factories.Values
            .Select(x =>
                x.TimeModel.SlotCount)
            .DefaultIfEmpty()
            .Max();
    }

    private OptimizationResult Fallback(
        SchedulingSolution solution,
        TimelineContextGroup timelines,
        EvaluationResult evaluation,
        string reason)
    {
        Console.WriteLine(
            $"CpSat回退:{reason}");

        return new OptimizationResult
        {
            Solution =
                solution,

            Timelines =
                timelines,

            Evaluation =
                evaluation
        };
    }

    private string BuildSolverParameters()
    {
        var parameters =
            $"max_time_in_seconds:{options.MaxSolveSeconds} " +
            $"log_search_progress:{options.EnableSolverLog.ToString().ToLowerInvariant()}";

        if(options.WorkerCount > 0)
        {
            parameters +=
                $" num_search_workers:{options.WorkerCount}";
        }

        return parameters;
    }

    private sealed class CpSatOperationVariable
    {
        public JobTicket Ticket { get; init; } = null!;

        public IntVar Start { get; init; } = null!;

        public IntVar End { get; init; } = null!;

        public List<CpSatMachineAssignment> Assignments { get; }
            = [];
    }

    private sealed class CpSatMachineAssignment
    {
        public string MachineCode { get; init; } = string.Empty;

        public int Duration { get; init; }

        public BoolVar Presence { get; init; } = null!;

        public IntervalVar Interval { get; init; } = null!;
    }
}
