using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace Game.Data
{
    [Serializable]
    public class MainData
    {
        public enum CharacterName
        {
            Archer,
            Commander,
            CrossbowMan,
            Halberdier,
            HeavyCavalry,
            HeavyInfantry,
            HeavySwordman,
            HightPriest,
            King,
            LightCavalry,
            LightInfantry,
            Mage,
            MountedKing,
            MountedKnight,
            MountedMage,
            MountedPaladin,
            MountedPriest,
            MountedScout,
            Paladin,
            Priest,
            Scout,
            Spearman,
            Swordman
        }

        public enum Players
        {
            Player,
            Opponent
        }
    }
}