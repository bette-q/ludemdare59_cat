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
    [SerializeField] private Image _readyGoImage;
    [SerializeField] private float _readyGoDuration = 1f;

    public E_CatItem CurCatItem { get; private set; }
    public bool IsDragging { get; private set; }
    private GameObject _curDragItem;

    private Coroutine _countDownCoroutine;
    private Coroutine _startGameCoroutine;

    private int _successCount;
    private int _failCount;
    private int _remainingTime;
    private bool _isGameRunning;

    public int RemainingTime => _remainingTime;
    public bool IsGameRunning => _isGameRunning;

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
        AudioManager.Instance.PlayClick();
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
        if (IsDragging)
        {
            CatManager.Instance.TryReceiveItemAtScreenPosition(ev.position, CurCatItem);
        }

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
        StartGameSequence();
    }

    private void StartGameSequence()
    {
        StopStartGameSequence();
        StopCountDown();
        CatManager.Instance.StopGame();
        ClearDragState();
        CatManager.Instance.HideAllCats();
        CatManager.Instance.ClearBrokenFurniture();
        _successCount = 0;
        _failCount = 0;
        _remainingTime = Setting.CountDownTime;
        _isGameRunning = false;
        _successText.SetText(_successCount.ToString());
        _failText.SetText(_failCount.ToString());
        _timeText.SetText(_remainingTime.ToString());
        _readyGoImage.gameObject.SetActive(false);

        _startGameCoroutine = StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        _readyGoImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(_readyGoDuration);
        _readyGoImage.gameObject.SetActive(false);

        _startGameCoroutine = null;
        GameStart();
    }

    private void GameStart()
    {
        _isGameRunning = true;
        AudioManager.Instance.PlayInGameMusic();
        CatManager.Instance.StartGame(this);
        _countDownCoroutine = StartCoroutine(CountDown());
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
        _timeText.SetText("0");
        EndGame();
    }

    private void EndGame()
    {
        _isGameRunning = false;
        StopStartGameSequence();
        ClearDragState();
        CatManager.Instance.StopGame();
        CatManager.Instance.HideAllCats();
        GameManager.Instance.OpenView<EndView>();
        var view = GameManager.Instance.GetView<EndView>();
        view.Show(_successCount, _failCount);
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

    private void StopStartGameSequence()
    {
        if (_startGameCoroutine == null)
        {
            return;
        }

        StopCoroutine(_startGameCoroutine);
        _startGameCoroutine = null;
    }


    private void OnDestroy()
    {
        CatManager.Instance.StopGame();
        ClearDragState();
        StopStartGameSequence();
        StopCountDown();
    }
}
