using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

// Script by Stephen Hayward

public enum TurnState
{
	None,
	AwaitingInput,
	Moving,
	Attacking,
	UsingAbility,
	ExitTurn
}

public enum BattleState
{
    Paused,
    Playing,
    GameOver
}

public class Stats
{
	public int goldCollected;
	public int turnsTaken; 
	public int expGained;
	public int shotsFired;
	public int hits;
	public int misses;
	public int damageDealt;
	public int healing;
	public int accuracy;
	public Text text;


	public void LoadStats()
	{
		text = GameObject.Find("GameOverPanel").transform.Find("Text").GetComponent<Text>();
		Debug.Log ("Text" + text);
	}

	public void UpdateStats()
	{
		if (shotsFired <= 0)
			accuracy = 0;
		else
			accuracy = hits * 100 / shotsFired;
		Debug.Log (shotsFired + "  " + hits + "  " + misses + "      acc   " + accuracy);
		text.text = "stats\r\nGold collected " + goldCollected + "\r\nTurns taken "+  turnsTaken + "\r\nExp gained "+ expGained + "\r\nDamage dealt " + damageDealt + "\r\nHealing " + healing + "\r\nShots fired " + shotsFired + "\r\nAccuracy " + accuracy + "%";
		text.alignment = TextAnchor.UpperLeft;
	}
}


public class BattleManager : MonoBehaviour
{
    // This class will handle the most things battle related (Turn-base system, references to player and enemy gameobjects, etc)
    public static bool IsTurnOver { get; set; } // WARNING: Setting this to yes will cause a new turn to be created
    public static BattleState CurrentBattleState { get; set; }

    private static GameManager gameManager;
    private static MouseCursor gameCursorManager;
    private static ActiveIndicator activeIndicator;
    private static PlayerController playerController;
	private static EnemyController enemyController;
    private static HUD hud;
    private static FloatingTextManager floatingTextManager;
    private static BattleManager instance;
	private static SoundEffectManager sound;
	private Stats stats;
	private PlayerProfile profile;
    private List<Humanoid> humanoids; // References to all game characters
    private List<Humanoid> players;
    private List<Humanoid> enemies;
    private static Humanoid activeHuman;

	private GameObject[] panels;

    public TurnState SetPlayerTurnState { set { playerController.ChangeState(value); }} // Expose for use in ActionBar Buttons
   
    private Humanoid SetActivePlayer { set { activeHuman = value; } }

	public int level;

	public bool expGiven = false;
    public bool IsGameOver { get { return (players.Count == 0 || enemies.Count == 0); } }
    public bool IsPlayersTurn { get { return (ActiveCharacter.gameObject.tag == "Player") ? true : false; } }
    public bool IsPlayerWinner { get { return enemies.Count == 0; } }
    public bool IsEnemyWinner { get { return players.Count == 0; } }
    public bool IsTie { get { return (enemies.Count == 0 && players.Count == 0); } }
	public List<Humanoid> HumanoidsList { get { return humanoids; } }
    public List<Humanoid> Players { get { return players; } }
    public List<Humanoid> Enemies { get { return enemies; } }
    public static Humanoid ActiveCharacter { get { return activeHuman; } }

    public static PlayerController GetPlayerController { get { return playerController; } }
    public static EnemyController GetEnemyController { get { return enemyController; } }
    public static MouseCursor GetMouseCursorMgr { get { return gameCursorManager; } }
    public static FloatingTextManager GetFloatingTextManager { get { return floatingTextManager; } }
    public static HUD GetHUD { get { return hud; } }
    public static BattleManager Instance { get { return instance; } }
    public static SoundEffectManager GetSoundManager { get { return sound; } }
    private bool finalGameSave;

    void Awake()
	{
        instance = this;


        humanoids = new List<Humanoid>();
        players = new List<Humanoid>();
        enemies = new List<Humanoid>();

        GameObject gameManagerObj = GameObject.Find("GameManager");
        hud = GameObject.Find("HUD").GetComponent<HUD>();

        // Get script references from seperate GameObjects
        gameManager = gameManagerObj.GetComponent<GameManager>();
        gameCursorManager = GetComponent<MouseCursor>();

        // Get script references from this GameObject
        playerController = GetComponent<PlayerController>();
        enemyController = GetComponent<EnemyController>();
        floatingTextManager = GetComponent<FloatingTextManager>();
        // Set variables
        playerController.SetFloatingTextMgr(floatingTextManager);
        enemyController.SetFloatingTextMgr(floatingTextManager);
    }

    void Start()
	{
		level = Application.loadedLevel;

		profile = gameManager.Profile;
		stats = playerController.stats;
		stats.LoadStats ();

        sound = GetComponent<SoundEffectManager>();
        Debug.Log("Sound component: " + sound);

        GetHumanoids();

        IsTurnOver = true; // Activate a new turn

        activeHuman = humanoids[0];

        // Order Panels by name
		panels = GameObject.FindGameObjectsWithTag("TurnSpace");
		Array.Sort (panels, delegate(GameObject panel1, GameObject panel2) { 
			return panel1.name.CompareTo(panel2.name); 
		});

        // Assign the lists in enemyController
        enemyController.TeamPlayers = players;
        enemyController.TeamAi = enemies;

		TurnOrder();

        CurrentBattleState = BattleState.Playing;
    }

	void GiveExp (float outCome)
	{
		if (expGiven == false) {
			Debug.Log (level + "islevel");
			profile.rExp += outCome * 1000 * Mathf.Pow (2, level - 1);
			profile.sExp += outCome * 1000 * Mathf.Pow (2, level - 1);
			profile.mExp += outCome * 1000 * Mathf.Pow (2, level - 1);
			int expStat = (int)(profile.rExp + profile.sExp + profile.mExp);
			stats.expGained = expStat;
			expGiven = true;
			Debug.Log (outCome * 1000 * Mathf.Pow (2, level - 1) + "  is exp");
		}
	}

    // Update is called once per frame
    void Update()
    {
		profile.UpdateLevels ();
		stats.UpdateStats ();
		sound.UpdateSound ();
        RemoveDead();

        Debug.Log("Active Character: " + activeHuman);
        Debug.Log("HERE NOW");
        Debug.Log("Player Count: " + players.Count);
        if (IsGameOver)
        {
            CurrentBattleState = BattleState.GameOver;

            if(IsTie)
            {
				GiveExp(0.5f);
                hud.GetGameOver.UpdateState(GameOver.GameOverState.Tie);
                Debug.Log("Game was a tie");
            }
            else if(IsPlayerWinner)
            {
				profile.CompleteCurrentMap();
				GiveExp(1f);
                hud.GetGameOver.UpdateState(GameOver.GameOverState.Win);
                Debug.Log("Player Won");
            }
            else if(IsEnemyWinner)
            {
				GiveExp(0.2f);
                hud.GetGameOver.UpdateState(GameOver.GameOverState.Lose);
                Debug.Log("Enemy Won"); 
            }

            if (!finalGameSave)
            {
                gameManager.Save();
                gameManager.ReloadCharactersLevels();
                finalGameSave = true;
            }
            Debug.Log("GameOver"); // End game or something~ ask if wants to replay if lost or display results and asks if wants to exit
            return;
        }

        Debug.Log("Current Battle State: " + CurrentBattleState);
        if (CurrentBattleState == BattleState.Playing && !PauseMenu.IsPaused)
        {
            // End turn if active character is dead to avoid game breaking bug.
            if (ActiveCharacter == null || ActiveCharacter.IsDead)
                IsTurnOver = true;

            //Debug.Log("Active Human: " + activeHuman.name);

            // If turn is over then update turn queue and create a new turn for character at the front
            if (IsTurnOver)
            {
                ActiveCharacter.Recovery += Humanoid.SPEED;
                TurnOrder();

                CreateNewTurn(humanoids[0]);
            }

            // Update active players turn
            if (IsPlayersTurn)
                playerController.CallUpdate();
            else
                enemyController.CallUpdate();


            Debug.DrawLine(ActiveCharacter.Position, new Vector3(ActiveCharacter.Position.x, ActiveCharacter.Position.y-15, 0), Color.red, 2, false);
        }
    }

    void RemoveDead()
    {
        List<Humanoid> bin = new List<Humanoid>();

        // Locate dead humans
        foreach(Humanoid h in humanoids)
        {
            if (h.IsDead)
            {
                bin.Add(h);
                Debug.Log("Destroy Me Now Called");
            }
        }

        bool anyItems = (bin.Count > 0);

        // Remove dead from lists and tell them they are dead
        foreach(Humanoid h in bin)
        {
            humanoids.Remove(h);
            players.Remove(h);
            enemies.Remove(h);

            h.SendMessage("KillMe");
        }

        // If anything was deleted then update references
        if (anyItems)
        {
            enemyController.TeamPlayers = players;
            enemyController.TeamAi = enemies;
        }

        // Probably redundant, but clear the bin
        bin.Clear();
    }

    // Get references to all gameobjects under the "Player" and "Enemy" tags
    public void GetHumanoids()
    {
        // Find all gameobjects with tag 'Character'
        GameObject[] getPlayers = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] getEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Get enemy humanoid script references
        for (int i = 0; i < getEnemies.Length; i++)
        {
            enemies.Add(getEnemies[i].GetComponent<Enemy>());
            humanoids.Add(enemies[i]);
        }

        // Get players humanoid script references
        for (int i = 0; i < getPlayers.Length; i++)
        {
            players.Add(getPlayers[i].GetComponent<Player>());
            humanoids.Add(players[i]);
        }
    }

    private void CreateNewTurn(Humanoid currentlyActive)
    {
		stats.turnsTaken++;
        activeHuman = currentlyActive;
        ActiveCharacter.ResetMovement();
        HUD.IndicatorTarget = currentlyActive.transform;

        // Create new turn based on who's turn it is
        if(IsPlayersTurn)
        {
            playerController.NewTurn(humanoids[0]);
            playerController.CurrentState = State.Enabled;
            enemyController.CurrentState = State.Disabled;
            Debug.Log("New player turn created.");
        }
        else
        {
            enemyController.NewTurn(humanoids[0]);
            playerController.CurrentState = State.Disabled;
            enemyController.CurrentState = State.Enabled;
            Debug.Log("New enemy turn created.");
        }

        IsTurnOver = false;
    }

    // Update queue for characters turns [TODO:- Soz broke this by chaning arrays to lists
    private void TurnOrder()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].turn = i * 2;
        }

        for (int i = 0; i < enemies.Count; i++) 
		{
			enemies [i].turn = (i * 2) + 1;
		}

        humanoids.Sort((x, y) => (x.Recovery != y.Recovery) ? x.Recovery.CompareTo(y.Recovery) : x.turn.CompareTo(y.turn));       
		
        // Keep Values Low
        while (humanoids[0].Recovery > 0)
        {
            // Note:- Careful of infinite bugs here ~ Skip this player if they are dead ~ caused one

            for(int i = 0; i < humanoids.Count; i++)
            {
                humanoids[i].Recovery--;
            }
        }

		for(int i = 0; i < humanoids.Count; i++)
		{
			if (humanoids[i].Recovery < 0)
				humanoids[i].Recovery = 0;
		}

        for (int i = 0; i < humanoids.Count; i++)
        {
            UnityEngine.UI.Image image = panels[i].GetComponent<UnityEngine.UI.Image>();
            TurnPanel turnPanel = panels[i].GetComponent<TurnPanel>();

            turnPanel.SetPanel(humanoids[i].CharData.name, (humanoids[i].tag == "Player"));
            image.sprite = humanoids[i].portrait;
        }

        // Disable unused panels
        for(int i = 0; i < panels.Length - humanoids.Count; i++)
        {
            UnityEngine.UI.Image image = panels[i].GetComponent<UnityEngine.UI.Image>();
            TurnPanel turnPanel = panels[i].GetComponent<TurnPanel>();

            panels[humanoids.Count + i].SetActive(false);    
        }
    }
}
