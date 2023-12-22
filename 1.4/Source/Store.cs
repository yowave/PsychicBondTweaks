using Verse;
using System.Collections.Generic;
using RimWorld;
using System.Linq;
using UnityEngine;

namespace PsychicBondTweaks
{
    internal class Store : GameComponent
    {
        public static Dictionary<string, bool> psychicBondToggle = new Dictionary<string, bool>();

        public Store(Game game)
        {
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            HashSet<string> pawnIdList = new HashSet<string>();
            foreach (Pawn pawn in PawnsFinder.AllMaps)
            {
                pawnIdList.Add(pawn.ThingID);
            }

            List<string> testExposeDictKeys = new List<string>(psychicBondToggle.Keys);
            foreach (string pawnID in testExposeDictKeys)
            {
                if (!pawnIdList.Contains(pawnID))
                {
                    psychicBondToggle.Remove(pawnID);
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref psychicBondToggle, "psychicBondToggle");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (psychicBondToggle == null) psychicBondToggle = new Dictionary<string, bool>();
            }
        }
    }
}
