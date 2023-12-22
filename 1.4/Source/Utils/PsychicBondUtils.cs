using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PsychicBondTweaks
{
    internal class PsychicBondUtils
    {
        public static Color PsychicBondTornLabelColor => new Color(0.66f, 0.66f, 0.66f);

        // can find PsychicBondProximity ThoughtDef info at Rimworld/Data/Biotech/Defs/ThoughtDefs/Thoughts_Situation_Special.xml
        private const string THOUGHT_PSYCHIC_BOND_PROXIMITY_DEF_NAME = "PsychicBondProximity";
        private const string THOUGHT_STAGE_PSYCHIC_BOND_PROXIMITY_STAGE_CLOSE_LABEL = "psychic bond";
        private const string THOUGHT_PSYCHIC_BOND_PROXIMITY_STAGE_DISTANCE_LABEL = "psychic bond distance";

        // ThoughtWorker_PsychicBondProximity class
        private static ThoughtDef psychicBondProximityMemoryDef = DefDatabase<ThoughtDef>.GetNamedSilentFail(THOUGHT_PSYCHIC_BOND_PROXIMITY_DEF_NAME);
        private static ThoughtStage psychicBondProximityStageCloseMood = psychicBondProximityMemoryDef.stages.First(stage => stage.label == THOUGHT_STAGE_PSYCHIC_BOND_PROXIMITY_STAGE_CLOSE_LABEL);
        private static ThoughtStage psychicBondProximityStageDistanceMood = psychicBondProximityMemoryDef.stages.First(stage => stage.label == THOUGHT_PSYCHIC_BOND_PROXIMITY_STAGE_DISTANCE_LABEL);

        // can find psychic bond hediff info at Rimworld/Data/Biotech/Defs/HediffDefs/Hediffs_Various.xml
        private const string HEDIFF_PSYCHIC_BOND_DEF_NAME = "PsychicBond";
        private const string HEDIFF_PSYCHICH_BOND_STAGE_OVERRIDE_LABLE = "psychic bond";
        private const string HEDIFF_PSYCHICH_BOND_DISTANCE_STAGE_OVERRIDE_LABLE = "psychic bond distance";
        private const string CONSCIOUNESS_CAP_MOD_DEF_NAME = "Consciousness";

        private static HediffDef psychicBondHediffDef = DefDatabase<HediffDef>.GetNamedSilentFail(HEDIFF_PSYCHIC_BOND_DEF_NAME);

        private static HediffStage psychicBondStage = psychicBondHediffDef.stages.First(stage => stage.overrideLabel == HEDIFF_PSYCHICH_BOND_STAGE_OVERRIDE_LABLE);
        private static PawnCapacityModifier psychicBondStageConsciousness = psychicBondStage.capMods.First(capMod => capMod.capacity.defName == CONSCIOUNESS_CAP_MOD_DEF_NAME);

        private static HediffStage psychicBondRefreshStage = null;
        private static PawnCapacityModifier psychicBondRefreshStageConsciousness = null;

        private static HediffStage psychicBondDistanceStage = psychicBondHediffDef.stages.First(stage => stage.overrideLabel == HEDIFF_PSYCHICH_BOND_DISTANCE_STAGE_OVERRIDE_LABLE);
        private static PawnCapacityModifier psychicBondDistanceStageConsciousness = psychicBondDistanceStage.capMods.First(capMod => capMod.capacity.defName == CONSCIOUNESS_CAP_MOD_DEF_NAME);

        public static void RemoveHediffPsychicBondDistanceStage()
        {
            /*ThoughtWorker_PsychicBondProximity
            Thought_SituationalSocial*/

            List<int> stagesToRemove = new List<int>();
            for (int i = 0; i < psychicBondHediffDef.stages.Count; ++i)
            {
                HediffStage stage = psychicBondHediffDef.stages[i];
                if (stage.overrideLabel == HEDIFF_PSYCHICH_BOND_DISTANCE_STAGE_OVERRIDE_LABLE)
                {
                    stagesToRemove.Add(i);
                }
            }

            foreach (int stageIndex in stagesToRemove)
            {
                psychicBondHediffDef.stages.RemoveAt(stageIndex);
            }
        }

        public static void AddHediffPsychicBondDistanceStage()
        {
            bool foundStage = false;
            foreach (HediffStage stage in psychicBondHediffDef.stages)
            {
                if (stage.overrideLabel == HEDIFF_PSYCHICH_BOND_DISTANCE_STAGE_OVERRIDE_LABLE)
                {
                    foundStage = true;
                }
            }

            if (!foundStage)
            {
                psychicBondHediffDef.stages.Add(psychicBondDistanceStage);
            }
        }

        // can find in RimWorld\Data\Biotech\Defs\ThoughtDefs\Thoughts_Memory_Special.xml
        public static void SetPsychichBondTornMoodOfsset()
        {
            ThoughtDefOf.PsychicBondTorn.stages[0].baseMoodEffect = Settings.bond_torn_mood_effect;
        }

        public static void SetPsychicBondProximityStageCloseMood()
        {
            psychicBondProximityStageCloseMood.baseMoodEffect = Settings.mood_bonus;
        }

        public static void SetPsychicBondProximityStageDistanceMood()
        {
            psychicBondProximityStageDistanceMood.baseMoodEffect = Settings.distance_mood_bonus;
        }

        public static void SetHediffPsychichBondConsciousnessOffset()
        {
            // incase there is no psychicbond refresh stage create one
            if (psychicBondRefreshStage is null)
            {
                AddPsychicBondRefreshStage();
            }

            if (psychicBondStageConsciousness != null)
            {
                psychicBondStageConsciousness.offset = ((int)Settings.consciousness_bonus) / 100f;
                Utils.LogM($"SetPsychichBondConsciousnessBonus -> PsychicBond Consciousness offset: {(int)(psychicBondStageConsciousness.offset * 100)}%");
            }

            if (psychicBondRefreshStageConsciousness != null)
            {
                psychicBondRefreshStageConsciousness.offset = psychicBondStageConsciousness.offset;
                Utils.LogM($"SetPsychichBondConsciousnessBonus -> psychic bond refresh Consciousness offset: {(int)(psychicBondRefreshStageConsciousness.offset * 100)}%");
            }

            if (psychicBondDistanceStageConsciousness != null)
            {
                psychicBondDistanceStageConsciousness.offset = ((int)Settings.distance_consciousness_bonus) / 100f;
                Utils.LogM($"SetPsychichBondConsciousnessBonus -> psychic bond distance Consciousness offset: {(int)(psychicBondDistanceStageConsciousness.offset * 100)}%");
            }
        }

        public static void RefreshPawnsPsychichBond()
        {
            // setting severity to 0.1f which will cause the hediff to refresh back to either 1f when far from partner or 0.5f when close to partner
            // this will make it so that any change to capMod settings will be registered without needing to load the game
            foreach (Pawn pawn in PawnsFinder.AllMapsAndWorld_Alive)
            {
                if (pawn != null && pawn.health != null)
                {
                    foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                    {
                        Hediff_PsychicBond hediff_PsychicBond = hediff as Hediff_PsychicBond;
                        if (hediff_PsychicBond is not null)
                        {
                            Utils.LogM($"RefreshPawnsPsychichBond() -> {pawn.Name} psychic bond refreshed");
                            hediff_PsychicBond.Severity = 0.1f;
                        }
                    }
                }
            }
        }

        // psychic bons minseverity starts from 0f, the game though sets the severity to 0.5f or 1.5f
        // this allows to create a refresh mechanism by adding another stage with severity of 0f while moving the original to 0.1f
        // which allows to move the severity to 0.1f which will cause the refresh stage to be active
        // after which the game will fix the Hediff current severity to either 0.5f or 1.5f depends on distance
        private static void AddPsychicBondRefreshStage()
        {
            psychicBondStage.minSeverity = 0.2f;

            HediffStage _psychichBondRefreshStage = new HediffStage();
            _psychichBondRefreshStage.overrideLabel = $"psychic bond refresh";
            _psychichBondRefreshStage.extraTooltip = psychicBondStage.extraTooltip;
            _psychichBondRefreshStage.minSeverity = 0.1f; // regular psychic bond stages severity is 0.5f or 1.5f
            _psychichBondRefreshStage.painFactor = psychicBondStage.painFactor;
            _psychichBondRefreshStage.statOffsets = new List<StatModifier>(psychicBondStage.statOffsets);
            _psychichBondRefreshStage.capMods = new List<PawnCapacityModifier>();
            foreach (PawnCapacityModifier capMod in psychicBondStage.capMods)
            {
                PawnCapacityModifier newCapMod = new PawnCapacityModifier();
                newCapMod.capacity = capMod.capacity;
                newCapMod.offset = psychicBondStageConsciousness.offset; // copying Psychic Bond stage offset
                _psychichBondRefreshStage.capMods.Add(newCapMod);
                // setting a reference for quick access
                psychicBondRefreshStageConsciousness = newCapMod;
            }
            // caching refresh stage for later manipulations
            psychicBondRefreshStage = _psychichBondRefreshStage;

            psychicBondHediffDef.stages.Add(_psychichBondRefreshStage);

            // sorting psychicBondHediff stages in order of severity, otherwise it'll break
            psychicBondHediffDef.stages.Sort((HediffStage stage1, HediffStage stage2) =>
            {
                int result;

                if (stage1.minSeverity > stage2.minSeverity)
                {
                    result = 1; // means greater
                }
                else if (stage1.minSeverity < stage2.minSeverity)
                {
                    result = -1; // means lower
                }
                else
                {
                    result = 0; // means equal
                }

                return result;
            });
        }

        public static void TearBond(Pawn bondingPawn, Pawn bondedPawn, int mentalBreakChance = 0, int moodOffset = 0, int moodDebuffDurationDays = 0)
        {
            bondingPawn.relations.RemoveDirectRelation(PawnRelationDefOf.Lover, bondedPawn);
            bondingPawn.relations.RemoveDirectRelation(PawnRelationDefOf.Fiance, bondedPawn);
            bondingPawn.relations.RemoveDirectRelation(PawnRelationDefOf.Spouse, bondedPawn);

            bondingPawn.RemoveHediff_PsychicBond(bondedPawn);
            bondedPawn.RemoveHediff_PsychicBond(bondingPawn);

            if (!Settings.disable_tear_bond_consequences)
            {
                bondingPawn.TryAddPsychicBondTearMemory(bondedPawn, moodOffset, moodDebuffDurationDays);

                // trying to set mental break state, if not then play negative event sound
                if (Rand.Chance(mentalBreakChance / 100f) && !bondingPawn.Dead && DefDatabase<MentalBreakDef>.AllDefsListForReading.Where((MentalBreakDef d) => d.intensity == MentalBreakIntensity.Extreme && d.Worker.BreakCanOccur(bondingPawn)).TryRandomElementByWeight((MentalBreakDef d) => d.Worker.CommonalityFor(bondingPawn, moodCaused: true), out var result))
                {
                    result.Worker.TryStart(bondingPawn, "MentalStateReason_BondedHumanDeath".Translate(bondedPawn), causedByMood: false);
                }
                else
                {
                    //LetterDefOf.NegativeEvent.arriveSound.PlayOneShotOnCamera();
                    SoundDefOf.RitualConclusion_Negative.PlayOneShotOnCamera();
                }
            }
        }

        public static Action TearBondAction = null;
        public static Action GetTearBondWarningDialog(Pawn bondingPawn, Pawn bondedPawn)
        {
            int psychicBondDurationTicks = bondingPawn.GetPsychichBondDurationTicks();
            int psychicBondDurationDays = (int)psychicBondDurationTicks.TicksToDays();
            int psychicBondDurationHours = psychicBondDurationTicks.TicksDaysHoursFraction();
            string bondDayDays = psychicBondDurationDays == 1 ? "day".PBTranslate() : "days".PBTranslate();
            string bondHourHours = psychicBondDurationHours == 1 ? "hour".PBTranslate() : "hours".PBTranslate();

            // calculating mental break chance and mood debuff offset and duration
            int settingsBondDurationDaysTicks = Settings.tear_bond_days_for_consequences.DaysToTicks();
            int bondDurationPercent = settingsBondDurationDaysTicks == 0 || settingsBondDurationDaysTicks < psychicBondDurationTicks ? 100 :
                (int)((float)psychicBondDurationTicks / settingsBondDurationDaysTicks * 100);

            int mentalBreakChance = Settings.tear_bond_consequences_minimum_mental_break_chance < bondDurationPercent ? bondDurationPercent :
                                    Settings.tear_bond_consequences_minimum_mental_break_chance;

            int moodOffset = Settings.bond_torn_mood_effect * bondDurationPercent / 100;
            int moodOffsetDuration = 30 * bondDurationPercent / 100;
            string moodDayDays = moodOffsetDuration == 1 ? "day".PBTranslate() : "days".PBTranslate();

            string searchReplace = $":param_bonding_pawn_name|{bondingPawn.Name.ToStringShort},:param_bonded_pawn_name|{bondedPawn.Name.ToStringShort},";
            searchReplace +=        $":param_days_duration|{psychicBondDurationDays},:param_bond_day_days|{bondDayDays},:params_bond_hours_duration|{psychicBondDurationHours},";
            searchReplace +=        $":param_bond_hour_hours|{bondHourHours},:param_mental_break_chance|{mentalBreakChance}%,:param_mood_offset|{moodOffset},";
            searchReplace +=        $":param_mood_offset_duration|{moodOffsetDuration},:param_mood_day_days|{moodDayDays}";

            string consequences = @$"{"tear_bond_warning_consequence_debuffs".PBTranslate(searchReplace)}
{(moodOffsetDuration > 0 ? "tear_bond_warning_consequence_relationship".PBTranslate(searchReplace) : "")}
{(mentalBreakChance > 0 ? "tear_bond_warning_consequence_mental_break".PBTranslate(searchReplace) : "")}
{(moodOffsetDuration > 0 ? "tear_bond_warning_consequence_mood".PBTranslate(searchReplace) : "")}";

            string warningMessage = @$"{"tear_bond_warning".PBTranslate(searchReplace)}

{"tear_bond_warning_duration".PBTranslate(searchReplace)}

{"tear_bond_warning_forget_relationship".PBTranslate(searchReplace)}

{(Settings.disable_tear_bond_consequences || (mentalBreakChance == 0 && moodOffsetDuration == 0) ? "" : consequences)}

{"tear_bond_warning_confirm".PBTranslate(searchReplace)}
";

            return delegate
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                    warningMessage,
                    delegate
                    {
                        //TearBond(bondingPawn, bondedPawn, mentalBreakChance, moodOffset, moodOffsetDuration);
                        TearBondAction = () => TearBond(bondingPawn, bondedPawn, mentalBreakChance, moodOffset, moodOffsetDuration);
                        bondingPawn.GetPsychicBondGene().RemoveBond();
                        TearBondAction = null;
                        Utils.LogM($"TearBondAction is null -> {TearBondAction is null}");
                    }
                ));
            };
        }
    }
}
