using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private E_CatType _catType;

    private E_CatItem _currentCatItem;


    private void OnCatInteraction(bool isSuccess)
    {
        var view = GameManager.Instance.GetView<MainView>();
        if (isSuccess)
        {
            view.UpdateSuccess();
        }
        else
        {
            view.UpdateFail();
        }

        view.StopShowCat();
        view.ShowCat();
    }

    private void OnEnable()
    {
        var listener = EventTriggerListener.Get(gameObject);
        listener.OnDropEvent -= OnDrop;
        listener.OnDropEvent += OnDrop;

        GameEvents.Instance.OnCatInteraction -= OnCatInteraction;
        GameEvents.Instance.OnCatInteraction += OnCatInteraction;
    }

    private void OnDisable()
    {
        var listener = EventTriggerListener.Get(gameObject);
        listener.OnDropEvent -= OnDrop;

        GameEvents.Instance.OnCatInteraction -= OnCatInteraction;
    }

    private void OnDrop(GameObject go, PointerEventData ev)
    {
        if (GameManager.Instance.GetView<MainView>() is not { IsDragging: true } view)
            return;
        Debug.Log($"拖动{view.CurCatItem}到{go.name}");
        OnCatInteraction(view.CurCatItem == _currentCatItem);
    }

    public void SetCurCatItem(E_CatItem catItem)
    {
        _currentCatItem = catItem;
    }
}