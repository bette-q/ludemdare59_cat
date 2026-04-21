using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndView : BaseView
{
    [SerializeField] private TMP_Text _goodScoreText;
    [SerializeField] private TMP_Text _badScoreText;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Image _resultImage;
    [SerializeField] private Sprite _successSprite;
    [SerializeField] private Sprite _failureSprite;

    private void Awake()
    {
        _returnButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            AudioManager.Instance.RestartMenuMusic();
            GameManager.Instance.OpenView<MenuView>();
        });
        _restartButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClick();
            RestartGame();
        });
    }

    public void Show(int goodScore, int badScore)
    {
        _goodScoreText.SetText(goodScore.ToString());
        _badScoreText.SetText(badScore.ToString());

        if (goodScore >= badScore)
        {
            _resultImage.sprite = _successSprite;
            AudioManager.Instance.PlayVictory();
        }
        else if (badScore > goodScore)
        {
            _resultImage.sprite = _failureSprite;
            AudioManager.Instance.PlayFailure();
        }
    }

    private static void RestartGame()
    {
        GameManager.Instance.OpenView<MainView>();
        GameManager.Instance.GetView<MainView>().ShowBeforeStart();
    }
}
