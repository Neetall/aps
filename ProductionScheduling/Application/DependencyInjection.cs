using Microsoft.Extensions.DependencyInjection;
using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Moves.Implementations;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.CpSat;
using ProductionScheduling.Algorithm.Optimization.Factory;
using ProductionScheduling.Algorithm.Optimization.Genetic;
using ProductionScheduling.Algorithm.Optimization.Lns;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.core;
using ProductionScheduling.Algorithm.Optimization.Lns.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Pipeline;
using ProductionScheduling.Algorithm.Optimization.Selection;
using ProductionScheduling.Algorithm.Optimization.SimulatedAnnealing;
using ProductionScheduling.Algorithm.Optimization.Tabu;
using ProductionScheduling.Algorithm.Scheduling;
using ProductionScheduling.Algorithm.Validation;
using ProductionScheduling.Application;
using ProductionScheduling.Application.Result;
using ProductionScheduling.Timeline;

public static class DependencyInjection
{
    public static IServiceCollection AddProductionScheduling(
        this IServiceCollection services)
    {
        RegisterConfiguration(services);
        RegisterTimeline(services);
        RegisterCalculation(services);
        RegisterIndexes(services);
        RegisterScheduling(services);
        RegisterEvaluation(services);
        RegisterValidation(services);
        RegisterOptimizationCore(services);
        RegisterMoves(services);
        RegisterMoveSelection(services);
        RegisterOptimizers(services);
        RegisterApplication(services);

        return services;
    }

    private static void RegisterConfiguration(
        IServiceCollection services)
    {
        services.AddSingleton(
            new AlgorithmDebugOptions
            {
                EnableDebugLog = false,
                EnableMoveLog = false,
                EnableIterationLog = false,
                EnableSchedulerLog = false,
                EnablePipelineLog = false
            });

        /*
         * 只创建一份总配置。
         * 各子配置均从总配置中获取，
         * 避免出现两套不同的配置实例。
         */
        services.AddSingleton<SchedulingAlgorithmOptions>();

        services.AddScoped<GeneticPopulationInitializer>();
        
        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .LocalSearch);

        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .SimulatedAnnealing);

        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .TabuSearch);

        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .Lns);

        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .GeneticAlgorithm);

        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .CpSat);

        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .Acceptance);

        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .Moves);

        services.AddSingleton(
            provider =>
                provider
                    .GetRequiredService<SchedulingAlgorithmOptions>()
                    .Evaluation);
    }

    private static void RegisterTimeline(
        IServiceCollection services)
    {
        services.AddScoped<TimelineInitializer>();
    }

    private static void RegisterCalculation(
        IServiceCollection services)
    {
        services.AddScoped<ScheduleDurationCalculator>(
            _ =>
                new ScheduleDurationCalculator(60));
    }

    private static void RegisterIndexes(
        IServiceCollection services)
    {
        services.AddScoped<JobTicketIndex>();
        services.AddScoped<SchedulingResourceIndex>();
    }

    private static void RegisterScheduling(
        IServiceCollection services)
    {
        services.AddScoped<SchedulePlacementService>();

        services.AddScoped<IScheduler>(
            provider =>
                new GreedyScheduler(
                    provider.GetRequiredService<SchedulingResourceIndex>(),
                    provider.GetRequiredService<SchedulePlacementService>(),
                    provider.GetRequiredService<AlgorithmDebugOptions>()));
    }

    private static void RegisterEvaluation(
        IServiceCollection services)
    {
        services.AddScoped(
            provider =>
                new ScheduleEvaluator(
                    provider.GetRequiredService<EvaluationScoreOptions>()));
    }

    private static void RegisterValidation(
        IServiceCollection services)
    {
        services.AddScoped<SchedulingSolutionValidator>();
    }

    private static void RegisterOptimizationCore(
        IServiceCollection services)
    {
        services.AddScoped<SolutionCloner>();
    }

    private static void RegisterMoves(
        IServiceCollection services)
    {
        services.AddScoped<ChangeMachineMove>();
        services.AddScoped<ShiftTimeMove>();
        services.AddScoped<SwapOperationMove>();
    }

    private static void RegisterMoveSelection(
        IServiceCollection services)
    {
        services.AddScoped<OperationSelector>();

        services.AddScoped<MoveSelector>(
            provider =>
            {
                var selector =
                    new MoveSelector();

                selector.Register(
                    provider.GetRequiredService<ChangeMachineMove>(),
                    5);

                selector.Register(
                    provider.GetRequiredService<ShiftTimeMove>(),
                    3);

                selector.Register(
                    provider.GetRequiredService<SwapOperationMove>(),
                    2);

                return selector;
            });

        services.AddScoped<MoveNeighborhoodGenerator>();
    }

    private static void RegisterOptimizers(
        IServiceCollection services)
    {
        /*
         * Local Search
         */
        services.AddScoped<LocalSearchOptimizer>();

        /*
         * Simulated Annealing
         */
        services.AddScoped<AcceptanceCriteria>();
        services.AddScoped<AnnealingState>();
        services.AddScoped<SimulatedAnnealingOptimizer>();

        /*
         * Tabu Search
         */
        services.AddScoped<TabuMemory>(
            _ =>
                new TabuMemory(100));

        services.AddScoped<TabuSearchOptimizer>();

        /*
         * LNS
         */
        services.AddScoped<LnsState>();

        services.AddScoped<IDestroyOperator,
            RandomDestroyOperator>();

        services.AddScoped<IDestroyOperator,
            WorstScoreDestroyOperator>();

        services.AddScoped<IRepairOperator,
            GreedyRepairOperator>();

        services.AddScoped<ILnsAcceptance,
            LnsAcceptance>();

        services.AddScoped<LnsOptimizer>();

        /*
         * Genetic Algorithm
         */
        services.AddScoped<GeneticDecoder>();
        services.AddScoped<TournamentSelection>();
        services.AddScoped<OrderCrossover>();
        services.AddScoped<GeneticMutation>();
        services.AddScoped<GeneticAlgorithmOptimizer>();

        /*
         * CP-SAT
         */
        services.AddScoped<CpSatOptimizer>();

        /*
         * Optimizer Factory
         */
        services.AddScoped<OptimizerFactory>();

        /*
         * Optimization Pipeline
         */
        services.AddScoped<OptimizationPipelineRunner>();
    }

    private static void RegisterApplication(
        IServiceCollection services)
    {
        services.AddScoped<SchedulingResultConverter>();
        services.AddScoped<SchedulingEngine>();
    }
}
