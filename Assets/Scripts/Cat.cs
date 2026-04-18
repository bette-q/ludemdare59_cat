using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public enum E_CatType
{
}

public class Cat : MonoBehaviour
{
    private void Awake()
    {
        EventTriggerListener.Get(gameObject).OnDropEvent += OnDrop;
    }

    private void OnDrop(GameObject go, PointerEventData ev)
    {
        if (GameManager.Instance.GetView<MainView>() is not { IsDragging: true } view)
            return;
        Debug.Log($"拖动{view.CurCatItem}到{go.name}");
    }
}