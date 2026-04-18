using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum E_CatType
{
    Cat1,
    Cat2,
    Cat3,
    Cat4,
}

public class Cat : MonoBehaviour
{
    private E_CatItem _currentCatItem;
    private Coroutine _hideCoroutine;
    private bool _isResolved;

    private void OnEnable()
    {
        var listener = EventTriggerListener.Get(gameObject);
        listener.OnDropEvent -= OnDrop;
        listener.OnDropEvent += OnDrop;
    }

    private void OnDisable()
    {
        var listener = EventTriggerListener.Get(gameObject);
        listener.OnDropEvent -= OnDrop;
        StopHideCoroutine();
        _isResolved = false;
    }

    private void OnDrop(GameObject go, PointerEventData ev)
    {
        var view = GameManager.Instance.GetView<MainView>();
        if (view == null || _isResolved || !view.TryConsumeDrag(out var catItem))
            return;
        Debug.Log($"拖动{catItem}到{go.name}");
        Resolve(catItem == _currentCatItem);
    }

    public void Show(E_CatItem catItem, float duration)
    {
        _currentCatItem = catItem;
        _isResolved = false;
        StopHideCoroutine();
        _hideCoroutine = StartCoroutine(HideAfter(duration));
    }

    private IEnumerator HideAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        Resolve(false);
    }

    private void Resolve(bool isSuccess)
    {
        if (_isResolved)
        {
            return;
        }

        _isResolved = true;
        StopHideCoroutine();

        var view = GameManager.Instance.GetView<MainView>();
        if (isSuccess)
        {
            view.UpdateSuccess();
        }
        else
        {
            view.UpdateFail();
        }
        transform.parent.gameObject.SetActive(false);
    }

    private void StopHideCoroutine()
    {
        if (_hideCoroutine == null)
        {
            return;
        }

        StopCoroutine(_hideCoroutine);
        _hideCoroutine = null;
    }
}