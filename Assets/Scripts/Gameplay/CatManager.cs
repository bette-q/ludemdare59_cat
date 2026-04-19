using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatManager : Singleton<CatManager>
{
    private Transform _spawn;
    private CatDefinition[] _normalCatDefinitions;
    private CatRequestDefinition[] _normalCatRequests;

    public CatManager()
    {
        _spawn = GameObject.Find("Spawn").transform;
        _normalCatDefinitions = Resources.LoadAll<CatDefinition>("CatDefinitions");
        _normalCatRequests = Resources.LoadAll<CatRequestDefinition>("CatRequests");
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

        var definition = GetRandomNormalCatDefinition();
        var request = GetRandomNormalCatRequest();
        if (definition == null || request == null)
        {
            child.gameObject.SetActive(false);
            return false;
        }

        cat.Show(definition, request, catDuration);
        return true;
    }

    private CatDefinition GetRandomNormalCatDefinition()
    {
        if (_normalCatDefinitions == null || _normalCatDefinitions.Length == 0)
        {
            return null;
        }

        return _normalCatDefinitions[Random.Range(0, _normalCatDefinitions.Length)];
    }

    private CatRequestDefinition GetRandomNormalCatRequest()
    {
        if (_normalCatRequests == null || _normalCatRequests.Length == 0)
        {
            return null;
        }

        return _normalCatRequests[Random.Range(0, _normalCatRequests.Length)];
    }
}
