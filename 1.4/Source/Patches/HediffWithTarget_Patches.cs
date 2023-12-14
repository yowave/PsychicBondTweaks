using HarmonyLib;
using RimWorld;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal class HediffWithTarget_Patches
    {
        // we never want to remove Hediff_PsychicBondTorn?
        [HarmonyPatch(typeof(HediffWithTarget), nameof(HediffWithTarget.ShouldRemove), MethodType.Getter)]
        [HarmonyPostfix]
        public static void ShouldRemove_Postfix_Patch(ref bool __result, ref HediffWithTarget __instance)
        {
            if (__instance is Hediff_PsychicBondTorn)
            {
                __result = false;
            }
        }
    }
}
