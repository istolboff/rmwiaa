﻿using System;

namespace RemindMeWhenIamAt.SharedCode
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; } = string.Empty;

        public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);
    }
}
