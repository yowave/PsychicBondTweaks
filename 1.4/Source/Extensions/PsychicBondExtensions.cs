using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PsychicBondTweaks
{
    public static partial class PsychicBondExtensions
    {
        public static void AddHediff_PsychicBond(this Pawn pawn, Pawn target)
        {
            Hediff_PsychicBond hediff_PsychicBond = null;
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                Hediff_PsychicBond temp_hediff_PsychicBond = hediff as Hediff_PsychicBond;
                if (temp_hediff_PsychicBond is not null && temp_hediff_PsychicBond.target == target)
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
                if (bondedPawn_hediff_PsychicBond is not null && bondedPawn_hediff_PsychicBond.target == target)
                {
                    removeList.Add(i);
                }
            }

            foreach (int hediffIndex in removeList)
            {
                Hediff hediff = pawn.health.hediffSet.hediffs[hediffIndex];
                pawn.health.RemoveHediff(hediff);
                Utils.LogM($"RemovePsychicBond -> Removing Psychic bond hediff from [{pawn.Name}] the bond was with [{target.Name}]");
            }
        }

        public static void AddHediff_PsychicBondTorn(this Pawn pawn, Pawn target)
        {
            Hediff_PsychicBondTorn hediff_PsychicBondTorn = null;
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                Hediff_PsychicBondTorn temp_hediff_PsychicBondTorn = hediff as Hediff_PsychicBondTorn;
                if (temp_hediff_PsychicBondTorn is not null && temp_hediff_PsychicBondTorn.target == target)
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
                if (bondedPawn_hediff_PsychicBondTorn is not null && bondedPawn_hediff_PsychicBondTorn.target == target)
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
    }
}
