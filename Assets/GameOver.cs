using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    Text title;
    private GameOverState state;

    public enum GameOverState
    {
        Undecided,
        Win,
        Lose,
        Tie
    }

    void Start()
    {
        gameObject.SetActive(false);
        Transform titlePanel = transform.Find("TitleBackground");
        title = titlePanel.Find("Text").GetComponent<Text>();
        state = GameOverState.Undecided;
    }

    public void UpdateState(GameOverState state)
    {
        this.state = state;

        if(BattleManager.CurrentBattleState == BattleState.GameOver)
        {
            switch (state)
            {
                case GameOverState.Win:
                    title.text = "Battle Won";
                    title.color = Color.green;
                    break;
                case GameOverState.Lose:
                    title.text = "Battle Lost";
                    title.color = Color.red;
                    break;
                case GameOverState.Tie:
                    title.text = "Tie";
                    title.color = Color.red + Color.yellow;
                    break;
                default:
                    title.text = "Undefined";
                    Debug.Log("GameOver state not decided.");
                    break;
            }
        }
    }

    public void OnReplayDown()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void OnBackToMenuDown()
    {
        Application.LoadLevel(0);
    }
}
