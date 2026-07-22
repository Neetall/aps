# ProductionScheduling API Test Payloads

启动 API 后访问：

- Swagger UI: `http://localhost:5120/swagger`
- OpenAPI JSON: `http://localhost:5120/openapi/v1.json`
- 排产接口: `POST http://localhost:5120/api/scheduling/execute`

建议测试顺序：

1. `00-greedy-baseline-ga-machine-choice.json`
2. `01-genetic-algorithm-beats-greedy-machine-choice.json`
3. `02-greedy-baseline-move-due-date-swap.json`
4. `03-local-search-due-date-swap.json`
5. `04-simulated-annealing-due-date-swap.json`
6. `05-tabu-search-due-date-swap.json`
7. `06-lns-due-date-repair.json`
8. `07-due-date-warning-soft-constraint.json`
9. `08-operation-crosses-break-without-preemption.json`
10. `09-cpsat-machine-choice.json`
11. `10-real-scale-30-orders-60-machines-no-ga-cpsat.json`

说明：

- `GaMachineChoice` 场景体现 GA 能通过 `PriorityGenes + MachineGenes` 重建整张排程，找到贪心拿不到的机器组合。
- `MoveDueDateSwap` 场景体现 Move-based 算法可以通过交换同设备工序降低延期惩罚。
- LNS 当前是基础 destroy/greedy repair，适合作为观察破坏-修复流水线的测试入口；它不一定每次都优于 Greedy。
- `DueDateWarning` 场景用于确认交期是软约束：实在无法满足时仍返回方案，并在 `message`、`warnings`、`evaluation.delayMessages` 中说明延期。
- `OperationCrossesBreak` 场景用于确认单个子订单可以跨休息时间，但不会被其他子订单插入；响应里的 `shiftPeriods` 会按实际工作时间拆段。
- `CpSatMachineChoice` 场景用于确认 CP-SAT 以数学求解方式选择机器和开始时间。
- 当前默认优化流水线是 `Greedy + CpSat`：`enableOptimization=true` 且 `algorithms=[]` 时，会在 Greedy 初始解后执行 CP-SAT。
- LocalSearch、SimulatedAnnealing、Tabu、Lns、GeneticAlgorithm 仍可在 `algorithms` 中显式传入开启。
