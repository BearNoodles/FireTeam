using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public static bool IsPaused { get { return Time.timeScale == 0; } }

	private GameManager gm;
    private Transform instructionPanel;
    private Transform battleStartMessage;
	private Transform optionsMenu;
    private Transform pauseButton;
    private Text pauseText;

    void Start()
    {
		gm = GameManager.instance;

        // Don't display when the level first starts
        gameObject.SetActive(false);
        instructionPanel = transform.parent.Find("InstructionPanel");
        battleStartMessage = transform.parent.Find("BattleStartMessage");
		optionsMenu = transform.parent.Find("OptionsMenu");
        pauseButton = transform.parent.Find("PauseButton");
        pauseText = pauseButton.GetComponentInChildren<Text>();

        pauseButton.GetComponent<Button>().onClick.AddListener(Toggle);
    }

    public void Resume()
    {
        if (IsPaused)
        {
            Time.timeScale = 1.0f;
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (IsPaused)
        {         
            BattleManager.ActiveCharacter.soundStatus = SoundStatus.None;          

            if (Input.GetKeyDown(KeyCode.Escape))
                Resume();
        }
    }

    void Toggle()
    {
        if (IsPaused) {
            pauseText.text = "Play";
            Resume();
        } else {
            pauseText.text = "Pause";
            Pause();
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

	public void History()
	{
		battleStartMessage.gameObject.SetActive(true);
	}

	public void Options()
	{
		optionsMenu.gameObject.SetActive(true);
	}

    public void Instructions()
    {
        instructionPanel.gameObject.SetActive(true);
    }

    public void ExitToDesktop()
    {
		gm.Save();
        Application.Quit();
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1.0f;
		gm.Save();
		gm.ReloadCharactersLevels();
        Application.LoadLevel(0); // 0 index being the main menu
    }
}
