
using System;
using UnityEngine;

[System.Serializable]
public struct GameStateEvent
{
    public bool transitioning;
    public Game_States.STATE nextState;
    public bool clearPreviousState;
}
