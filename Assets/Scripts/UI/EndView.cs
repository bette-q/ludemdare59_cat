using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndView : BaseView
{
    [SerializeField] private TMP_Text _goodScoreText;
    [SerializeField] private TMP_Text _badScoreText;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _restartButton;

    private void Awake()
    {
        _returnButton.onClick.AddListener(() => { GameManager.Instance.OpenView<MenuView>(); });
        _restartButton.onClick.AddListener(RestartGame);
    }

    public void Show(int goodScore, int badScore)
    {
        _goodScoreText.SetText(goodScore.ToString());
        _badScoreText.SetText(badScore.ToString());

        if (goodScore >= badScore)
        {
            AudioManager.Instance.PlayVictory();
        }
        else if (badScore > goodScore)
        {
            AudioManager.Instance.PlayFailure();
        }
    }

    private static void RestartGame()
    {
        GameManager.Instance.OpenView<MainView>();
        GameManager.Instance.GetView<MainView>().ShowBeforeStart();
    }
}
