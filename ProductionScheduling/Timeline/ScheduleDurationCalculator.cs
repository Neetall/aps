using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Domain.Resources;

namespace ProductionScheduling.Timeline;

/// <summary>
/// 派工单时间计算器
/// 根据设备能力计算占用Slot数量
/// </summary>
public class ScheduleDurationCalculator
{
    /// <summary>
    /// Slot粒度
    /// 单位：分钟
    /// 默认1小时
    /// </summary>
    private readonly int granularityMinutes;



    public ScheduleDurationCalculator(
        int granularityMinutes = 60)
    {
        if(granularityMinutes <= 0)
            throw new ArgumentException(
                "时间粒度必须大于0");


        this.granularityMinutes =
            granularityMinutes;
    }



    /// <summary>
    /// 计算需要占用多少Slot
    /// </summary>
    public int Calculate(
        JobTicket ticket,
        MachineCapability capability)
    {
        if(capability.HourlyCapacity <= 0)
        {
            throw new ArgumentException(
                $"派工单{ticket.Code}产能必须大于0");
        }



        /*
         * 生产时间:
         *
         * 长度 / 米每小时
         */
        var productionHours =
            ticket.Length /
            capability.HourlyCapacity;



        /*
         * 换型时间:
         *
         * 已经属于生产占用
         */
        var setupHours =
            capability.SetupMinutes /
            60.0;



        var totalHours =
            productionHours +
            setupHours;



        return ConvertToSlot(
            totalHours);
    }



    /// <summary>
    /// 小时转换Slot
    /// </summary>
    private int ConvertToSlot(
        double hours)
    {
        var minutes =
            hours * 60;



        /*
         * 规则：
         *
         * 1分钟也占1小时
         *
         * 61分钟=2小时
         */
        return Math.Max(
            1,
            (int)Math.Ceiling(
                minutes /
                granularityMinutes));
    }
}