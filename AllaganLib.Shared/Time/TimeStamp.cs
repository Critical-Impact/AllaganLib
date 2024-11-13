using System;
using System.Globalization;

namespace AllaganLib.Shared.Time;

public readonly struct TimeStamp : IComparable<TimeStamp>, IEquatable<TimeStamp>
{
    public long Time { get; init; }
    
    public static implicit operator long(TimeStamp ts)
        => ts.Time;
    
    public TimeStamp()
        : this(0)
    {
    }
    
    public TimeStamp(long value)
        => this.Time = value;
    
    public static TimeStamp UtcNow
        => new(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    
    public static readonly TimeStamp Epoch
        = new(0);
    
    public static readonly TimeStamp MinValue
        = new(long.MinValue);
    
    public static readonly TimeStamp MaxValue
        = new(long.MaxValue);
    
    public TimeStamp AddMilliseconds(long value)
        => new(this.Time + value);
    
    public TimeStamp AddSeconds(long value)
        => new(this.Time + RealTime.MillisecondsPerSecond * value);
    
    public TimeStamp AddMinutes(long value)
        => new(this.Time + RealTime.MillisecondsPerMinute * value);
    
    public TimeStamp AddHours(long value)
        => new(this.Time + RealTime.MillisecondsPerHour * value);
    
    public TimeStamp AddDays(long value)
        => new(this.Time + RealTime.MillisecondsPerDay * value);
    
    public TimeStamp RoundToSecond()
    {
        var ms = this.Time % RealTime.MillisecondsPerSecond;
        return new TimeStamp(this.Time - ms + (ms * 2 >= RealTime.MillisecondsPerSecond ? 1000 : 0));
    }
    
    public long TotalSeconds
        => this.Time / RealTime.MillisecondsPerSecond;
    
    public long TotalMinutes
        => this.Time / RealTime.MillisecondsPerMinute;
    
    public long TotalHours
        => this.Time / RealTime.MillisecondsPerHour;
    
    public long TotalDays
        => this.Time / RealTime.MillisecondsPerDay;
    
    public int CurrentSecond
        => (int)(this.TotalSeconds % RealTime.SecondsPerMinute);
    
    public int CurrentMinute
        => (int)(this.TotalMinutes % RealTime.MinutesPerHour);
    
    public int CurrentHour
        => (int)(this.TotalHours % RealTime.HoursPerDay);
    
    public int CurrentSecondOfHour
        => (int)(this.TotalSeconds % RealTime.SecondsPerHour);
    
    public int CurrentSecondOfDay
        => (int)(this.TotalSeconds % RealTime.SecondsPerDay);
    
    public int CurrentMinuteOfDay
        => (int)(this.TotalMinutes % RealTime.MinutesPerDay);
    
    public int CompareTo(TimeStamp other)
        => this.Time.CompareTo(other.Time);
    
    public bool Equals(TimeStamp other)
        => this.Time == other.Time;
    
    public override bool Equals(object? obj)
        => obj is TimeStamp other && this.Equals(other);
    
    public override int GetHashCode()
        => this.Time.GetHashCode();
    
    public TimeStamp Max(TimeStamp other)
        => this.Time < other.Time ? other : this;
    
    public TimeStamp Min(TimeStamp other)
        => this.Time < other.Time ? this : other;
    
    public static long operator -(TimeStamp lhs, TimeStamp rhs)
        => lhs.Time - rhs.Time;
    
    public static TimeStamp operator +(TimeStamp lhs, long offset)
        => new(lhs.Time + offset);
    
    public static TimeStamp operator +(long offset, TimeStamp rhs)
        => rhs + offset;
    
    public static TimeStamp operator -(TimeStamp lhs, long offset)
        => new(lhs.Time - offset);
    
    public static bool operator ==(TimeStamp left, TimeStamp right)
        => left.Time == right.Time;
    
    public static bool operator !=(TimeStamp left, TimeStamp right)
        => left.Time != right.Time;
    
    public static bool operator <(TimeStamp left, TimeStamp right)
        => left.Time < right.Time;
    
    public static bool operator <=(TimeStamp left, TimeStamp right)
        => left.Time <= right.Time;
    
    public static bool operator >(TimeStamp left, TimeStamp right)
        => left.Time > right.Time;
    
    public static bool operator >=(TimeStamp left, TimeStamp right)
        => left.Time >= right.Time;
    
    public DateTime LocalTime
        => DateTimeOffset.FromUnixTimeMilliseconds(this.Time).LocalDateTime;
    
    public DateTime DateTime
        => DateTimeOffset.FromUnixTimeMilliseconds(this.Time).UtcDateTime;
    
    public override string ToString()
        => this.LocalTime.ToString(CultureInfo.InvariantCulture);
}