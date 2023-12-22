using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal class InteractionWorker_RomanceAttempt_Patches
    {
        // stops automatic romance in various cases, depends on settings
        [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), nameof(InteractionWorker_RomanceAttempt.RandomSelectionWeight))]
        [HarmonyPostfix]
        public static void RandomSelectionWeight_Postfix_Patch(ref float __result, Pawn initiator, Pawn recipient)
        {
            Utils.LogM("+++ RandomSelectionWeight_Postfix_Patch +++");
            if (StopRomanceAttempt(initiator, recipient))
            {
                __result = 0f;
            }
        }

        // have an effect among others when right clicking on another pawn        
        [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), nameof(InteractionWorker_RomanceAttempt.SuccessChance))]
        [HarmonyPostfix]
        public static void SuccessChance_Postfix_Patch(ref float __result, Pawn initiator, Pawn recipient)
        {
            Utils.LogM("+++ SuccessChance_Postfix_Patch +++");
            if (StopRomanceAttempt(initiator, recipient, false))
            {
                __result = 0f;
            }
        }

        private static bool StopRomanceAttempt(Pawn initiator, Pawn recipient, bool considerInitiatorBondingDisable = true)
        {
            bool stopRomanceAttempt = false;

            bool initiatorPsychichBondGene = initiator.HasPsychicBondGene();
            bool recipientPsychicBondGene = recipient.HasPsychicBondGene();

            if (initiatorPsychichBondGene || recipientPsychicBondGene)
            {
                if (!initiator.HasBondWith(recipient))
                {
                    if (initiatorPsychichBondGene && (initiator.HasPsychicBondHediff() || initiator.HasPsychicBondTornHediff() || initiator.HasPsychicBondTornThought()))
                    {
                        Utils.LogM($"Stopping bond attempt between [{initiator.Name.ToStringShort}] and [{recipient.Name.ToStringShort}], reason: one of them have a bond/bond torn hediff/bond torn thought");
                        stopRomanceAttempt = true;
                    }
                    else if (recipientPsychicBondGene && (recipient.HasPsychicBondHediff() || recipient.HasPsychicBondTornHediff() || recipient.HasPsychicBondTornThought()))
                    {
                        Utils.LogM($"Stopping bond attempt between [{initiator.Name.ToStringShort}] and [{recipient.Name.ToStringShort}], reason: one of them have a bond/bond torn hediff/bond torn thought");
                        stopRomanceAttempt = true;
                    }
                    else if (Settings.prevent_colonist_bonding_with_strangers &&
                            (initiator.IsColonist || recipient.IsColonist) &&
                            (!initiator.IsColonist || !recipient.IsColonist))
                    {
                        Utils.LogM($"Stopping bond attempt between [{initiator.Name.ToStringShort}] and [{recipient.Name.ToStringShort}], reason: one of them is a colonist, while the other isn't.");
                        stopRomanceAttempt = true;
                    }
                    else if (Settings.prevent_strangers_bonding_with_strangers && !initiator.IsColonist && !recipient.IsColonist)
                    {
                        Utils.LogM($"Stopping bond attempt between [{initiator.Name.ToStringShort}] and [{recipient.Name.ToStringShort}], reason: both of them are not colonists.");
                        stopRomanceAttempt = true;
                    }
                    else if (initiator.IsColonist && initiatorPsychichBondGene && Settings.add_bonding_toggle_gizmo && initiator.IsBondingDisabled() && considerInitiatorBondingDisable)
                    {
                        Utils.LogM($"Stopping bond attempt between [{initiator.Name.ToStringShort}] and [{recipient.Name.ToStringShort}], reason: [{initiator.Name.ToStringShort}] bonding gizmo disabled.");
                        stopRomanceAttempt = true;
                    }
                    else if (Settings.add_bonding_toggle_gizmo && recipient.IsColonist && recipientPsychicBondGene && recipient.IsBondingDisabled())
                    {
                        Utils.LogM($"Stopping bond attempt between [{initiator.Name.ToStringShort}] and [{recipient.Name.ToStringShort}], reason: [{recipient.Name.ToStringShort}] bonding gizmo disabled.");
                        stopRomanceAttempt = true;
                    }
                }
            }

            return stopRomanceAttempt;
        }
    }
}
