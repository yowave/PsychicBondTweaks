using HarmonyLib;
using RimWorld;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal class Hediff_PsychicBondTorn_Patches
    {
        // patching getter for Visible, so the hediff will be visible on the pawn
        [HarmonyPatch(typeof(Hediff_PsychicBondTorn), nameof(Hediff_PsychicBondTorn.Visible), MethodType.Getter)]
        [HarmonyPostfix]
        public static void Visible_Postfix_Patch(ref bool __result)
        {
            __result = true;
        }

        // skipping the original
        // remove tick condition to make sure this hediff will never be removed with time, so as to always be able to rebond when resurrected
        [HarmonyPatch(typeof(Hediff_PsychicBondTorn), nameof(Hediff_PsychicBondTorn.Notify_Resurrected))]
        [HarmonyPrefix]
        public static bool Notify_Resurrected_Prefix_Patch(ref Hediff_PsychicBondTorn __instance)
        {
            Utils.LogM($"+++ Notify_Resurrected_Prefix_Patch +++");
            Pawn pawn = __instance.pawn;
            Pawn target = __instance.target as Pawn;
            if (target is not null && !target.Dead && !target.Destroyed)
            {
                Gene_PsychicBonding gene_PsychicBonding = pawn.genes?.GetFirstGeneOfType<Gene_PsychicBonding>();
                if (gene_PsychicBonding != null)
                {
                    gene_PsychicBonding.BondTo(target);
                }
                else
                {
                    target.genes?.GetFirstGeneOfType<Gene_PsychicBonding>()?.BondTo(pawn);
                }
            }

            return false;
        }
    }
}
