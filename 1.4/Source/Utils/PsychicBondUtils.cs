using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PsychicBondTweaks
{
    internal class PsychicBondUtils
    {
        public static Color PsychicBondTornLabelColor => new Color(0.66f, 0.66f, 0.66f);

        // can find psychic bond hediff info at Rimworld/Data/Biotech/Defs/HediffDefs/Hediffs_Various.xml
        private const string PSYCHICH_BOND_OVERRIDE_LABLE = "psychic bond";
        private const string PSYCHICH_BOND_DISTANCE_OVERRIDE_LABLE = "psychic bond distance";
        private const string PSYCHIC_BOND_HEDIFF_DEF_NAME = "PsychicBond";
        private const string PSYCHIC_BOND_CONSCIOUNESS_CAP_MOD_DEF_NAME = "Consciousness";


        private static HediffDef psychicBondHediffDef = DefDatabase<HediffDef>.GetNamedSilentFail(PSYCHIC_BOND_HEDIFF_DEF_NAME);

        private static HediffStage psychicBondStage = psychicBondHediffDef.stages.First(stage => stage.overrideLabel == PSYCHICH_BOND_OVERRIDE_LABLE);
        private static PawnCapacityModifier psychicBondStageConsciousness = psychicBondStage.capMods.First(capMod => capMod.capacity.defName == PSYCHIC_BOND_CONSCIOUNESS_CAP_MOD_DEF_NAME);

        private static HediffStage psychicBondRefreshStage = null;
        private static PawnCapacityModifier psychicBondRefreshStageConsciousness = null;

        private static HediffStage psychicBondDistanceStage = psychicBondHediffDef.stages.First(stage => stage.overrideLabel == PSYCHICH_BOND_DISTANCE_OVERRIDE_LABLE);
        private static PawnCapacityModifier psychicBondDistanceStageConsciousness = psychicBondDistanceStage.capMods.First(capMod => capMod.capacity.defName == PSYCHIC_BOND_CONSCIOUNESS_CAP_MOD_DEF_NAME);

        // can find in RimWorld\Data\Biotech\Defs\ThoughtDefs\Thoughts_Memory_Special.xml
        public static void SetPsychichBondTornMoodEffect()
        {
            ThoughtDefOf.PsychicBondTorn.stages[0].baseMoodEffect = Settings.bond_torn_mood_effect;
        }

        public static void RemovePsychicBondDistanceStage()
        {
            List<int> stagesToRemove = new List<int>();
            for (int i = 0; i < psychicBondHediffDef.stages.Count; ++i)
            {
                HediffStage stage = psychicBondHediffDef.stages[i];
                if (stage.overrideLabel == PSYCHICH_BOND_DISTANCE_OVERRIDE_LABLE)
                {
                    stagesToRemove.Add(i);
                }
            }

            foreach (int stageIndex in stagesToRemove)
            {
                psychicBondHediffDef.stages.RemoveAt(stageIndex);
            }
        }

        public static void AddPsychicBondDistanceStage()
        {
            bool foundStage = false;
            foreach (HediffStage stage in psychicBondHediffDef.stages)
            {
                if (stage.overrideLabel == PSYCHICH_BOND_DISTANCE_OVERRIDE_LABLE)
                {
                    foundStage = true;
                }
            }

            if (!foundStage)
            {
                psychicBondHediffDef.stages.Add(psychicBondDistanceStage);
            }
        }

        public static void SetPsychichBondConsciousnessOffset()
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

        // psychic bons minseverity starts from 0f, the game though sets the severity to 0.5f or 1f
        // this allows to create a refresh mechanism by adding another stage with severity of 0f while moving the original to 0.1f
        // which allows to move the severity to 0.1f which will cause the refresh stage to be active
        // after which the game will fix the Hediff current severity to either 0.5f or 1f depends on distance
        public static void AddPsychicBondRefreshStage()
        {
            psychicBondStage.minSeverity = 0.2f;

            HediffStage _psychichBondRefreshStage = new HediffStage();
            _psychichBondRefreshStage.overrideLabel = $"psychic bond refresh";
            _psychichBondRefreshStage.extraTooltip = psychicBondStage.extraTooltip;
            _psychichBondRefreshStage.minSeverity = 0.1f; // regular psychic bond stages minseverity is 0f or 1f
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
    }
}
