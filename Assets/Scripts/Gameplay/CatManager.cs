using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatManager : Singleton<CatManager>
{
    private Transform _spawn;
    private Transform _brokenRoot;
    private CatManagerRuntime _runtime;
    private GameObject _brokenFurniturePrefab;
    private CatDefinition[] _normalCatDefinitions;
    private CatRequestDefinition[] _normalCatRequests;
    private FurnitureDefinition[] _furnitureDefinitions;
    private SpecialCatDefinition[] _specialCatDefinitions;
    private Coroutine _normalSpawnCoroutine;
    private Coroutine _specialSpawnCoroutine;
    private MainView _mainView;
    private SpecialCat _activeSpecialCat;
    private Transform _activeSpecialSpawnPoint;

    public CatManager()
    {
        _spawn = GameObject.Find("Spawn").transform;
        _brokenRoot = GameObject.Find("BrokenRoot").transform;
        _brokenFurniturePrefab = Resources.Load<GameObject>("Prefab/BrokenFurniture");
        _normalCatDefinitions = Resources.LoadAll<CatDefinition>("CatDefinitions");
        _normalCatRequests = Resources.LoadAll<CatRequestDefinition>("CatRequests");
        _furnitureDefinitions = Resources.LoadAll<FurnitureDefinition>("FurnitureDefinitions");
        _specialCatDefinitions = Resources.LoadAll<SpecialCatDefinition>("SpecialCatDefinitions");
        EnsureRuntime();
        HideAllCats();
    }

    public void StartGame(MainView mainView)
    {
        StopGame();
        _mainView = mainView;
        SpawnNormalCatOnce();
        _normalSpawnCoroutine = _runtime.StartCoroutine(RunNormalSpawnSchedule());
        _specialSpawnCoroutine = _runtime.StartCoroutine(RunSpecialSpawnSchedule());
    }

    public void StopGame()
    {
        StopNormalSpawnSchedule();
        StopSpecialSpawnSchedule();
        _mainView = null;
        _activeSpecialSpawnPoint = null;

        if (_activeSpecialCat != null)
        {
            _activeSpecialCat.transform.parent.gameObject.SetActive(false);
            _activeSpecialCat = null;
        }
    }

    public void NotifySpecialCatFinished(SpecialCat specialCat)
    {
        if (_activeSpecialCat == specialCat)
        {
            _activeSpecialCat = null;
            _activeSpecialSpawnPoint = null;
        }
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
        return SpawnNormalCatOnce(catDuration);
    }

    public void SpawnBrokenFurniture(Furniture furniture)
    {
        var sourceRenderer = furniture.SpriteRenderer;
        var brokenObject = Object.Instantiate(_brokenFurniturePrefab, _brokenRoot);
        brokenObject.name = $"{furniture.gameObject.name}_Broken";

        var brokenFurniture = brokenObject.GetComponent<BrokenFurniture>();
        brokenFurniture.Initialize(furniture.BrokenSprite, sourceRenderer, furniture.BrokenTargetY, furniture.BrokenFallSpeed);
    }

    private IEnumerator RunNormalSpawnSchedule()
    {
        while (_mainView != null && _mainView.IsGameRunning)
        {
            var waitTime = GameSettings.GameSetting.GetCatSpawnInterval(_mainView.RemainingTime);
            yield return new WaitForSeconds(waitTime);
            if (_mainView == null || !_mainView.IsGameRunning || _mainView.RemainingTime <= 0)
            {
                break;
            }

            SpawnNormalCatOnce();
        }

        _normalSpawnCoroutine = null;
    }

    private IEnumerator RunSpecialSpawnSchedule()
    {
        while (_mainView != null && _mainView.IsGameRunning)
        {
            var spawnInterval = GameSettings.GameSetting.GetSpecialCatSpawnInterval(_mainView.RemainingTime);
            if (spawnInterval < 0f)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(spawnInterval);
            if (_mainView == null || !_mainView.IsGameRunning || _mainView.RemainingTime <= 0)
            {
                break;
            }

            if (_activeSpecialCat != null)
            {
                continue;
            }

            var appearanceDuration = GameSettings.GameSetting.GetSpecialCatAppearanceDuration(_mainView.RemainingTime);
            var furnitureFailDuration = GameSettings.GameSetting.GetSpecialFurnitureFailDuration(_mainView.RemainingTime);
            if (appearanceDuration < 0f || furnitureFailDuration < 0f)
            {
                continue;
            }

            SpawnSpecialCat(appearanceDuration, furnitureFailDuration);
        }

        _specialSpawnCoroutine = null;
    }

    private bool SpawnNormalCatOnce()
    {
        if (_mainView == null)
        {
            return false;
        }

        return SpawnNormalCatOnce(GameSettings.GameSetting.GetCatAppearanceDuration(_mainView.RemainingTime));
    }

    private bool SpawnNormalCatOnce(float catDuration)
    {
        var hiddenCats = new List<Transform>();
        for (var i = 0; i < _spawn.childCount; i++)
        {
            var hides = _spawn.GetChild(i);
            if (!IsSpecialSpawnPoint(hides) && !hides.gameObject.activeSelf)
            {
                hiddenCats.Add(hides);
            }
        }

        if (hiddenCats.Count == 0)
        {
            return false;
        }

        var child = hiddenCats[Random.Range(0, hiddenCats.Count)];
        var spawnPos = child.GetComponent<SpawnPos>();
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
        var furnitureDefinition = GetRandomFurnitureDefinition(spawnPos);
        if (definition == null || request == null || furnitureDefinition == null)
        {
            child.gameObject.SetActive(false);
            return false;
        }

        furniture.ResetFurniture(furnitureDefinition);
        cat.Show(definition, request, furniture, catDuration);
        return true;
    }

    private void SpawnSpecialCat(float appearanceDuration, float furnitureFailDuration)
    {
        if (!TryGetAvailableSpecialCatGroup(out var specialCatGroup))
        {
            return;
        }

        var definition = GetRandomSpecialCatDefinition();
        if (definition == null)
        {
            return;
        }

        specialCatGroup.gameObject.SetActive(true);
        var specialCat = specialCatGroup.GetComponentInChildren<SpecialCat>(true);
        if (specialCat == null)
        {
            specialCatGroup.gameObject.SetActive(false);
            return;
        }

        specialCat.Show(definition, appearanceDuration, furnitureFailDuration);
        _activeSpecialCat = specialCat;
        _activeSpecialSpawnPoint = specialCatGroup;
    }

    private bool TryGetAvailableSpecialCatGroup(out Transform spawnTransform)
    {
        var candidates = new List<Transform>();
        for (var i = 0; i < _spawn.childCount; i++)
        {
            var child = _spawn.GetChild(i);
            if (!IsSpecialSpawnPoint(child))
            {
                continue;
            }

            if (child.gameObject.activeSelf || _activeSpecialSpawnPoint == child)
            {
                continue;
            }

            candidates.Add(child);
        }

        if (candidates.Count == 0)
        {
            spawnTransform = null;
            return false;
        }

        spawnTransform = candidates[Random.Range(0, candidates.Count)];
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

    private FurnitureDefinition GetRandomFurnitureDefinition(SpawnPos spawnPos)
    {
        if (spawnPos != null &&
            spawnPos.AllowedFurnitureDefinitions != null &&
            spawnPos.AllowedFurnitureDefinitions.Length > 0)
        {
            return spawnPos.AllowedFurnitureDefinitions[Random.Range(0, spawnPos.AllowedFurnitureDefinitions.Length)];
        }

        return GetRandomFurnitureDefinition();
    }

    private FurnitureDefinition GetRandomFurnitureDefinition()
    {
        if (_furnitureDefinitions == null || _furnitureDefinitions.Length == 0)
        {
            return null;
        }

        return _furnitureDefinitions[Random.Range(0, _furnitureDefinitions.Length)];
    }

    private SpecialCatDefinition GetRandomSpecialCatDefinition()
    {
        if (_specialCatDefinitions == null || _specialCatDefinitions.Length == 0)
        {
            return null;
        }

        return _specialCatDefinitions[Random.Range(0, _specialCatDefinitions.Length)];
    }

    private static bool IsSpecialSpawnPoint(Transform spawnTransform)
    {
        var spawnPos = spawnTransform.GetComponent<SpawnPos>();
        return spawnPos != null && spawnPos.IsSpecialOnly;
    }

    private void EnsureRuntime()
    {
        if (_runtime != null)
        {
            return;
        }

        var rootObject = new GameObject("CatManagerRuntime");
        var sceneRoot = GameObject.Find("SceneRoot");
        if (sceneRoot != null)
        {
            rootObject.transform.SetParent(sceneRoot.transform, false);
        }

        _runtime = rootObject.AddComponent<CatManagerRuntime>();
    }

    private void StopNormalSpawnSchedule()
    {
        if (_normalSpawnCoroutine == null || _runtime == null)
        {
            return;
        }

        _runtime.StopCoroutine(_normalSpawnCoroutine);
        _normalSpawnCoroutine = null;
    }

    private void StopSpecialSpawnSchedule()
    {
        if (_specialSpawnCoroutine == null || _runtime == null)
        {
            return;
        }

        _runtime.StopCoroutine(_specialSpawnCoroutine);
        _specialSpawnCoroutine = null;
    }

}
