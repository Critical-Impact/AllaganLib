using System;
using System.Linq;
using Lumina.Excel.Sheets;

namespace AllaganLib.Shared.Time;

//Credit to Ottermandias
public readonly struct BitfieldUptime : IEquatable<BitfieldUptime>
{
    private const uint BadHour = RealTime.HoursPerDay + 1;
    private const uint AllHoursValue = 0x00FFFFFF;
    private readonly uint _hours; // bitfield, 0-23 for each hour.

    public static readonly BitfieldUptime AllHours = new(AllHoursValue);

    public static BitfieldUptime Combine(BitfieldUptime lhs, BitfieldUptime rhs)
        => new(lhs._hours | rhs._hours);

    public bool Equals(BitfieldUptime rhs)
        => this._hours == rhs._hours;

    public bool AlwaysUp()
        => this._hours == AllHoursValue;

    public bool IsUp(uint hour)
    {
        if (hour >= RealTime.HoursPerDay)
            hour %= RealTime.HoursPerDay;
        return ((this._hours >> (int)hour) & 1) == 1;
    }

    public uint FirstHour
        => Bits.TrailingZeroCount(this._hours);

    public uint EndHour
        => Bits.HighestSetBit(this._hours) + 1;

    // Returns null if the time is not continuous.
    public (uint, uint)? StartAndEnd()
    {
        var startHour = this.FirstHour;
        var endHour = this.EndHour;
        var count = this.Count;
        if (startHour + count == endHour)
            return (startHour, endHour);

        startHour = Bits.HighestSetBit(~this._hours & AllHoursValue) + 1;
        endHour = Bits.TrailingZeroCount(~this._hours);
        if ((startHour + count) % RealTime.HoursPerDay == endHour)
            return (startHour, endHour);

        return null;
    }

    public uint Count
        => Bits.Popcount(this._hours);

    public BitfieldUptime Overlap(BitfieldUptime rhs)
        => new(this._hours & rhs._hours);

    public bool Overlaps(BitfieldUptime rhs)
        => this.Overlap(rhs)._hours != 0;

    private static uint NextTime(int currentHour, uint hours)
    {
        if (hours == 0)
            return BadHour;

        var rotatedHours = currentHour == 0 ? hours : (hours >> currentHour) | ((hours << (32 - currentHour)) >> 8);
        return Bits.TrailingZeroCount(rotatedHours);
    }

    public uint NextUptime(int currentHour)
        => NextTime(currentHour, this._hours);

    public uint NextDowntime(int currentHour)
        => NextTime(currentHour, ~this._hours & AllHoursValue);

    public TimeInterval NextUptime(TimeStamp now)
    {
        var hour = now.CurrentEorzeaHour();
        var nextUptime = this.NextUptime(hour);
        var nextDowntime = this.NextDowntime((int)(hour + nextUptime) % RealTime.HoursPerDay);
        if (nextUptime == BadHour)
            return TimeInterval.Never;
        if (nextDowntime == BadHour)
            return TimeInterval.Always;

        now = now.SyncToEorzeaHour();
        if (nextUptime == 0)
            return new TimeInterval(now, now.AddEorzeaHours(nextDowntime));

        now = now.AddEorzeaHours(nextUptime);
        return new TimeInterval(now, now.AddEorzeaHours(nextDowntime));
    }

    // Print a string of 24 '0' or '1' as uptimes.
    public string UptimeTable()
        => new(Convert.ToString(this._hours, 2).PadLeft(RealTime.HoursPerDay, '0').Reverse().ToArray());

    // Print hours in human readable format.
    public string PrintHours(bool simple = false, string simpleSeparator = "|")
    {
        var ret = "";
        int min = -1, max = -1;

        var hours = this.StartAndEnd();
        if (hours != null)
        {
            var (start, end) = hours.Value;
            return simple ? $"{start:D2}-{end:D2}" : $"{start:D2}:00 - {end:D2}:00 ET";
        }

        void AddString()
        {
            if (min < 0)
                return;

            if (ret.Length > 0)
            {
                if (simple)
                {
                    ret += simpleSeparator;
                }
                else
                {
                    ret = ret.Replace(" and ", ", ");
                    ret += " and ";
                }
            }

            if (simple)
                ret += $"{min:D2}-{max + 1:D2}";
            else
                ret += $"{min:D2}:00 - {max + 1:D2}:00 ET";

            min = -1;
            max = -1;
        }

        for (var i = 0u; i < RealTime.HoursPerDay; ++i)
        {
            if (this.IsUp(i))
                if (min < 0)
                    min = (int)i;
                else
                    max = (int)i;
            else
                AddString();
        }

        AddString();

        return ret;
    }

    // Convert the ephemeral time as given by the table to a bitfield.
    private static uint ConvertFromEphemeralTime(ushort start, ushort end)
    {
        // Up at all times
        if (start == end || start > 2400 || end > 2400)
            return AllHoursValue;

        var ret = 0u;
        start /= 100;
        end /= 100;

        if (end < start)
            end += RealTime.HoursPerDay;

        for (int i = start; i < end; ++i)
            ret |= 1u << (i % RealTime.HoursPerDay);
        return ret;
    }

    private BitfieldUptime(uint hours)
        => this._hours = hours;

    public BitfieldUptime(ushort start, ushort end)
        => this._hours = ConvertFromEphemeralTime(start, end);

    public static BitfieldUptime FromHours(uint startHour, uint endHour)
    {
        var hours = 0u;
        startHour %= RealTime.HoursPerDay;
        endHour %= RealTime.HoursPerDay;
        if (endHour == startHour)
            return AllHours;

        if (endHour < startHour)
        {
            for (; startHour < RealTime.HoursPerDay; ++startHour)
                hours |= 1u << (int)startHour;
            for (startHour = 0; startHour < endHour; ++startHour)
                hours |= 1u << (int)startHour;
        }
        else
        {
            for (; startHour < endHour; ++startHour)
                hours |= 1u << (int)startHour;
        }

        return new BitfieldUptime(hours);
    }

    // Convert the rare pop time given by the table to a bitfield.
    public BitfieldUptime(GatheringRarePopTimeTable table)
    {
        this._hours = 0;

        // Convert the time slots to ephemeral format to reuse that function.
        for (var index = 0; index < table.StartTime.Count; index++)
        {
            var tableStartTime = table.StartTime[index];
            var tableDuration = table.Duration[index];
            if (tableDuration == 0)
            {
                continue;
            }

            var duration = tableDuration == 160 ? (ushort)200 : tableDuration;
            var end = (ushort)((tableStartTime + duration) % 2400);
            this._hours |= ConvertFromEphemeralTime(tableStartTime, end);
        }
    }
}