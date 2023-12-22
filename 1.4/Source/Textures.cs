using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PsychicBondTweaks
{
    [StaticConstructorOnStartup]
    internal static class Textures
    {
        public static readonly Texture2D TearBondTexture = TearBondTexture = ContentFinder<Texture2D>.Get("UI/Commands/TearBond");
    }
}
