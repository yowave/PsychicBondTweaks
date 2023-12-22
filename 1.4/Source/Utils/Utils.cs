using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace PsychicBondTweaks
{
    internal static class Utils
    {
        public static void LogM<T>(T message)
        {
            if (Settings.debug_mode)
            {
                Log.Message($"[{PsychicBondTweaks.ModName}] -> {message}");
            }
        }

        public static void LogE<T>(T message)
        {
            Log.Error($"[{PsychicBondTweaks.ModName}] ERROR -> {message}");
        }
    }
}
