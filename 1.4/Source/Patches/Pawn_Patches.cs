using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal static class Pawn_Patches
    {
        private static readonly Command_Action TearBondGizmoCommand = new Command_Action
        {
            defaultLabel = "tear_bond_gizmo_label".PBTranslate(),
            defaultDesc = "tear_bond_gizmo_desc".PBTranslate(),
            icon = Textures.TearBondTexture,
            Order = -900f,
        };

        private static Command_Action TryGetTearBondGizmo(this Pawn bondingPawn)
        {
            Command_Action tearBondGizmoCommand = null;
            Pawn bondedPawn = bondingPawn.GetBondedPawn();
            if (bondedPawn is not null)
            {
                tearBondGizmoCommand = TearBondGizmoCommand;
                tearBondGizmoCommand.action = PsychicBondUtils.GetTearBondWarningDialog(bondingPawn, bondedPawn);
            }
            return tearBondGizmoCommand;
        }

        private static readonly Command_Toggle allowBondingCommandToggle = new Command_Toggle
        {
            defaultLabel = "bonding_gizmo_enabled".PBTranslate(),
            defaultDesc = "bonding_gizmo_enabled_desc".PBTranslate(),
            //icon = TexCommand.ToggleVent,
            icon = ContentFinder<Texture2D>.Get("UI/Icons/Genes/Gene_PsychicBonding"),
            Order = -899f,
        };

        private static Command_Toggle TryGetAllowBondingGizmo(this Pawn bondingPawn)
        {
            Command_Toggle allowBondingGizmo = null;
            if (bondingPawn.GetBondedPawn() is null && bondingPawn.DevelopmentalStage.Adult())
            {
                allowBondingGizmo = allowBondingCommandToggle;
                allowBondingGizmo.isActive = () =>
                {
                    bool isActive = false;
                    Store.psychicBondToggle.TryGetValue(bondingPawn.ThingID, out isActive);

                    if (isActive)
                    {
                        allowBondingGizmo.defaultLabel = "bonding_gizmo_enabled".PBTranslate();
                        allowBondingGizmo.defaultDesc = "bonding_gizmo_enabled_desc".PBTranslate($":param_pawn_name|{bondingPawn.Name.ToStringShort}");
                    }
                    else
                    {
                        allowBondingGizmo.defaultLabel = "bonding_gizmo_disabled".PBTranslate();
                        allowBondingGizmo.defaultDesc = "bonding_gizmo_disabled_desc".PBTranslate($":param_pawn_name|{bondingPawn.Name.ToStringShort}");
                    }

                    return isActive;
                };

                allowBondingGizmo.toggleAction = delegate
                {
                    bool isActive = false;
                    Store.psychicBondToggle.TryGetValue(bondingPawn.ThingID, out isActive);
                    Store.psychicBondToggle[bondingPawn.ThingID] = !isActive;
                };
            }
            return allowBondingGizmo;
        }

        // pass through postfix: https://harmony.pardeike.net/articles/patching-postfix.html#pass-through-postfixes
        [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
        [HarmonyPostfix]
        public static IEnumerable<Gizmo> GetGizmos_Postfix_Patch(IEnumerable<Gizmo> values, Pawn __instance)
        {
            foreach (Gizmo gizmo in values)
            {
                yield return gizmo;
            }

            Pawn pawn = __instance;
            if (pawn.IsColonist && Find.Selector.SelectedPawns.Count == 1 && pawn.HasPsychicBondGene())
            {
                if (Settings.add_bonding_toggle_gizmo)
                {
                    Command_Toggle allowBondingGizmo = pawn.TryGetAllowBondingGizmo();
                    if (allowBondingGizmo is not null)
                    {
                        yield return allowBondingGizmo;
                    }
                }

                if (Settings.add_tear_bond_gizmo)
                {
                    Command_Action tearBondGizmoCommand = pawn.TryGetTearBondGizmo();
                    if (tearBondGizmoCommand is not null)
                    {
                        yield return tearBondGizmoCommand;
                    }
                }
            }
        }
    }
}
