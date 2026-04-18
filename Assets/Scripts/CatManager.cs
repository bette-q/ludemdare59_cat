using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatManager : Singleton<CatManager>
{
    private int _lastIndex;
    private Transform _spawn;

    public CatManager()
    {
        _spawn = GameObject.Find("Spawn").transform;
        _lastIndex = -1;
        for (var i = 0; i < _spawn.childCount; i++)
        {
            _spawn.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ShowCat()
    {
        for (var i = 0; i < _spawn.childCount; i++)
        {
            _spawn.GetChild(i).gameObject.SetActive(false);
        }

        var list = Enumerable.Range(0, _spawn.childCount).ToList();
        if (_lastIndex != -1)
        {
            list.Remove(_lastIndex);
        }

        var index = list[Random.Range(0, list.Count)];
        _lastIndex = index;
        var child = _spawn.GetChild(index);
        child.gameObject.SetActive(true);
        child.GetComponentInChildren<Cat>(true)
            .SetCurCatItem((E_CatItem)Random.Range(0, Enum.GetValues(typeof(E_CatItem)).Length));
    }
}