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

    private bool _petNextIsFirstSound = true;
    private bool _foodNextIsFirstSound = true;
    private bool _toyNextIsFirstSound = true;

    private void Awake()
    {
        _startButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            GameManager.Instance.OpenView<MainView>();
            var view = GameManager.Instance.GetView<MainView>();
            view.ShowBeforeStart();
        });
        _petButton.onClick.AddListener(OnPetButtonClicked);
        _foodButton.onClick.AddListener(OnFoodButtonClicked);
        _toyButton.onClick.AddListener(OnToyButtonClicked);
    }

    private void OnPetButtonClicked()
    {
        AudioManager.Instance.PlayClick();

        if (_petNextIsFirstSound)
        {
            AudioManager.Instance.PlayAttention1();
        }
        else
        {
            AudioManager.Instance.PlayAttention2();
        }

        _petNextIsFirstSound = !_petNextIsFirstSound;
    }

    private void OnFoodButtonClicked()
    {
        AudioManager.Instance.PlayClick();

        if (_foodNextIsFirstSound)
        {
            AudioManager.Instance.PlayFood1();
        }
        else
        {
            AudioManager.Instance.PlayFood2();
        }

        _foodNextIsFirstSound = !_foodNextIsFirstSound;
    }

    private void OnToyButtonClicked()
    {
        AudioManager.Instance.PlayClick();

        if (_toyNextIsFirstSound)
        {
            AudioManager.Instance.PlayToy1();
        }
        else
        {
            AudioManager.Instance.PlayToy2();
        }

        _toyNextIsFirstSound = !_toyNextIsFirstSound;
    }
}
