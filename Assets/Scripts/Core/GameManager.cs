using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<System.Type, BaseView> _allViews = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BeforeSceneLoad()
    {
        foreach (var view in Object.FindObjectsOfType<BaseView>(true))
        {
            view.gameObject.SetActive(view is MenuView);
        }
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
        foreach (var kvp in _allViews)
        {
            kvp.Value.gameObject.SetActive(kvp.Value is T);
        }
    }

    public void OpenEndView(int goodScore, int badScore)
    {
        OpenView<EndView>();
        GetView<EndView>().Show(goodScore, badScore);
    }
}
