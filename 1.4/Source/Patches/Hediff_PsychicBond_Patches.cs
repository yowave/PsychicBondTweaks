using HarmonyLib;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal class Hediff_PsychicBond_Patches
    {
        // we never want to automatically remove Hediff_PsychicBond, this way when one of the pawns are dead we call removebond.
        [HarmonyPatch(typeof(Hediff_PsychicBond), nameof(Hediff_PsychicBond.ShouldRemove), MethodType.Getter)]
        [HarmonyPostfix]
        public static void ShouldRemove_Postfix_Patch(ref bool __result, ref Hediff_PsychicBond __instance)
        {
            Pawn pawn = __instance.pawn;
            Pawn target = (Pawn)__instance.target;
            if (pawn.Dead || target.Dead)
            {
                pawn.GetPsychicBondGene()?.RemoveBond();
                target.GetPsychicBondGene()?.RemoveBond();
            }
            __result = false;
        }

        // setting severity incase of [Settings.remove_psychic_distance], this is redundant but oh well :)
        [HarmonyPatch(typeof(Hediff_PsychicBond), nameof(Hediff_PsychicBond.PostTick))]
        [HarmonyPostfix]
        public static void PostTick_Postfix_Patch(ref Hediff_PsychicBond __instance)
        {
            if (Settings.remove_psychic_distance)
            {
                __instance.Severity = 0.5f;
            }
        }
    }
}
