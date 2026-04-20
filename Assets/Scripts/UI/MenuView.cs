using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuView : BaseView
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _endButton;

    private Slider _volumeSlider;

    private void Awake()
    {
        _volumeSlider = transform.Find("volume/Slider")?.GetComponent<Slider>();
        if (_volumeSlider != null)
        {
            _volumeSlider.SetValueWithoutNotify(AudioManager.Instance.MasterVolume);
            _volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        }

        _startButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            GameManager.Instance.OpenView<GuideView>();
        });
        _endButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        });
    }
}
