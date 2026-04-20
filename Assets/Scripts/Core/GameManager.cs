using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<System.Type, BaseView> _allViews = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BeforeSceneLoad()
    {
        AudioManager.Instance.PlayMusic();
        SetGameCursor();

        foreach (var view in Object.FindObjectsOfType<BaseView>(true))
        {
            view.gameObject.SetActive(view is MenuView);
        }
    }

    private static void SetGameCursor()
    {
        var cursorTexture = Resources.Load<Texture2D>("handle");
        Cursor.SetCursor(cursorTexture, new Vector2(4f, 4f), CursorMode.Auto);
    }

    public GameManager()
    {
        foreach (var view in Object.FindObjectsOfType<BaseView>(true))
        {
            _allViews.Add(view.GetType(), view);
        }
    }

    public T GetView<T>() where T : BaseView
    {
        return _allViews.TryGetValue(typeof(T), out var view) ? (T)view : null;
    }

    public void OpenView<T>() where T : BaseView
    {
        if (typeof(T) == typeof(MenuView) || typeof(T) == typeof(GuideView))
        {
            AudioManager.Instance.PlayMenuMusic();
        }
        else if (typeof(T) == typeof(MainView))
        {
            AudioManager.Instance.StopAndResetMusic();
        }

        foreach (var kvp in _allViews)
        {
            kvp.Value.gameObject.SetActive(kvp.Value is T);
        }
    }

}
