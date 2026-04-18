using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainView : BaseView
{
    [SerializeField] private List<GameObject> _eventTriggerList;
    [SerializeField] private List<GameObject> _itemList;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _successText;
    [SerializeField] private TMP_Text _failText;

    public E_CatItem CurCatItem { get; private set; }
    public bool IsDragging { get; private set; }
    private GameObject _curDragItem;

    private Coroutine _countDownCoroutine;
    private Coroutine _spawnCatCoroutine;

    private int _successCount;
    private int _failCount;
    private int _remainingTime;
    private bool _isGameRunning;

    private void Awake()
    {
        for (var i = 0; i < _eventTriggerList.Count; i++)
        {
            var index = i;
            var trigger = EventTriggerListener.Get(_eventTriggerList[index]);
            trigger.OnDragBegin += (go, ev) => { OnDragBegin((E_CatItem)index, go, ev); };
            trigger.OnDragEvent += (go, ev) => { OnDragEvent((E_CatItem)index, go, ev); };
            trigger.OnDragEnd += (go, ev) => { OnDragEnd((E_CatItem)index, go, ev); };
        }
    }

    private void OnDragBegin(E_CatItem itemType, GameObject go, PointerEventData ev)
    {
        Clear();
        IsDragging = true;
        CurCatItem = itemType;
        _curDragItem = Instantiate(_itemList[(int)CurCatItem], ev.position, Quaternion.identity);
        _curDragItem.GetComponent<Image>().raycastTarget = false;
        _curDragItem.transform.SetParent(transform);
    }

    private void OnDragEvent(E_CatItem itemType, GameObject go, PointerEventData ev)
    {
        if (!IsDragging) return;
        _curDragItem.transform.position = ev.position;
    }

    private void OnDragEnd(E_CatItem itemType, GameObject go, PointerEventData ev)
    {
        IsDragging = false;
        Clear();
    }

    private void Clear()
    {
        if (_curDragItem != null)
        {
            Destroy(_curDragItem);
            _curDragItem = null;
        }
    }

    public void ShowBeforeStart()
    {
        GameStart();
    }

    private void GameStart()
    {
        StopCountDown();
        StopSpawnCat();
        ClearDragState();
        CatManager.Instance.HideAllCats();
        _successCount = 0;
        _failCount = 0;
        _remainingTime = Setting.CountDownTime;
        _isGameRunning = true;
        _successText.SetText(_successCount.ToString());
        _failText.SetText(_failCount.ToString());
        _timeText.SetText(_remainingTime.ToString());
        SpawnCatOnce();
        _countDownCoroutine = StartCoroutine(CountDown());
        _spawnCatCoroutine = StartCoroutine(SpawnCat());
    }

    public void UpdateSuccess()
    {
        _successCount++;
        _successText.SetText(_successCount.ToString());
    }

    public void UpdateFail()
    {
        _failCount++;
        _failText.SetText(_failCount.ToString());
    }

    private IEnumerator CountDown()
    {
        var time = _remainingTime;
        while (time > 0)
        {
            _remainingTime = time;
            _timeText.SetText(time.ToString());
            yield return new WaitForSeconds(1f);
            time--;
        }

        _remainingTime = 0;
        _isGameRunning = false;
        _timeText.SetText("0");
        StopSpawnCat();
        ClearDragState();
        CatManager.Instance.HideAllCats();
    }

    private IEnumerator SpawnCat()
    {
        while (_isGameRunning)
        {
            var waitTime = Setting.GetCatSpawnInterval(_remainingTime);
            yield return new WaitForSeconds(waitTime);
            if (!_isGameRunning || _remainingTime <= 0)
            {
                yield break;
            }

            SpawnCatOnce();
        }
    }

    private void SpawnCatOnce()
    {
        CatManager.Instance.ShowHiddenCat(Setting.GetCatAppearanceDuration(_remainingTime));
    }

    public bool TryConsumeDrag(out E_CatItem catItem)
    {
        catItem = CurCatItem;
        if (!IsDragging)
        {
            return false;
        }

        IsDragging = false;
        Clear();
        return true;
    }

    private void ClearDragState()
    {
        IsDragging = false;
        Clear();
    }

    private void StopCountDown()
    {
        if (_countDownCoroutine == null)
        {
            return;
        }

        StopCoroutine(_countDownCoroutine);
        _countDownCoroutine = null;
    }

    private void StopSpawnCat()
    {
        if (_spawnCatCoroutine == null)
        {
            return;
        }

        StopCoroutine(_spawnCatCoroutine);
        _spawnCatCoroutine = null;
    }


    private void OnDestroy()
    {
        ClearDragState();
        StopCountDown();
        StopSpawnCat();
    }
}
