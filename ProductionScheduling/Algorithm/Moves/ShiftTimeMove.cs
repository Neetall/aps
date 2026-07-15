using ProductionScheduling.Algorithm.Optimization;
using ProductionScheduling.Timeline;

namespace ProductionScheduling.Algorithm.Moves;

/// <summary>
/// 调整派工单开始时间
///
/// 设备不变
///
/// 只改变:
/// StartSlot
/// </summary>
public class ShiftTimeMove : IMove
{
    private ScheduledOperation? operation;


    private int oldStartSlot;



    private readonly Random random;



    public ShiftTimeMove(
        Random? random = null)
    {
        this.random =
            random
            ??
            new Random();
    }



    public string Name =>
        "ShiftTime";



    public bool Apply(
        MoveContext context)
    {
        operation =
            context.CurrentOperation;



        if(operation == null)
        {
            return false;
        }



        oldStartSlot =
            operation.StartSlot;



        if(!context.Timeline.Machines
            .TryGetValue(
                operation.MachineCode,
                out var machineTimeline))
        {
            Clear();

            return false;
        }



        /*
         * 释放旧时间
         */
        machineTimeline.Release(
            operation.StartSlot,
            operation.DurationSlots);



        int newStart;



        /*
         * 随机选择方向
         *
         * 50%前移
         * 50%后移
         */
        if(random.Next(2) == 0)
        {
            newStart =
                FindForward(
                    machineTimeline,
                    operation);
        }
        else
        {
            newStart =
                FindBackward(
                    machineTimeline,
                    operation);
        }



        if(newStart < 0)
        {
            /*
             * 恢复
             */
            machineTimeline.Occupy(
                oldStartSlot,
                operation.DurationSlots);


            Clear();

            return false;
        }



        /*
         * 占用新位置
         */
        machineTimeline.Occupy(
            newStart,
            operation.DurationSlots);



        operation.StartSlot =
            newStart;



        return true;
    }



    public void Undo(
        MoveContext context)
    {
        if(operation == null)
        {
            return;
        }



        if(!context.Timeline.Machines
            .TryGetValue(
                operation.MachineCode,
                out var machineTimeline))
        {
            return;
        }



        /*
         * 释放当前位置
         */
        machineTimeline.Release(
            operation.StartSlot,
            operation.DurationSlots);



        /*
         * 恢复旧位置
         */
        machineTimeline.Occupy(
            oldStartSlot,
            operation.DurationSlots);



        operation.StartSlot =
            oldStartSlot;



        Clear();
    }



    private int FindForward(
        MachineTimeline timeline,
        ScheduledOperation operation)
    {
        return timeline.FindEarliest(
            operation.DurationSlots,
            oldStartSlot + 1);
    }



    private int FindBackward(
        MachineTimeline timeline,
        ScheduledOperation operation)
    {
        for(
            var i = oldStartSlot - 1;
            i >= 0;
            i--)
        {
            if(timeline.CanOccupy(
                   i,
                   operation.DurationSlots))
            {
                return i;
            }
        }


        return -1;
    }



    private void Clear()
    {
        operation = null;

        oldStartSlot = 0;
    }
}