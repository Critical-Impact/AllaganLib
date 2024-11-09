using System;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Action = System.Action;

namespace AllaganLib.Shared.Time;

public class SeTime : ISeTime
{
    private readonly IFramework _framework;
    
    private static TimeStamp GetServerTime()
        => new(Framework.GetServerTime() * 1000);
    
    public TimeStamp ServerTime { get; private set; }
    
    public TimeStamp EorzeaTime { get; private set; }
    
    public long EorzeaTotalMinute { get; private set; }
    
    public long EorzeaTotalHour { get; private set; }
    
    public short EorzeaMinuteOfDay { get; private set; }
    
    public byte EorzeaHourOfDay { get; private set; }
    
    public byte EorzeaMinuteOfHour { get; private set; }
    
    public event Action? Updated;
    
    public event Action? HourChanged;
    
    public event Action? WeatherChanged;
    
    public SeTime(IFramework framework)
    {
        this._framework = framework;
        this.Update(null!);
        this._framework.Update += this.Update;
    }
    
    public void Dispose()
        => this._framework.Update -= this.Update;
    
    private unsafe TimeStamp GetEorzeaTime()
    {
        var framework = Framework.Instance();
        if (framework == null)
            return this.ServerTime.ConvertToEorzea();
        
        return Math.Abs(new TimeStamp(framework->UtcTime.Timestamp * 1000) - this.ServerTime) < 5000
            ? new TimeStamp(framework->ClientTime.EorzeaTime * 1000)
            : this.ServerTime.ConvertToEorzea();
    }
    
    private void Update(IFramework _)
    {
        this.ServerTime = GetServerTime();
        this.EorzeaTime = this.GetEorzeaTime();
        var minute = this.EorzeaTime.TotalMinutes;
        if (minute != this.EorzeaTotalMinute)
        {
            this.EorzeaTotalMinute = minute;
            this.EorzeaMinuteOfDay = (short)(this.EorzeaTotalMinute % RealTime.MinutesPerDay);
            this.EorzeaMinuteOfHour = (byte)(this.EorzeaMinuteOfDay % RealTime.MinutesPerHour);
        }
        
        var hour = this.EorzeaTotalMinute / RealTime.MinutesPerHour;
        if (hour != this.EorzeaTotalHour)
        {
            this.EorzeaTotalHour = hour;
            this.EorzeaHourOfDay = (byte)(this.EorzeaMinuteOfDay / RealTime.MinutesPerHour);
            this.HourChanged?.Invoke();
        }
        
        this.Updated?.Invoke();
    }
}