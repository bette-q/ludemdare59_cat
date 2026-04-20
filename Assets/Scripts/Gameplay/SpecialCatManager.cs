using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpecialCatManager : Singleton<SpecialCatManager>
{
    private struct SpawnPointInfo
    {
        public Transform Transform;
        public SpawnPos SpawnPos;
    }

    private Transform _root;
    private SpecialCatManagerRuntime _runtime;
    private GameObject _specialCatPrefab;
    private SpecialCatDefinition[] _specialCatDefinitions;
    private readonly List<SpawnPointInfo> _spawnPoints = new();
    private Coroutine _scheduleCoroutine;
    private MainView _mainView;
    private SpecialCat _activeSpecialCat;
    private Transform _activeSpawnPoint;

    public SpecialCatManager()
    {
        EnsureRuntime();
        _specialCatPrefab = Resources.Load<GameObject>("Prefab/SpecialCatGroup");
        _specialCatDefinitions = Resources.LoadAll<SpecialCatDefinition>("SpecialCatDefinitions");
        CacheSpawnPositions();
    }

    public void StartGame(MainView mainView)
    {
        StopGame();
        _mainView = mainView;
        _scheduleCoroutine = _runtime.StartCoroutine(RunSchedule());
    }

    public void StopGame()
    {
        if (_scheduleCoroutine != null && _runtime != null)
        {
            _runtime.StopCoroutine(_scheduleCoroutine);
            _scheduleCoroutine = null;
        }

        _mainView = null;

        if (_activeSpecialCat != null)
        {
            Object.Destroy(_activeSpecialCat.transform.parent.gameObject);
            _activeSpecialCat = null;
        }

        _activeSpawnPoint = null;

        if (_root != null)
        {
            for (var i = _root.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_root.GetChild(i).gameObject);
            }
        }
    }

    public void NotifySpecialCatFinished(SpecialCat specialCat)
    {
        if (_activeSpecialCat == specialCat)
        {
            _activeSpecialCat = null;
            _activeSpawnPoint = null;
        }
    }

    public bool IsSpawnOccupied(Transform spawnTransform)
    {
        return spawnTransform != null && _activeSpawnPoint == spawnTransform;
    }

    private IEnumerator RunSchedule()
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

        _scheduleCoroutine = null;
    }

    private void SpawnSpecialCat(float appearanceDuration, float furnitureFailDuration)
    {
        if (_specialCatPrefab == null)
        {
            return;
        }

        if (!TryGetAvailableSpecialSpawnPoint(out var spawnPoint))
        {
            return;
        }

        var definition = GetRandomSpecialCatDefinition();
        if (definition == null)
        {
            return;
        }

        var specialCatGroup = Object.Instantiate(_specialCatPrefab, _root);
        specialCatGroup.name = _specialCatPrefab.name;
        specialCatGroup.transform.position = spawnPoint.position;
        specialCatGroup.SetActive(true);

        var specialCat = specialCatGroup.GetComponentInChildren<SpecialCat>(true);
        if (specialCat == null)
        {
            Object.Destroy(specialCatGroup);
            return;
        }

        specialCat.Show(definition, appearanceDuration, furnitureFailDuration);
        _activeSpecialCat = specialCat;
        _activeSpawnPoint = spawnPoint;
    }

    private SpecialCatDefinition GetRandomSpecialCatDefinition()
    {
        if (_specialCatDefinitions == null || _specialCatDefinitions.Length == 0)
        {
            return null;
        }

        return _specialCatDefinitions[Random.Range(0, _specialCatDefinitions.Length)];
    }

    private bool TryGetAvailableSpecialSpawnPoint(out Transform spawnTransform)
    {
        var candidates = new List<SpawnPointInfo>();
        for (var i = 0; i < _spawnPoints.Count; i++)
        {
            var point = _spawnPoints[i];
            if (point.Transform == null || point.SpawnPos == null)
            {
                continue;
            }

            if (!point.SpawnPos.CanSpawnSpecial)
            {
                continue;
            }

            if (point.Transform.gameObject.activeSelf || IsSpawnOccupied(point.Transform))
            {
                continue;
            }

            candidates.Add(point);
        }

        if (candidates.Count == 0)
        {
            spawnTransform = null;
            return false;
        }

        var selected = candidates[Random.Range(0, candidates.Count)];
        spawnTransform = selected.Transform;
        return true;
    }

    private void EnsureRuntime()
    {
        if (_runtime != null)
        {
            return;
        }

        var rootObject = new GameObject("SpecialCatRoot");
        var sceneRoot = GameObject.Find("SceneRoot");
        if (sceneRoot != null)
        {
            rootObject.transform.SetParent(sceneRoot.transform, false);
        }

        _root = rootObject.transform;
        _runtime = rootObject.AddComponent<SpecialCatManagerRuntime>();
    }

    private void CacheSpawnPositions()
    {
        _spawnPoints.Clear();
        var spawnRoot = GameObject.Find("Spawn");
        if (spawnRoot == null)
        {
            return;
        }

        for (var i = 0; i < spawnRoot.transform.childCount; i++)
        {
            var child = spawnRoot.transform.GetChild(i);
            var spawnPos = child.GetComponent<SpawnPos>();
            _spawnPoints.Add(new SpawnPointInfo
            {
                Transform = child,
                SpawnPos = spawnPos
            });
        }
    }
}
