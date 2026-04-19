using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideView : BaseView
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _petButton;
    [SerializeField] private Button _foodButton;
    [SerializeField] private Button _toyButton;

    private void Awake()
    {
        _startButton.onClick.AddListener(() =>
        {
            GameManager.Instance.OpenView<MainView>();
            var view = GameManager.Instance.GetView<MainView>();
            view.ShowBeforeStart();
        });
        _petButton.onClick.AddListener(() => { AudioManager.Instance.PlayCatRequest(E_CatRequestSound.Petting); });
        _foodButton.onClick.AddListener(() => { AudioManager.Instance.PlayCatRequest(E_CatRequestSound.Food); });
        _toyButton.onClick.AddListener(() => { AudioManager.Instance.PlayCatRequest(E_CatRequestSound.Toy); });
    }
}
