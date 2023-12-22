using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychicBondTweaks
{
    internal static class TimeUtils
    {
        public static readonly int TICKS_PER_HOUR = 2500;
        public static readonly int TICKS_PER_DAY = TICKS_PER_HOUR * 24;

        public static float TicksToHours(this int ticks)
        {
            return (float)ticks / TICKS_PER_HOUR;
        }

        public static float TicksToDays(this int ticks)
        {
            return (float)ticks / TICKS_PER_DAY;
        }

        public static int DaysToTicks(this int days)
        {
            return days * TICKS_PER_DAY;
        }

        public static int TicksDaysHoursFraction(this int ticks)
        {
            float days = ticks.TicksToDays();
            int daysInt = (int)days;
            return (int)((days - daysInt) * TICKS_PER_DAY / TICKS_PER_HOUR);
        }
    }
}
