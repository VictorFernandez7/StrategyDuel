using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Army", menuName = "Army/New Army", order = 1)]

    public class ArmyScriptableObject : ScriptableObject
    {
        [Title("Common Data")]
        [SerializeField] public GameObject playableCharacter; 
        [SerializeField] public GameObject characterCard;

        [Title("Caharacter Data")]
        public List<Character> characters;
    }

    [Serializable]
    public class Character
    {
        [Title("Character Type")]
        [SerializeField] [HideLabel] public MainData.CharacterName name;
        [FoldoutGroup("Parameters")] [SerializeField] public string cardName;
        [FoldoutGroup("Parameters")] [SerializeField] [TextArea] public string description;
        [FoldoutGroup("Parameters")] [SerializeField] public Sprite classIcon;
        [FoldoutGroup("Parameters")] [SerializeField] [Range(0, 3)] public int cost;
        [FoldoutGroup("Parameters")] [SerializeField] [Range(1, 3)] public int healthPoints;
        [FoldoutGroup("Parameters")] [SerializeField] [Range(1, 3)] public int armorPoints;
        [FoldoutGroup("Parameters")] [SerializeField] [Range(1, 3)] public int attackPoints;
        [FoldoutGroup("Parameters")] [SerializeField] [Range(1, 3)] public int movementPoints;
        [FoldoutGroup("Parameters")] [SerializeField] [Range(1, 3)] public int attacks;
        [FoldoutGroup("Parameters")] [SerializeField] [Range(1, 3)] public int attackRange;
    }
}