using System;

public class GameEvents : Singleton<GameEvents>
{
    public Action<bool> OnCatInteraction;
    public Action<bool, float> OnSpecialCatResolved;

    public GameEvents()
    {
        ClearAll();
    }

    private void ClearAll()
    {
        OnCatInteraction = null;
        OnSpecialCatResolved = null;
    }
}
