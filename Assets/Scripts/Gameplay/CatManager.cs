using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatManager : Singleton<CatManager>
{
    private Transform _spawn;

    public CatManager()
    {
        _spawn = GameObject.Find("Spawn").transform;
        HideAllCats();
    }

    public void HideAllCats()
    {
        for (var i = 0; i < _spawn.childCount; i++)
        {
            _spawn.GetChild(i).gameObject.SetActive(false);
        }
    }

    public bool ShowHiddenCat(float catDuration)
    {
        var hiddenCats = new List<Transform>();
        for (var i = 0; i < _spawn.childCount; i++)
        {
            var hides = _spawn.GetChild(i);
            if (!hides.gameObject.activeSelf)
            {
                hiddenCats.Add(hides);
            }
        }

        if (hiddenCats.Count == 0)
        {
            return false;
        }

        var child = hiddenCats[Random.Range(0, hiddenCats.Count)];
        child.gameObject.SetActive(true);
        var cat = child.GetComponentInChildren<Cat>(true);
        if (cat == null)
        {
            child.gameObject.SetActive(false);
            return false;
        }

        var request = GameSettings.GameSetting.GetRandomNormalCatRequest();
        if (request == null)
        {
            child.gameObject.SetActive(false);
            return false;
        }

        cat.Show(request, catDuration);
        return true;
    }
}
