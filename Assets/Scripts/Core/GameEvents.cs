using System;

public class GameEvents : Singleton<GameEvents>
{
    public Action<bool> OnCatInteraction;

    public GameEvents()
    {
        ClearAll();
    }

    private void ClearAll()
    {
        OnCatInteraction = null;
    }
}