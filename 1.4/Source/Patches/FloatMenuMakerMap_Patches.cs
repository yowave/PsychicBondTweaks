using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PsychicBondTweaks.Patches
{
    [Harmony]
    internal class FloatMenuMakerMap_Patches
    {
        private static FloatMenuOption TearBondContextMenuOption = new FloatMenuOption("tear_bond_gizmo_label".PBTranslate(),
                        null,
                        Textures.TearBondTexture,
                        Color.white,
                        iconJustification: HorizontalJustification.Right);

        [HarmonyPatch(typeof(FloatMenuMakerMap), "ChoicesAtFor")]
        [HarmonyPostfix]
        public static void ChoicesAtFor_Postfix_Patch(ref List<FloatMenuOption> __result, Vector3 clickPos, Pawn pawn)
        {
            if (Settings.add_tear_bond_context_menu && pawn.HasPsychicBondGene())
            {
                IntVec3 c = IntVec3.FromVector3(clickPos);
                List<Thing> thingList = c.GetThingList(pawn.Map);
                foreach (Thing thing in thingList)
                {
                    Pawn bondedPawn = pawn.GetBondedPawn();
                    if (thing is Pawn clickedPawn && pawn == clickedPawn &&
                        bondedPawn is not null)
                    {
                        FloatMenuOption tearBondContextMenuOption = TearBondContextMenuOption;
                        tearBondContextMenuOption.action = PsychicBondUtils.GetTearBondWarningDialog(pawn, bondedPawn);
                        tearBondContextMenuOption.tooltip = "tear_bond_gizmo_desc".PBTranslate();

                        __result.Add(tearBondContextMenuOption);
                        break;
                    }
                }
            }
        }
    }
}
