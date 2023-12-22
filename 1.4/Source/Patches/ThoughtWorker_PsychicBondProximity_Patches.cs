using HarmonyLib;
using RimWorld;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal class ThoughtWorker_PsychicBondProximity_Patches
    {
        [HarmonyPatch(typeof(ThoughtWorker_PsychicBondProximity), nameof(ThoughtWorker_PsychicBondProximity.NearPsychicBondedPerson))]
        [HarmonyPostfix]
        public static void NearPsychicBondedPerson(ref bool __result)
        {
            if (Settings.remove_psychic_distance)
            {
                __result = true;
            }
        }
    }
}
