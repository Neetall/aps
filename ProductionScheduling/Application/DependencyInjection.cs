using Microsoft.Extensions.DependencyInjection;

using ProductionScheduling.Algorithm.Calculation;
using ProductionScheduling.Algorithm.Configuration;
using ProductionScheduling.Algorithm.Evaluation;
using ProductionScheduling.Algorithm.Index;
using ProductionScheduling.Algorithm.Optimization.Core;
using ProductionScheduling.Algorithm.Optimization.Factory;
using ProductionScheduling.Algorithm.Optimization.LocalSearch;
using ProductionScheduling.Algorithm.Optimization.Lns;
using ProductionScheduling.Algorithm.Optimization.Lns.Acceptance;
using ProductionScheduling.Algorithm.Optimization.Lns.core;
using ProductionScheduling.Algorithm.Optimization.Lns.Core;
using ProductionScheduling.Algorithm.Optimization.Lns.Destroy;
using ProductionScheduling.Algorithm.Optimization.Lns.Repair;
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
        /*
         * =========================
         * Configuration
         * =========================
         */

        services.AddSingleton(
            new SchedulingAlgorithmOptions());


        services.AddSingleton(
            new LocalSearchOptions());


        services.AddSingleton(
            new SimulatedAnnealingOptions());


        services.AddSingleton(
            new TabuSearchOptions());


        services.AddSingleton(
            new LnsOptions());


        services.AddSingleton(
            new AcceptanceOptions());


        services.AddSingleton(
            new MoveOptions());



        /*
         * =========================
         * Timeline
         * =========================
         */

        services.AddScoped<TimelineInitializer>();



        /*
         * =========================
         * Calculation
         * =========================
         */

        services.AddScoped<ScheduleDurationCalculator>(
            _ =>
                new ScheduleDurationCalculator(60));



        /*
         * =========================
         * Index
         * =========================
         */

        services.AddScoped<SchedulingResourceIndex>();

        services.AddScoped<JobTicketIndex>();



        /*
         * =========================
         * Scheduler
         * =========================
         */

        services.AddScoped<IScheduler, GreedyScheduler>();



        /*
         * =========================
         * Evaluation
         * =========================
         */

        services.AddScoped<ScheduleEvaluator>();



        /*
         * =========================
         * Validation
         * =========================
         */

        services.AddScoped<SchedulingSolutionValidator>();



        /*
         * =========================
         * Optimization Core
         * =========================
         */

        services.AddScoped<SolutionCloner>();



        /*
         * =========================
         * Move Selection
         * =========================
         */

        services.AddScoped<OperationSelector>();

        services.AddScoped<MoveSelector>();

        services.AddScoped<MoveSelectorFactory>();

        services.AddScoped<MoveNeighborhoodGenerator>();



        /*
         * =========================
         * Local Search
         * =========================
         */

        services.AddScoped<LocalSearchOptimizer>();



        /*
         * =========================
         * Simulated Annealing
         * =========================
         */

        services.AddScoped<AcceptanceCriteria>();

        services.AddScoped<AnnealingState>();

        services.AddScoped<SimulatedAnnealingOptimizer>();



        /*
         * =========================
         * Tabu
         * =========================
         */

        services.AddScoped<TabuMemory>(
            _ =>
                new TabuMemory(100));


        services.AddScoped<TabuSearchOptimizer>();



        /*
         * =========================
         * LNS
         * =========================
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
         * =========================
         * Optimizer Factory
         * =========================
         */

        services.AddScoped<OptimizerFactoryContext>();

        services.AddScoped<OptimizerFactory>();


        services.AddScoped<Func<
            OptimizationAlgorithmType,
            ISolutionOptimizer>>(provider =>
        {
            return type =>
            {
                return type switch
                {
                    OptimizationAlgorithmType.LocalSearch =>
                        provider.GetRequiredService<
                            LocalSearchOptimizer>(),


                    OptimizationAlgorithmType.SimulatedAnnealing =>
                        provider.GetRequiredService<
                            SimulatedAnnealingOptimizer>(),


                    OptimizationAlgorithmType.Tabu =>
                        provider.GetRequiredService<
                            TabuSearchOptimizer>(),


                    OptimizationAlgorithmType.Lns =>
                        provider.GetRequiredService<
                            LnsOptimizer>(),


                    _ =>
                        throw new NotSupportedException(
                            $"不支持优化算法:{type}")
                };
            };
        });


        /*
         * =========================
         * Optimization Pipeline
         * =========================
         */

        services.AddScoped<OptimizationPipelineRunner>();



        /*
         * =========================
         * Result
         * =========================
         */

        services.AddScoped<SchedulingResultConverter>();



        /*
         * =========================
         * Application
         * =========================
         */

        services.AddScoped<SchedulingEngine>();


        return services;
    }
}