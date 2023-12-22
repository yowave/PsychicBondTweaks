using System.Collections.Generic;
using Verse;
using UnityEngine;
using System;
using RimWorld;
using System.Text;

namespace PsychicBondTweaks
{
    internal class Settings : ModSettings
    {
        public static bool debug_mode;
        public static bool prevent_colonist_bonding_with_strangers = true;
        public static bool prevent_strangers_bonding_with_strangers = false;
        public static bool remove_psychic_distance;
        public static int distance_consciousness_bonus = -25;
        public static int distance_mood_bonus = -10;
        public static int consciousness_bonus = 15;
        public static int mood_bonus = 12;
        public static int bond_torn_mood_effect = -25;
        public static bool add_tear_bond_gizmo = true;
        public static bool add_tear_bond_context_menu;
        public static bool disable_tear_bond_consequences;
        public static int tear_bond_days_for_consequences = 1;
        public static int tear_bond_consequences_minimum_mental_break_chance = 33;
        public static bool add_bonding_toggle_gizmo = true;

        // for labels
        private static string distanceBuffDebuff;

        // scrollbar vars
        private static Vector2 scrollPosition;

        public static void DoWindowContents(Rect inRect)
        {
            Listing_GUI listing = new Listing_GUI();
            //listing.ColumnWidth = inRect.width / 2.05f;

            listing.BeginScrollView(inRect, ref scrollPosition);
            listing.Gap(6);
            listing.SetElementsGap(10);

            listing.SetPadding(0, 8);

            listing.CheckboxLabeled("debug_mode", ref debug_mode);
            listing.CheckboxLabeled("add_bonding_toggle_gizmo", ref add_bonding_toggle_gizmo);

            listing.SetPadding(30, 8);

            listing.Title("strangers_bonds_title");
            listing.CheckboxLabeled("prevent_colonist_bonding_with_strangers", ref prevent_colonist_bonding_with_strangers);
            listing.CheckboxLabeled("prevent_strangers_bonding_with_strangers", ref prevent_strangers_bonding_with_strangers);

            listing.Title("psychic_bond_far_title");
            listing.CheckboxLabeled("remove_psychic_distance", ref remove_psychic_distance);
            if (!remove_psychic_distance)
            {
                distanceBuffDebuff = distance_consciousness_bonus < 0 ? "debuff".PBTranslate() : "buff".PBTranslate();
                listing.SliderLabeled("distance_consciousness_bonus", ref distance_consciousness_bonus, -100, 100, true, $":param_buff_debuff|{distanceBuffDebuff}");
                distanceBuffDebuff = distance_mood_bonus < 0 ? "debuff".PBTranslate() : "buff".PBTranslate();
                listing.SliderLabeled("distance_mood_bonus", ref distance_mood_bonus, -100, 100, false, $":param_buff_debuff|{distanceBuffDebuff}");
            }

            listing.Title("psychic_bond_near_title");
            listing.SliderLabeled("consciousness_bonus", ref consciousness_bonus, 0, 100, true);
            listing.SliderLabeled("mood_bonus", ref mood_bonus, 0, 100, false);

            listing.Title("psychic_bond_torn_title");
            distanceBuffDebuff = bond_torn_mood_effect < 0 ? "debuff".PBTranslate() : "buff".PBTranslate();
            listing.SliderLabeled("bond_torn_mood_effect", ref bond_torn_mood_effect, -100, 100, false, $":param_buff_debuff|{distanceBuffDebuff}");

            listing.Title("tear_bond_title");
            listing.CheckboxLabeled("add_tear_bond_gizmo", ref add_tear_bond_gizmo);
            listing.CheckboxLabeled("add_tear_bond_context_menu", ref add_tear_bond_context_menu);
            listing.CheckboxLabeled("disable_tear_bond_consequences", ref disable_tear_bond_consequences, () => tear_bond_days_for_consequences = 1);
            if (!disable_tear_bond_consequences)
            {
                listing.SliderLabeled("tear_bond_consequences_minimum_mental_break_chance", ref tear_bond_consequences_minimum_mental_break_chance, 0, 100, true);

                StringBuilder searchReplace = new StringBuilder();
                searchReplace.Append(":param_tear_consequences_example|");
                searchReplace.AppendLine(GetExample(0.25f));
                searchReplace.AppendLine(GetExample(0.50f));
                searchReplace.Append(GetExample(1f));

                listing.SliderLabeled("tear_bond_days_for_consequences", ref tear_bond_days_for_consequences, 1, 10, false, searchReplace.ToString());
            }

            listing.EndScrollView();
        }

        private static string GetExample(float percent)
        {
            int psychicBondDurationTicks = (int)(tear_bond_days_for_consequences.DaysToTicks() * percent);
            int psychicBondDurationDays = (int)psychicBondDurationTicks.TicksToDays();
            int psychicBondDurationHours = psychicBondDurationTicks.TicksDaysHoursFraction();
            string bondDayDays = psychicBondDurationDays == 1 ? "day".PBTranslate() : "days".PBTranslate();
            string bondHourHours = psychicBondDurationHours == 1 ? "hour".PBTranslate() : "hours".PBTranslate();
            // calculating mental break chance and mood debuff offset and duration
            int settingsBondDurationDaysTicks = tear_bond_days_for_consequences.DaysToTicks();
            int bondDurationPercent = settingsBondDurationDaysTicks == 0 || settingsBondDurationDaysTicks < psychicBondDurationTicks ? 100 :
                (int)((float)psychicBondDurationTicks / settingsBondDurationDaysTicks * 100);

            int mentalBreakChance = tear_bond_consequences_minimum_mental_break_chance < bondDurationPercent ? bondDurationPercent :
                                    tear_bond_consequences_minimum_mental_break_chance;

            int moodOffset = bond_torn_mood_effect * bondDurationPercent / 100;
            int moodOffsetDuration = 30 * bondDurationPercent / 100;
            string moodDayDays = moodOffsetDuration == 1 ? "day".PBTranslate() : "days".PBTranslate();

            string searchReplace = $":param_days_duration|{psychicBondDurationDays},:param_bond_day_days|{bondDayDays},:params_bond_hours_duration|{psychicBondDurationHours},";
            searchReplace +=        $":param_bond_hour_hours|{bondHourHours},:param_mental_break_chance|{mentalBreakChance}%,:param_mood_offset|{moodOffset},";
            searchReplace +=        $":param_mood_offset_duration|{moodOffsetDuration},:param_mood_day_days|{moodDayDays}";

            return "tear_bond_days_for_consequences_example".PBTranslate(searchReplace);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref debug_mode, "debug_mode", debug_mode, true);
            Scribe_Values.Look(ref prevent_colonist_bonding_with_strangers, "prevent_colonist_bonding_with_strangers", prevent_colonist_bonding_with_strangers, true);
            Scribe_Values.Look(ref prevent_strangers_bonding_with_strangers, "prevent_strangers_bonding_with_strangers", prevent_strangers_bonding_with_strangers, true);
            Scribe_Values.Look(ref remove_psychic_distance, "remove_psychic_distance", remove_psychic_distance, true);
            Scribe_Values.Look(ref distance_consciousness_bonus, "distance_consciousness_bonus", distance_consciousness_bonus, true);
            Scribe_Values.Look(ref distance_mood_bonus, "distance_mood_bonus", distance_mood_bonus, true);
            Scribe_Values.Look(ref consciousness_bonus, "consciousness_bonus", consciousness_bonus, true);
            Scribe_Values.Look(ref mood_bonus, "mood_bonus", mood_bonus, true);
            Scribe_Values.Look(ref bond_torn_mood_effect, "bond_torn_mood_effect", bond_torn_mood_effect, true);
            Scribe_Values.Look(ref add_tear_bond_gizmo, "add_tear_bond_gizmo", add_tear_bond_gizmo, true);
            Scribe_Values.Look(ref add_tear_bond_context_menu, "add_tear_bond_context_menu", add_tear_bond_context_menu, true);
            Scribe_Values.Look(ref disable_tear_bond_consequences, "disable_tear_bond_consequences", disable_tear_bond_consequences, true);
            Scribe_Values.Look(ref tear_bond_days_for_consequences, "tear_bond_days_for_consequences", tear_bond_days_for_consequences, true);
            Scribe_Values.Look(ref tear_bond_consequences_minimum_mental_break_chance, "tear_bond_consequences_minimum_mental_break_chance", tear_bond_consequences_minimum_mental_break_chance, true);
            Scribe_Values.Look(ref add_bonding_toggle_gizmo, "add_bonding_toggle_gizmo", add_bonding_toggle_gizmo, true);
        }

        public static void ApplySettings()
        {
            Utils.LogM("+++ ApplySettings +++");
            PsychicBondUtils.SetHediffPsychichBondConsciousnessOffset();
            PsychicBondUtils.SetPsychichBondTornMoodOfsset();
            PsychicBondUtils.SetPsychicBondProximityStageDistanceMood();
            PsychicBondUtils.SetPsychicBondProximityStageCloseMood();
            PsychicBondUtils.RefreshPawnsPsychichBond();
        }
    }
}
