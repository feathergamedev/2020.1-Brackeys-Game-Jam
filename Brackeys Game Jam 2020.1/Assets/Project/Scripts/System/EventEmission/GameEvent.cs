using System.Collections.Generic;

public enum GameEvent
{
    None,
    AppOpen,
    LoadHomeMenu,
    GameInitialize,
    LevelStart,
    LevelReload,
    LevelComplete,
    GamePause,
    GameResume,
}

public class IntEvent : IEvent
{
    public int Value
    {
        get;
    }

    public IntEvent(int value)
    {
        Value = value;
    }
}

public class BoolEvent : IEvent
{
    public bool Value
    {
        get;
    }

    public BoolEvent(bool value)
    {
        Value = value;
    }
}