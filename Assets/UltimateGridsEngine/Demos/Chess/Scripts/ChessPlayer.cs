using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ChessPlayer
{
    public List<GridObject> m_Pieces;

    public string name;
    public int forward;

    public ChessPlayer(string name, bool positiveZMovement)
    {
        this.name = name;
        m_Pieces = new List<GridObject>();

        if (positiveZMovement == true)
        {
            this.forward = 1;
        }
        else
        {
            this.forward = -1;
        }
    }
}
