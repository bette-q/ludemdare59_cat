using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideView : BaseView
{
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        _startButton.onClick.AddListener(() =>
        {
            GameManager.Instance.OpenView<MainView>();
            var view = GameManager.Instance.GetView<MainView>();
            view.ShowBeforeStart();
        });
    }
}