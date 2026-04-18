using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum E_CatItem
{
    Food,
    Petting,
    Toy,
}

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
    private Coroutine _showCatCoroutine;

    private int _successCount;
    private int _failCount;

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
        }
    }

    public void ShowBeforeStart()
    {
        GameStart();
    }

    private void GameStart()
    {
        if (_countDownCoroutine != null)
        {
            StopCoroutine(_countDownCoroutine);
        }
        _successCount = 0;
        _failCount = 0;
        _successText.SetText(_successCount.ToString());
        _failText.SetText(_failCount.ToString());
        _countDownCoroutine = StartCoroutine(CountDown());
        StopShowCat();
        ShowCat();
    }

    public void ShowCat()
    {
        CatManager.Instance.ShowCat();
        StopShowCat();
        _showCatCoroutine = StartCoroutine(ShowCatTime());
    }

    public void StopShowCat()
    {
        if (_showCatCoroutine != null)
        {
            StopCoroutine(_showCatCoroutine);
        }
    }

    private IEnumerator ShowCatTime()
    {
        yield return new WaitForSeconds(Setting.CatAppearanceDuration);
        GameEvents.Instance.OnCatInteraction?.Invoke(false);
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
        var time = Setting.CountDownTime;
        while (time > 0)
        {
            _timeText.SetText(time.ToString());
            yield return new WaitForSeconds(1f);
            time--;
        }

        _timeText.SetText("0");
    }


    private void OnDestroy()
    {
        Clear();

        if (_countDownCoroutine != null)
        {
            StopCoroutine(_countDownCoroutine);
            _countDownCoroutine = null;
        }

        if (_showCatCoroutine != null)
        {
            StopCoroutine(_showCatCoroutine);
        }
    }
}