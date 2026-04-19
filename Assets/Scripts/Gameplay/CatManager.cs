using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatManager : Singleton<CatManager>
{
    private Transform _spawn;
    private Transform _brokenRoot;
    private GameObject _brokenFurniturePrefab;
    private CatDefinition[] _normalCatDefinitions;
    private CatRequestDefinition[] _normalCatRequests;
    private FurnitureDefinition[] _furnitureDefinitions;

    public CatManager()
    {
        _spawn = GameObject.Find("Spawn").transform;
        _brokenRoot = GameObject.Find("BrokenRoot").transform;
        _brokenFurniturePrefab = Resources.Load<GameObject>("Prefab/BrokenFurniture");
        _normalCatDefinitions = Resources.LoadAll<CatDefinition>("CatDefinitions");
        _normalCatRequests = Resources.LoadAll<CatRequestDefinition>("CatRequests");
        _furnitureDefinitions = Resources.LoadAll<FurnitureDefinition>("FurnitureDefinitions");
        HideAllCats();
    }

    public void HideAllCats()
    {
        for (var i = 0; i < _spawn.childCount; i++)
        {
            _spawn.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ClearBrokenFurniture()
    {
        for (var i = _brokenRoot.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(_brokenRoot.GetChild(i).gameObject);
        }
    }

    public bool ShowHiddenCat(float catDuration)
    {
        var hiddenCats = new List<Transform>();
        for (var i = 0; i < _spawn.childCount; i++)
        {
            var hides = _spawn.GetChild(i);
            if (!hides.gameObject.activeSelf && !SpecialCatManager.Instance.IsSpawnOccupied(hides))
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
        var furniture = child.GetComponentInChildren<Furniture>(true);
        if (cat == null)
        {
            child.gameObject.SetActive(false);
            return false;
        }

        if (furniture == null)
        {
            child.gameObject.SetActive(false);
            return false;
        }

        var definition = GetRandomNormalCatDefinition();
        var request = GetRandomNormalCatRequest();
        var furnitureDefinition = GetRandomFurnitureDefinition();
        if (definition == null || request == null || furnitureDefinition == null)
        {
            child.gameObject.SetActive(false);
            return false;
        }

        furniture.ResetFurniture(furnitureDefinition);
        cat.Show(definition, request, furniture, catDuration);
        return true;
    }

    public void SpawnBrokenFurniture(Furniture furniture)
    {
        var sourceRenderer = furniture.SpriteRenderer;
        var brokenObject = Object.Instantiate(_brokenFurniturePrefab, _brokenRoot);
        brokenObject.name = $"{furniture.gameObject.name}_Broken";

        var brokenFurniture = brokenObject.GetComponent<BrokenFurniture>();
        brokenFurniture.Initialize(furniture.BrokenSprite, sourceRenderer, furniture.BrokenTargetY, furniture.BrokenFallSpeed);
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

    private FurnitureDefinition GetRandomFurnitureDefinition()
    {
        if (_furnitureDefinitions == null || _furnitureDefinitions.Length == 0)
        {
            return null;
        }

        return _furnitureDefinitions[Random.Range(0, _furnitureDefinitions.Length)];
    }
}
