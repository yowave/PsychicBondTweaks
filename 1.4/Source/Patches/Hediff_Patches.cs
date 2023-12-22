using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal class Hediff_Patches
    {
        [HarmonyPatch(typeof(Hediff), nameof(Hediff.LabelColor), MethodType.Getter)]
        [HarmonyPostfix]
        public static void LabelColor_Postfix_Patch(ref Color __result, ref Hediff __instance)
        {
            if (__instance is Hediff_PsychicBondTorn)
            {
                __result = PsychicBondUtils.PsychicBondTornLabelColor;
            }
        }

        // skip the merge incase of different target for Hediff_PsychicBond or Hediff_PsychicBondTorn
        [HarmonyPatch(typeof(Hediff), nameof(Hediff.TryMergeWith))]
        [HarmonyPostfix]
        public static void TryMergeWith_Postfix_Patch(ref bool __result, ref Hediff __instance, Hediff other)
        {
            if (__instance is Hediff_PsychicBond instance_hediff_PsychicBond &&
                other is Hediff_PsychicBond other_hediff_PsychicBond &&
                instance_hediff_PsychicBond is not null &&
                other_hediff_PsychicBond is not null &&
                instance_hediff_PsychicBond.target != other_hediff_PsychicBond.target)
            {
                Utils.LogM("TryMergeWith_Postfix_Patch -> canceling merging Hediff_PsychicBond");
                __result = false;
            }
            else if (__instance is Hediff_PsychicBondTorn instance_hediff_PsychicBondTorn &&
                    other is Hediff_PsychicBondTorn other_hediff_PsychicBondTorn &&
                    instance_hediff_PsychicBondTorn is not null &&
                    other_hediff_PsychicBondTorn is not null &&
                    instance_hediff_PsychicBondTorn.target != other_hediff_PsychicBondTorn.target)
            {
                Utils.LogM("TryMergeWith_Postfix_Patch -> canceling merging Hediff_PsychicBondTorn");
                __result = false;
            }
        }
    }
}
