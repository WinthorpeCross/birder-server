﻿using System;

namespace Birder.Services
{
    public interface ISystemClockService
    {
        DateTime GetNow { get; }
        DateTime GetToday { get; }
        DateTime GetEndOfToday { get; }
    }

    public class SystemClockService : ISystemClockService
    {
        public DateTime GetNow { get { return DateTime.Now; } }
        public DateTime GetToday { get { return DateTime.Today; } }
        public DateTime GetEndOfToday { get { return DateTime.Today.Date.AddDays(1).AddTicks(-1); } }
    }
}
