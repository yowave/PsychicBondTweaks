using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PsychicBondTweaks
{
    public static partial class PsychicBondExtensions
    {
        private const string PSYCHIC_BOND_TORN_TEAR_BOND_DEF_NAME = "PsychicBondTorn_TearBond";
        private static readonly ThoughtDef PSYCHIC_BOND_TORN_THOUGHT_DEF = DefDatabase<ThoughtDef>.GetNamed(PSYCHIC_BOND_TORN_TEAR_BOND_DEF_NAME, true);

        public static void AddHediff_PsychicBond(this Pawn pawn, Pawn target)
        {
            Hediff_PsychicBond hediff_PsychicBond = null;
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                Hediff_PsychicBond temp_hediff_PsychicBond = hediff as Hediff_PsychicBond;
                if (temp_hediff_PsychicBond is not null && temp_hediff_PsychicBond.target.ThingID == target.ThingID)
                {
                    hediff_PsychicBond = temp_hediff_PsychicBond;
                }
            }

            if (hediff_PsychicBond == null)
            {
                hediff_PsychicBond = (Hediff_PsychicBond)HediffMaker.MakeHediff(HediffDefOf.PsychicBond, pawn);
                hediff_PsychicBond.target = target;
                pawn.health.AddHediff(hediff_PsychicBond);
                Utils.LogM($"AddHediff_PsychicBond -> adding Psychic bond hediff to [{pawn.Name}], hediff target [{target.Name}]");
            }
        }

        public static void RemoveHediff_PsychicBond(this Pawn pawn, Pawn target)
        {
            Hediff_PsychicBond bondedPawn_hediff_PsychicBond;
            List<int> removeList = new List<int>();
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; ++i)
            {
                bondedPawn_hediff_PsychicBond = pawn.health.hediffSet.hediffs[i] as Hediff_PsychicBond;
                if (bondedPawn_hediff_PsychicBond is not null && bondedPawn_hediff_PsychicBond.target.ThingID == target.ThingID)
                {
                    removeList.Add(i);
                }
            }

            foreach (int hediffIndex in removeList)
            {
                //Hediff hediff = pawn.health.hediffSet.hediffs[hediffIndex];
                //pawn.health.RemoveHediff(hediff);
                pawn.health.hediffSet.hediffs.RemoveAt(hediffIndex);
                Utils.LogM($"RemovePsychicBond -> Removing Psychic bond hediff from [{pawn.Name}] the bond was with [{target.Name}]");
            }
        }

        public static void AddHediff_PsychicBondTorn(this Pawn pawn, Pawn target)
        {
            Hediff_PsychicBondTorn hediff_PsychicBondTorn = null;
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                Hediff_PsychicBondTorn temp_hediff_PsychicBondTorn = hediff as Hediff_PsychicBondTorn;
                if (temp_hediff_PsychicBondTorn is not null && temp_hediff_PsychicBondTorn.target.ThingID == target.ThingID)
                {
                    hediff_PsychicBondTorn = temp_hediff_PsychicBondTorn;
                }
            }

            if (hediff_PsychicBondTorn == null)
            {
                hediff_PsychicBondTorn = (Hediff_PsychicBondTorn)HediffMaker.MakeHediff(HediffDefOf.PsychicBondTorn, pawn);
                hediff_PsychicBondTorn.target = target;
                pawn.health.AddHediff(hediff_PsychicBondTorn);
                Utils.LogM($"AddHediff_PsychicBondTorn -> adding Psychic bond torn hediff to [{pawn.Name}] the bond was with [{target.Name}]");
            }
        }

        public static void RemoveHediff_PsychicBondTorn(this Pawn pawn, Pawn target)
        {
            Hediff_PsychicBondTorn bondedPawn_hediff_PsychicBondTorn;
            List<int> removeList = new List<int>();
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; ++i)
            {
                bondedPawn_hediff_PsychicBondTorn = pawn.health.hediffSet.hediffs[i] as Hediff_PsychicBondTorn;
                if (bondedPawn_hediff_PsychicBondTorn is not null && bondedPawn_hediff_PsychicBondTorn.target.ThingID == target.ThingID)
                {
                    removeList.Add(i);
                }
            }

            foreach (int hediffIndex in removeList)
            {
                Hediff hediff = pawn.health.hediffSet.hediffs[hediffIndex];
                pawn.health.RemoveHediff(hediff);
                Utils.LogM($"RemovePsychicBondTorn -> Removing Psychic bond torn hediff from [{pawn.Name}] the bond was with [{target.Name}]");
            }
        }

        public static void TryAddPsychicBondTornMemory(this Pawn pawn, Pawn otherPawn)
        {
            Thought_Memory newThought = ThoughtMaker.MakeThought(ThoughtDefOf.PsychicBondTorn, null);
            if (!pawn.Dead && ThoughtUtility.CanGetThought(pawn, newThought.def))
            {
                // sanity check, removing same memory if already existing
                pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(ThoughtDefOf.PsychicBondTorn, (Thought_Memory m) => m.otherPawn == otherPawn);

                newThought.pawn = pawn;
                newThought.otherPawn = otherPawn;
                pawn.needs.mood.thoughts.memories.Memories.Add(newThought);

                if (newThought.def.showBubble && pawn.Spawned && PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    MoteMaker.MakeMoodThoughtBubble(pawn, newThought);
                }
            }
        }

        // can find the ThoughtDef info in Rimworld\Data\Biotec\Defs\ThoughtDefs\Thoughts_Memory_Special.xml
        public static void TryAddPsychicBondTearMemory(this Pawn pawn, Pawn otherPawn, int moodOffset = -25, int moodOffsetDurationDays = 30)
        {
            Thought_Memory newThought = ThoughtMaker.MakeThought(PSYCHIC_BOND_TORN_THOUGHT_DEF, null);
            newThought.moodOffset = moodOffset;
            newThought.durationTicksOverride = moodOffsetDurationDays.DaysToTicks();

            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(PSYCHIC_BOND_TORN_THOUGHT_DEF, (Thought_Memory m) => m.otherPawn == otherPawn);

            if (ThoughtUtility.CanGetThought(pawn, newThought.def))
            {
                pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefIf(PSYCHIC_BOND_TORN_THOUGHT_DEF, (Thought_Memory m) => m.otherPawn == otherPawn);

                newThought.pawn = pawn;
                newThought.otherPawn = otherPawn;
                pawn.needs.mood.thoughts.memories.Memories.Add(newThought);

                if (newThought.def.showBubble && pawn.Spawned && PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    MoteMaker.MakeMoodThoughtBubble(pawn, newThought);
                }
            }
        }

        public static Gene_PsychicBonding GetPsychicBondGene(this Pawn pawn)
        {
            Gene_PsychicBonding pbGene = null;
            foreach (Gene gene in pawn?.genes.Endogenes)
            {
                if (gene.Active && gene is Gene_PsychicBonding _pbGene)
                {
                    pbGene = _pbGene;
                    break;
                }
            }

            foreach (Gene gene in pawn?.genes.Xenogenes)
            {
                if (gene.Active && gene is Gene_PsychicBonding _pbGene)
                {
                    pbGene = _pbGene;
                    break;
                }
            }

            return pbGene;
        }

        public static bool HasPsychicBondGene(this Pawn pawn)
        {
            return pawn.GetPsychicBondGene() is not null;
        }

        public static bool HasPsychicBondTornHediff(this Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediff<Hediff_PsychicBondTorn>() != null;
        }

        public static bool HasPsychicBondTornThought(this Pawn pawn)
        {
            bool result = false;
            if (!pawn.Dead)
            {
                result = pawn.needs.mood.thoughts.memories.Memories.Any(memory => memory.def.defName == ThoughtDefOf.PsychicBondTorn.defName ||
                                                                memory.def.defName == PSYCHIC_BOND_TORN_TEAR_BOND_DEF_NAME);
            }
            return result;
        }

        public static bool HasPsychicBondHediff(this Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediff<Hediff_PsychicBond>() != null;
        }

        public static bool HasBondWith(this Pawn pawn, Pawn otherPawn)
        {
            return pawn.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_PsychicBond hediffPB && hediffPB.target.ThingID == otherPawn.ThingID); ;
        }

        public static Hediff_PsychicBond GetPsychicBondHediff(this Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediff<Hediff_PsychicBond>();
        }

        public static Pawn GetBondedPawn(this Pawn pawn)
        {
            return (Pawn)(pawn.GetPsychicBondHediff()?.target);
        }

        public static int GetPsychichBondDurationTicks(this Pawn pawn)
        {
            Hediff_PsychicBond hediff_PsychicBond = GetPsychicBondHediff(pawn);
            return hediff_PsychicBond is not null ?
                hediff_PsychicBond.ageTicks :
                0;
        }

        public static bool IsBondingDisabled(this Pawn pawn)
        {
            return !Store.psychicBondToggle.TryGetValue(pawn.ThingID, out bool recipientBondToggled) || !recipientBondToggled;
        }
    }
}
