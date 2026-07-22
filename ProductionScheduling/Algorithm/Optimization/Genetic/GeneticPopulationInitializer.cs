using ProductionScheduling.Algorithm.Scheduling;

namespace ProductionScheduling.Algorithm.Optimization.Genetic;

/// <summary>
/// 遗传算法初始种群生成器
///
/// 种群组成:
/// 1. 当前排产方案作为种子个体
/// 2. 基于种子方案生成扰动个体
/// 3. 生成完全随机个体
/// </summary>
public sealed class GeneticPopulationInitializer
{
    private readonly Random random;

    public GeneticPopulationInitializer()
        : this(Random.Shared)
    {
    }

    public GeneticPopulationInitializer(
        Random random)
    {
        this.random =
            random ??
            throw new ArgumentNullException(
                nameof(random));
    }

    /// <summary>
    /// 创建初始种群
    /// </summary>
    /// <param name="seedSolution">
    /// Greedy或上一阶段生成的初始方案
    /// </param>
    /// <param name="jobTicketCodes">
    /// 需要参与排产的全部派工单编码
    /// </param>
    /// <param name="candidateMachines">
    /// 派工单可选择的候选设备
    ///
    /// JobTicketCode -> MachineCode集合
    /// </param>
    /// <param name="populationSize">
    /// 种群数量
    /// </param>
    public List<GeneticIndividual> Initialize(
        SchedulingSolution seedSolution,
        IReadOnlyCollection<string> jobTicketCodes,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines,
        int populationSize)
    {
        ValidateArguments(
            seedSolution,
            jobTicketCodes,
            candidateMachines,
            populationSize);

        var codes =
            jobTicketCodes
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        var population =
            new List<GeneticIndividual>(
                populationSize);

        var signatures =
            new HashSet<string>(
                StringComparer.Ordinal);

        /*
         * 第一名个体始终保留当前方案。
         */
        var seed =
            CreateSeedIndividual(
                seedSolution,
                codes,
                candidateMachines);

        AddIfUnique(
            population,
            signatures,
            seed);

        /*
         * 约60%的种群基于当前方案扰动生成。
         *
         * 这样既保留Greedy方案附近的搜索能力，
         * 又不会让整个种群完全随机。
         */
        var perturbationTarget =
            Math.Max(
                1,
                (int)Math.Round(
                    populationSize * 0.6));

        var maxAttempts =
            Math.Max(
                populationSize * 20,
                100);

        var attempts =
            0;

        while(
            population.Count < perturbationTarget &&
            population.Count < populationSize &&
            attempts < maxAttempts)
        {
            attempts++;

            var individual =
                CreatePerturbedIndividual(
                    seed,
                    codes,
                    candidateMachines);

            AddIfUnique(
                population,
                signatures,
                individual);
        }

        /*
         * 其余种群使用完全随机染色体。
         */
        attempts = 0;

        while(
            population.Count < populationSize &&
            attempts < maxAttempts)
        {
            attempts++;

            var individual =
                CreateRandomIndividual(
                    codes,
                    candidateMachines);

            AddIfUnique(
                population,
                signatures,
                individual);
        }

        /*
         * 当搜索空间很小，可能无法生成足够多的唯一染色体。
         *
         * 此时允许重复，确保种群数量满足配置要求。
         */
        while(population.Count < populationSize)
        {
            population.Add(
                CreateRandomIndividual(
                    codes,
                    candidateMachines));
        }

        return population;
    }

    /// <summary>
    /// 根据已有排产方案创建种子个体
    /// </summary>
    private GeneticIndividual CreateSeedIndividual(
        SchedulingSolution seedSolution,
        IReadOnlyList<string> jobTicketCodes,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines)
    {
        var individual =
            new GeneticIndividual();

        var operationMap =
            seedSolution
                .Operations
                .GroupBy(
                    x => x.JobTicketCode,
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x
                        .OrderBy(y => y.StartSlot)
                        .First(),
                    StringComparer.OrdinalIgnoreCase);

        /*
         * 根据当前方案的开始时间生成排序基因。
         *
         * 使用连续排名而不是直接使用StartSlot，
         * 可以避免大量相同开始时间造成基因重复。
         */
        var orderedCodes =
            jobTicketCodes
                .OrderBy(code =>
                    operationMap.TryGetValue(
                        code,
                        out var operation)
                        ? operation.StartSlot
                        : int.MaxValue)
                .ThenBy(code =>
                    operationMap.TryGetValue(
                        code,
                        out var operation)
                        ? operation.EndSlot
                        : int.MaxValue)
                .ThenBy(
                    code => code,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        for(var index = 0;
            index < orderedCodes.Count;
            index++)
        {
            var code =
                orderedCodes[index];

            individual.PriorityGenes[code] =
                index;

            if(
                operationMap.TryGetValue(
                    code,
                    out var operation) &&
                IsCandidateMachine(
                    code,
                    operation.MachineCode,
                    candidateMachines))
            {
                individual.MachineGenes[code] =
                    operation.MachineCode;
            }
            else
            {
                individual.MachineGenes[code] =
                    SelectRandomMachine(
                        code,
                        candidateMachines);
            }
        }

        individual.Invalidate();

        return individual;
    }

    /// <summary>
    /// 基于种子个体生成扰动个体
    /// </summary>
    private GeneticIndividual CreatePerturbedIndividual(
        GeneticIndividual seed,
        IReadOnlyList<string> jobTicketCodes,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines)
    {
        var individual =
            seed.CloneGenes();

        if(jobTicketCodes.Count == 0)
            return individual;

        /*
         * 随机交换两个工单的排序基因。
         */
        if(jobTicketCodes.Count >= 2)
        {
            var firstIndex =
                random.Next(
                    jobTicketCodes.Count);

            var secondIndex =
                random.Next(
                    jobTicketCodes.Count - 1);

            if(secondIndex >= firstIndex)
                secondIndex++;

            var firstCode =
                jobTicketCodes[firstIndex];

            var secondCode =
                jobTicketCodes[secondIndex];

            (
                individual.PriorityGenes[firstCode],
                individual.PriorityGenes[secondCode]
            )
            =
            (
                individual.PriorityGenes[secondCode],
                individual.PriorityGenes[firstCode]
            );
        }

        /*
         * 随机修改1到3个工单的设备基因。
         */
        var machineMutationCount =
            Math.Min(
                jobTicketCodes.Count,
                random.Next(
                    1,
                    Math.Min(
                        3,
                        jobTicketCodes.Count) + 1));

        var mutationCodes =
            jobTicketCodes
                .OrderBy(_ =>
                    random.Next())
                .Take(
                    machineMutationCount);

        foreach(var code in mutationCodes)
        {
            individual.MachineGenes[code] =
                SelectRandomMachine(
                    code,
                    candidateMachines,
                    individual.MachineGenes[code]);
        }

        /*
         * 对一个排序基因增加少量随机扰动，
         * 增加种群多样性。
         */
        var priorityCode =
            jobTicketCodes[
                random.Next(
                    jobTicketCodes.Count)];

        individual.PriorityGenes[priorityCode] +=
            random.NextDouble() - 0.5;

        individual.Invalidate();

        return individual;
    }

    /// <summary>
    /// 创建完全随机个体
    /// </summary>
    private GeneticIndividual CreateRandomIndividual(
        IReadOnlyList<string> jobTicketCodes,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines)
    {
        var individual =
            new GeneticIndividual();

        foreach(var code in jobTicketCodes)
        {
            individual.PriorityGenes[code] =
                random.NextDouble();

            individual.MachineGenes[code] =
                SelectRandomMachine(
                    code,
                    candidateMachines);
        }

        individual.Invalidate();

        return individual;
    }

    /// <summary>
    /// 随机选择候选设备
    /// </summary>
    private string SelectRandomMachine(
        string jobTicketCode,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines,
        string? excludedMachineCode = null)
    {
        var machines =
            candidateMachines[
                jobTicketCode]
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if(machines.Count == 0)
        {
            throw new InvalidOperationException(
                $"派工单{jobTicketCode}没有候选设备");
        }

        /*
         * 如果存在其他候选设备，
         * 优先避免选择当前设备。
         */
        var alternatives =
            string.IsNullOrWhiteSpace(
                excludedMachineCode)
                ? machines
                : machines
                    .Where(x =>
                        !string.Equals(
                            x,
                            excludedMachineCode,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();

        var source =
            alternatives.Count > 0
                ? alternatives
                : machines;

        return source[
            random.Next(
                source.Count)];
    }

    private bool IsCandidateMachine(
        string jobTicketCode,
        string machineCode,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines)
    {
        return
            candidateMachines.TryGetValue(
                jobTicketCode,
                out var machines) &&
            machines.Any(x =>
                string.Equals(
                    x,
                    machineCode,
                    StringComparison.OrdinalIgnoreCase));
    }

    private void AddIfUnique(
        ICollection<GeneticIndividual> population,
        ISet<string> signatures,
        GeneticIndividual individual)
    {
        var signature =
            CreateSignature(
                individual);

        if(!signatures.Add(signature))
            return;

        population.Add(
            individual);
    }

    /// <summary>
    /// 生成染色体唯一标识
    /// </summary>
    private string CreateSignature(
        GeneticIndividual individual)
    {
        var priorityPart =
            string.Join(
                "|",
                individual
                    .PriorityGenes
                    .OrderBy(
                        x => x.Key,
                        StringComparer.OrdinalIgnoreCase)
                    .Select(x =>
                        $"{x.Key}:{x.Value:F8}"));

        var machinePart =
            string.Join(
                "|",
                individual
                    .MachineGenes
                    .OrderBy(
                        x => x.Key,
                        StringComparer.OrdinalIgnoreCase)
                    .Select(x =>
                        $"{x.Key}:{x.Value}"));

        return
            $"{priorityPart}#{machinePart}";
    }

    private void ValidateArguments(
        SchedulingSolution seedSolution,
        IReadOnlyCollection<string> jobTicketCodes,
        IReadOnlyDictionary<
            string,
            IReadOnlyList<string>> candidateMachines,
        int populationSize)
    {
        ArgumentNullException.ThrowIfNull(
            seedSolution);

        ArgumentNullException.ThrowIfNull(
            jobTicketCodes);

        ArgumentNullException.ThrowIfNull(
            candidateMachines);

        if(populationSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(populationSize),
                "种群数量必须大于0");
        }

        if(jobTicketCodes.Count == 0)
        {
            throw new ArgumentException(
                "派工单集合不能为空",
                nameof(jobTicketCodes));
        }

        foreach(
            var code in
            jobTicketCodes.Distinct(
                StringComparer.OrdinalIgnoreCase))
        {
            if(string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "派工单编码不能为空",
                    nameof(jobTicketCodes));
            }

            if(
                !candidateMachines.TryGetValue(
                    code,
                    out var machines) ||
                machines == null ||
                machines.Count == 0)
            {
                throw new InvalidOperationException(
                    $"派工单{code}没有配置候选设备");
            }

            if(
                machines.All(
                    string.IsNullOrWhiteSpace))
            {
                throw new InvalidOperationException(
                    $"派工单{code}的候选设备编码全部为空");
            }
        }
    }
}
