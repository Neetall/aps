using ProductionScheduling.Domain.Resources;

namespace ProductionScheduling.Algorithm.Index;

/// <summary>
///     排产资源索引
///     根据派工单快速查询设备能力
/// </summary>
public class SchedulingResourceIndex
{
    /*
     * JT001
     *
     * [
     *   M001能力,
     *   M002能力
     * ]
     */
    private readonly Dictionary<string, List<MachineCapability>> capabilities =
        new();


    /*
     * JT001_M001
     *
     * MachineCapability
     */
    private readonly Dictionary<(string, string), MachineCapability> capabilityMap =
        new();


    /*
     * JT001
     *
     * [
     *   M001,
     *   M002
     * ]
     */
    private readonly Dictionary<string, List<string>> machineCodes =
        new();


    /// <summary>
    ///     创建索引
    /// </summary>
    public void Build(
        IEnumerable<Machine> machines)
    {
        capabilities.Clear();

        machineCodes.Clear();

        capabilityMap.Clear();


        foreach (var machine in machines)
        foreach (var capability in machine.Capabilities)
        {
            /*
             * JT -> Capability
             */
            if (!capabilities.TryGetValue(
                    capability.JobTicketCode,
                    out var list))
            {
                list =
                    [];

                capabilities.Add(
                    capability.JobTicketCode,
                    list);
            }


            list.Add(
                capability);


            /*
             * JT -> MachineCode
             */
            if (!machineCodes.TryGetValue(
                    capability.JobTicketCode,
                    out var codes))
            {
                codes =
                    [];

                machineCodes.Add(
                    capability.JobTicketCode,
                    codes);
            }


            if (!codes.Contains(
                    capability.MachineCode))
                codes.Add(
                    capability.MachineCode);


            /*
             * JT + Machine
             *
             * -> Capability
             */
            capabilityMap
                [
                    (
                        capability.JobTicketCode,
                        capability.MachineCode
                    )
                ]
                =
                capability;
        }
    }


    /// <summary>
    ///     获取支持该派工单的设备能力
    /// </summary>
    public IReadOnlyList<MachineCapability> GetCapabilities(
        string jobTicketCode)
    {
        if (capabilities.TryGetValue(
                jobTicketCode,
                out var result))
            return result;


        return [];
    }


    /// <summary>
    ///     获取支持该派工单的设备
    /// </summary>
    public IReadOnlyList<string> GetMachineCodes(
        string jobTicketCode)
    {
        if (machineCodes.TryGetValue(
                jobTicketCode,
                out var result))
            return result;


        return [];
    }


    /// <summary>
    ///     获取指定设备能力
    /// </summary>
    public MachineCapability? GetCapability(
        string jobTicketCode,
        string machineCode)
    {
        if (capabilityMap.TryGetValue(
                (
                    jobTicketCode,
                    machineCode
                ),
                out var capability))
            return capability;


        return null;
    }


    /// <summary>
    ///     是否支持该派工单
    /// </summary>
    public bool HasCapability(
        string jobTicketCode)
    {
        return capabilities.ContainsKey(
            jobTicketCode);
    }
}