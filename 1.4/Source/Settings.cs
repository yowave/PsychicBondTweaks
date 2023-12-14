using System.Collections.Generic;
using Verse;
using UnityEngine;
using System;

namespace PsychicBondTweaks
{
    internal class Settings : ModSettings
    {
        public static bool debug_mode;
        public static float distance_consciousness_bonus = -25f;
        public static float consciousness_bonus = 15f;
        public static float bond_torn_mood_effect = -25f;


        public static void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.maxOneColumn = true;
            listingStandard.ColumnWidth = inRect.width / 2.05f;
            listingStandard.Begin(inRect);
            listingStandard.Gap(6f);

            listingStandard.CheckboxLabeled("debug_mode".Translate(), ref debug_mode, "debug_mode_desc".Translate());
            listingStandard.Gap(4f);

            listingStandard.Label($"{"distance_consciousness_bonus".Translate()}: {(int)distance_consciousness_bonus}%", -1f, "distance_consciousness_bonus_desc".Translate());
            distance_consciousness_bonus = (int)listingStandard.Slider(distance_consciousness_bonus, -100f, 100f);
            listingStandard.Gap(4f);

            listingStandard.Label($"{"consciousness_bonus".Translate()}: {(int)consciousness_bonus}%", -1f, "consciousness_bonus_desc".Translate());
            consciousness_bonus = (int)listingStandard.Slider(consciousness_bonus, 0f, 100f);
            listingStandard.Gap(4f);

            listingStandard.Label($"{"bond_torn_mood_effect".Translate()}: {(int)bond_torn_mood_effect}", -1f, "bond_torn_mood_effect_desc".Translate());
            bond_torn_mood_effect = (int)listingStandard.Slider(bond_torn_mood_effect, -100f, 100f);
            listingStandard.Gap(4f);

            listingStandard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref debug_mode, "debug_mode", debug_mode, true);
            Scribe_Values.Look(ref distance_consciousness_bonus, "distance_consciousness_bonus", distance_consciousness_bonus, true);
            Scribe_Values.Look(ref consciousness_bonus, "consciousness_bonus", consciousness_bonus, true);
            Scribe_Values.Look(ref bond_torn_mood_effect, "bond_torn_mood_effect", bond_torn_mood_effect, true);
        }

        public static void ApplySettings()
        {
            Utils.LogM("+++ ApplySettings +++");
            PsychicBondUtils.SetPsychichBondConsciousnessOffset();
            PsychicBondUtils.SetPsychichBondTornMoodEffect();
            PsychicBondUtils.RefreshPawnsPsychichBond();
        }
    }
}
