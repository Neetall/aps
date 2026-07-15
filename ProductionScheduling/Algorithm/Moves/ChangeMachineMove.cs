using ProductionScheduling.Algorithm.Optimization;
using ProductionScheduling.Domain.Orders;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Moves;

/// <summary>
/// 更换加工设备
///
/// 一个派工单整体迁移到另一台设备
///
/// 不拆分工单
/// 整体迁移
/// </summary>
public class ChangeMachineMove : IMove
{
    private readonly ScheduleDurationCalculator durationCalculator;


    private ScheduledOperation? operation;


    private string? oldMachine;


    private int oldStartSlot;


    private int oldDuration;



    public ChangeMachineMove(
        ScheduleDurationCalculator durationCalculator)
    {
        this.durationCalculator =
            durationCalculator;
    }



    public string Name =>
        "ChangeMachine";



    public bool Apply(
        MoveContext context)
    {
        operation =
            context.CurrentOperation;


        if(operation == null)
        {
            return false;
        }



        oldMachine =
            operation.MachineCode;


        oldStartSlot =
            operation.StartSlot;


        oldDuration =
            operation.DurationSlots;



        /*
         * 获取派工单
         */
        var ticket =
            context.JobTicketIndex
                .Get(
                    operation.JobTicketCode);


        if(ticket == null)
        {
            return false;
        }



        /*
         * 获取支持该工单的设备能力
         */
        var capabilities =
            context.ResourceIndex
                .GetCapabilities(
                    operation.JobTicketCode);



        foreach(var capability in capabilities)
        {

            /*
             * 当前设备跳过
             */
            if(capability.MachineCode ==
               oldMachine)
            {
                continue;
            }



            /*
             * 获取新设备时间轴
             */
            if(!context.Timeline.Machines
                .TryGetValue(
                    capability.MachineCode,
                    out var newTimeline))
            {
                continue;
            }



            /*
             * 重新计算加工时间
             */
            var newDuration =
                durationCalculator.Calculate(
                    ticket,
                    capability);



            /*
             * 查找新设备空闲时间
             */
            var newStart =
                newTimeline.FindEarliest(
                    newDuration);



            if(newStart < 0)
            {
                continue;
            }



            /*
             * 获取旧设备时间轴
             */
            if(!context.Timeline.Machines
                .TryGetValue(
                    oldMachine,
                    out var oldTimeline))
            {
                return false;
            }



            /*
             * 释放旧设备
             */
            oldTimeline.Release(
                oldStartSlot,
                oldDuration);



            /*
             * 占用新设备
             */
            newTimeline.Occupy(
                newStart,
                newDuration);



            /*
             * 更新排产结果
             */
            operation.MachineCode =
                capability.MachineCode;


            operation.StartSlot =
                newStart;


            operation.DurationSlots =
                newDuration;



            return true;
        }



        return false;
    }



    /// <summary>
    /// 撤销操作
    ///
    /// SA接受失败方案时使用
    /// </summary>
    public void Undo(
        MoveContext context)
    {
        if(operation == null ||
           oldMachine == null)
        {
            return;
        }



        /*
         * 当前设备释放
         */
        if(context.Timeline.Machines
            .TryGetValue(
                operation.MachineCode,
                out var currentTimeline))
        {
            currentTimeline.Release(
                operation.StartSlot,
                operation.DurationSlots);
        }



        /*
         * 恢复旧设备
         */
        if(context.Timeline.Machines
            .TryGetValue(
                oldMachine,
                out var oldTimeline))
        {
            oldTimeline.Occupy(
                oldStartSlot,
                oldDuration);
        }



        /*
         * 恢复方案
         */
        operation.MachineCode =
            oldMachine;


        operation.StartSlot =
            oldStartSlot;


        operation.DurationSlots =
            oldDuration;



        /*
         * 清理状态
         */
        operation = null;
        oldMachine = null;
    }
}