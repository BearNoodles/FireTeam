using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    private BattleManager battleManager;
    private PlayerController playerController;
    private MouseCursor cursorManager;
    private Transform pauseMenu;
    private GameOver gameOver;

    private static ActiveIndicator activeIndicator; // TODO:- If this works, restructure reference sharing network?
    private static Text statusText;

    private bool displayHitChance = false;
    public bool DisplayHitChance
    {
        set
        {
            UpdateHitChance(value);
            displayHitChance = value;
        }

        get
        {
            return displayHitChance;
        }
    }

    public MouseCursor GetCursorManager { get { return cursorManager; } }
    public GameOver GetGameOver { get { return gameOver; } }

    public static Transform IndicatorTarget { get { return activeIndicator.ActiveTarget; } set { activeIndicator.SetTarget(value); } }
    public string StatusMsg { get { return statusText.text; } set { statusText.text = value; } } // Text to be displayed in top left corner

    // Use this for initialization
    void Awake () {
        // Get reference to BattleManager GameObjects Script
		GameObject battleManagerObj = GameObject.Find ("BattleManager");
		battleManager = battleManagerObj.GetComponent<BattleManager> ();

        // Get reference to active indicator
        Transform activeIndicatorObj = transform.Find("ActiveUnitIndicator");
        activeIndicator = activeIndicatorObj.GetComponent<ActiveIndicator>();

        statusText = (Text)transform.Find("StatusMessage").GetComponent<Text>();
        gameOver = transform.Find("GameOverPanel").GetComponent<GameOver>();
	}
	
    void Start()
    {
        // Get references
        playerController = BattleManager.GetPlayerController;
        cursorManager = BattleManager.GetMouseCursorMgr;
        pauseMenu = transform.Find("PauseMenu");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !battleManager.IsGameOver)
        {
            Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
        }

        if(BattleManager.CurrentBattleState == BattleState.GameOver)
        {
            gameOver.gameObject.SetActive(true);
        }
    }

    public void OnSkipButtonDown()
    {
        playerController.ChangeState(TurnState.ExitTurn);
        cursorManager.CurrentCursor = CursorsType.Default;
    }

    // Calculate the chance of hitting each enemy and set position to display relevant to targets
	private void UpdateHitChance(bool isActive)
	{
        List<Humanoid> humanoidList = battleManager.HumanoidsList;

        if (isActive)
        {
            Vector3 activeCharPos = BattleManager.ActiveCharacter.transform.position;

            if (humanoidList.Count == 0)
                return;

            foreach (Humanoid h in humanoidList)
            {
                // Calculate character distance from player
                float distance = Vector3.Distance(activeCharPos, h.transform.position);

                float hitChance = Mathf.Floor(Formula.CalculateHitChance(distance));

                Color hitcolor = HitChance.GetColour(hitChance);
                h.hitChanceText.color = hitcolor;

                // Calculate hit chance
                h.hitChanceText.text = hitChance.ToString() + "%";

                Transform hitChanceInner = h.hitChanceText.transform.parent;
                Transform hitChanceOuter = hitChanceInner.parent;

                // Don't show active player hit chance
                if (h == BattleManager.ActiveCharacter)
                {
                    hitChanceInner.gameObject.SetActive(false);
                    hitChanceOuter.gameObject.SetActive(false);
                    continue;
                }

                // Display HitChance
                hitChanceInner.gameObject.SetActive(true);
                hitChanceOuter.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach(Humanoid h in humanoidList)
            {
                Transform hitChanceInner = h.hitChanceText.transform.parent;
                Transform hitChanceOuter = hitChanceInner.parent;
                hitChanceInner.gameObject.SetActive(false);
                hitChanceOuter.gameObject.SetActive(false);
            }
        }
	}
}
