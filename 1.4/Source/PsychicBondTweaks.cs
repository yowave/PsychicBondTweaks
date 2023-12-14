using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace PsychicBondTweaks
{
    public class PsychicBondTweaks : Mod
    {
        public static string ModName { get; private set; }
        public static string PackageId { get; private set; }

        public static PsychicBondTweaks Instance { get; private set; }

        public static Harmony Harmony { get; private set; }
        public static bool Patched { get; private set; }

        public PsychicBondTweaks(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();
            Instance = this;
            ModName = content.Name;
            PackageId = content.PackageId;
            Harmony = new Harmony(PackageId);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            Settings.ApplySettings();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return ModName;
        }

        public static void TryPerformPatches()
        {
            if (!Patched)
            {
                try
                {
                    Harmony.PatchAll();
                    Patched = true;
                    Utils.LogM($"patched successfully...");
                }
                catch (Exception ex) { Utils.LogE($"{ex}"); }
            }
        }
    }
}
