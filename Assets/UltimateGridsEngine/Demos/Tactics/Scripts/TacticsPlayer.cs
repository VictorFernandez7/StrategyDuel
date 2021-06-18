using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TacticsPlayer
{
    public List<TacticsCharacter> m_Units;

    public string m_PlayerName;

    public TacticsPlayer(string name)
    {
        m_PlayerName = name;
        m_Units = new List<TacticsCharacter>();
    }
}
