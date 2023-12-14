using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal class Gene_PsychicBonding_Patches
    {
        [HarmonyPatch(typeof(Gene_PsychicBonding), nameof(Gene_PsychicBonding.BondTo))]
        [HarmonyPrefix]
        public static bool BondTo_Prefix_Patch(ref Gene_PsychicBonding __instance, ref Pawn ___bondedPawn, Pawn newBond)
        {
            Utils.LogM($"+++ BondTo_Prefix_Patch +++");
            if (!ModLister.CheckBiotech("Psychic bonding") || newBond == null || ___bondedPawn != null)
            {
                return false; // stop execution and don't continue to the original method
            }

            Pawn bondingPawn = __instance.pawn;
            Pawn bondedPawn = ___bondedPawn = newBond;

            Utils.LogM($"BondTo_Prefix_Patch -> creating bond between [{bondingPawn?.Name}] and [{bondedPawn?.Name}]");

            bondingPawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(ThoughtDefOf.PsychicBondTorn, (Thought_Memory m) => m.otherPawn == bondedPawn);
            bondingPawn.RemoveHediff_PsychicBondTorn(bondedPawn);

            bondedPawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(ThoughtDefOf.PsychicBondTorn, (Thought_Memory m) => m.otherPawn == bondingPawn);
            bondedPawn.RemoveHediff_PsychicBondTorn(bondingPawn);


            bondingPawn.AddHediff_PsychicBond(bondedPawn);
            Gene_PsychicBonding gene_PsychicBonding = bondedPawn.genes?.GetFirstGeneOfType<Gene_PsychicBonding>();
            if (gene_PsychicBonding != null)
            {
                gene_PsychicBonding.BondTo(bondingPawn);
                return false;
            }

            bondedPawn.AddHediff_PsychicBond(bondingPawn);

            return false;
        }

        [HarmonyPatch(typeof(Gene_PsychicBonding), nameof(Gene_PsychicBonding.RemoveBond))]
        [HarmonyPrefix]
        public static bool RemoveBond_Prefix_Patch(ref Pawn ___bondedPawn, ref Gene_PsychicBonding __instance)
        {
            Utils.LogM($"+++ RemoveBond_Prefix_Patch +++");
            if (___bondedPawn == null)
            {
                return false;
            }

            Pawn bondingPawn = __instance.pawn;
            Pawn bondedPawn = ___bondedPawn;
            ___bondedPawn = null;

            bondingPawn.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.PsychicBondTorn, bondedPawn);
            bondedPawn.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.PsychicBondTorn, bondingPawn);

            bondingPawn.RemoveHediff_PsychicBond(bondedPawn);
            bondedPawn.RemoveHediff_PsychicBond(bondingPawn);

            bondedPawn.genes?.GetFirstGeneOfType<Gene_PsychicBonding>()?.RemoveBond();

            // Hediff_PsychicBondTorn is used to bond the pawns again incase one of them gets resurrected            
            bondedPawn.AddHediff_PsychicBondTorn(bondingPawn);
            bondingPawn.AddHediff_PsychicBondTorn(bondedPawn);

            // setting bondingPawn mental state if alive
            if (!bondingPawn.Dead && DefDatabase<MentalBreakDef>.AllDefsListForReading.Where((MentalBreakDef d) => d.intensity == MentalBreakIntensity.Extreme && d.Worker.BreakCanOccur(bondingPawn)).TryRandomElementByWeight((MentalBreakDef d) => d.Worker.CommonalityFor(bondingPawn, moodCaused: true), out var result))
            {
                result.Worker.TryStart(bondingPawn, "MentalStateReason_BondedHumanDeath".Translate(bondedPawn), causedByMood: false);
            }

            return false;
        }
    }
}
