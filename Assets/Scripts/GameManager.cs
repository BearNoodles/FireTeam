//#define SkipToScene
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public Humanoid rifleManPref, medicPref, supportPref;

	private Animator anim;
    private PlayerProfile playerProfile;
    private Player[] playerChars; // References to player characters

	public PlayerProfile Profile
	{
		get { return playerProfile; }
	}

	// Use this for initialization
	void Awake()
	{
		// Apply singleton logic so that there is only one instance of GameManager
		if (instance == null) instance = this;
		else Destroy(gameObject);

		// Persist object through scenes
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start()
	{
		#if SkipToScene
		// NOTE FROM STEVE:- For scene
		CreateProfile(1, "Santa");
		Application.LoadLevel(3);
		#endif

		Initialize();
	}

	void Initialize()
	{
		Init init = GetComponent<Init>();
		anim = GameObject.FindGameObjectWithTag("TableInner").GetComponent<Animator>();

		if (playerProfile != null) {
			init.UnloadPanels();
			init.LoadSecondaryPanels();
			anim.Play("TableIdleLoaded");
		}
	}

	void OnLevelWasLoaded(int level) {
		if (level >= 1) {
			try {
				GenerateCharsFromProfile(playerProfile);
			} catch(Exception e) {
				Debug.Log("Error: " + e.Message);
			}
		} else {
			Initialize();
		}
	}

	public void CreateProfile(int id, string name)
	{
		string path = Application.persistentDataPath + "/PlayerProfile" + id + ".dat";
		BinaryFormatter binaryFormatter = new BinaryFormatter();

		// Create or override existing file
		FileStream file = File.Create(path);

		// Apply all the data to be saved to a new player profile
		playerProfile = new PlayerProfile(id, name);
		playerProfile.charData = CreateDefaultTeam();

		// Write data to file
		binaryFormatter.Serialize(file, playerProfile);

		file.Close();
	}

	public void UnloadProfile() {
		GameObject.Find("Canvas").transform.Find("Team").GetComponent<TeamBuilder>().Unload();
		playerProfile = null;
	}

	public void Save()
	{
		string path = Application.persistentDataPath + "/PlayerProfile" + playerProfile.id + ".dat";
		BinaryFormatter binaryFormatter = new BinaryFormatter();
        
		// Create or override existing file
		FileStream file = File.Create(path);

		// Write data to file
		binaryFormatter.Serialize(file, playerProfile);

		file.Close();
	}

	public void Delete(int id)
	{
		string path = Application.persistentDataPath + "/PlayerProfile" + id + ".dat";
		
		if(File.Exists(path)) {
			// Delete File
			File.Delete(path);
		} else {
			Debug.Log("Could not delete " + path);
		}
	}

	public void Load(int id)
	{
		string path = Application.persistentDataPath + "/PlayerProfile" + id + ".dat";

		if(File.Exists(path)) {
			BinaryFormatter binaryFormatter = new BinaryFormatter();

			// Open File
			FileStream file = File.Open(path, FileMode.Open);

			// Grab information from playerProfile
			playerProfile = (PlayerProfile)binaryFormatter.Deserialize(file);
			file.Close();
		} else {
			Debug.Log("Could not load " + path);
		}
	}

	public PlayerProfile[] LoadSaves()
	{
		PlayerProfile[] temp = new PlayerProfile[4];

		for (int i = 0; i < temp.Length; i++) {
			string path = Application.persistentDataPath + "/PlayerProfile" + i + ".dat";

			try {
				if(File.Exists(path)) {
		            BinaryFormatter binaryFormatter = new BinaryFormatter();

		            // Open File
					FileStream file = File.Open(path, FileMode.Open);

		            // Grab information from playerProfile
		            temp[i] = (PlayerProfile)binaryFormatter.Deserialize(file);
		            file.Close();
		        }
			} catch {
				Debug.Log("No save file found in slot " + i);
			}
		}

		return temp;
    }

    // Load player characters from their prefabs referenced in the inspector
    public Player LoadCharPrefab(UnitType unittype, Vector2 position)
    {
        switch(unittype)
        {
            case UnitType.Rifleman:
                return (Player)Instantiate(rifleManPref, position, Quaternion.identity);
            case UnitType.Medic:
                return (Player)Instantiate(medicPref, position, Quaternion.identity);
            case UnitType.Support:
                return (Player)Instantiate(supportPref, position, Quaternion.identity);
        }

        return null;
    }

    public void ReloadCharactersLevels()
    {
        foreach (CharacterData cd in playerProfile.charData)
        {
            if (cd.type == UnitType.Rifleman)
                cd.level = playerProfile.rLevel;
            else if (cd.type == UnitType.Support)
                cd.level = playerProfile.sLevel;
            else if (cd.type == UnitType.Medic)
                cd.level = playerProfile.mLevel;
        }
    }

    public void GenerateCharsFromProfile(PlayerProfile profile)
    {
        int noOfPlayers = playerProfile.charData.Length;
        Transform parentObj = GameObject.Find("PlayerFireTeam").transform;
        SpawnManager spawnManager = GameObject.Find("SpawnPoints").GetComponent<SpawnManager>();

        Debug.Log("Number of generate chars from profile iterations: " + playerProfile.charData.Length);

        // Reset Health (To fix bug with trying to replay game and health still being 0)
        foreach (CharacterData cd in playerProfile.charData)
        {
            cd.health = 10000;
        }

        ReloadCharactersLevels();

        // Load all characters from prefabs and apply loaded settings
        for (int i = 0; i < noOfPlayers; i++)
        {
            Player temp = LoadCharPrefab(playerProfile.charData[i].type, Vector2.zero);

            if (temp == null)
                Debug.Log("Character No." + i + "'s prefab was not loaded.");

            temp.CharData = playerProfile.charData[i];
            temp.name = playerProfile.charData[i].name;

            temp.transform.SetParent(parentObj);

            spawnManager.SetSpawnPoint(temp.transform);
            Debug.Log("Character Loaded: " + temp.name);
        }
    }

    // Get references to all the player scripts in scene. Note: This is quite slow, so do not call too often
    public Player[] GetPlayersReferences()
    {
        GameObject[] refs = GameObject.FindGameObjectsWithTag("Player");

        Player[] playerScripts = new Player[refs.Length];

        for (int i = 0; i < refs.Length; i++)
            playerScripts[i] = refs[i].GetComponent<Humanoid>() as Player;

        return playerScripts;
    }

	public bool IsDuplicateName(string name)
	{
		bool val = false;
		for (int i = 0; i < playerProfile.charData.Length; i++)
			if (name == playerProfile.charData[i].name) val = true;

		return val;
	}

	private CharacterData[] CreateDefaultTeam()
	{
		List<CharacterData> team = new List<CharacterData>();
		team.Add(CreateStartCharData(0, "Steve", UnitType.Rifleman, 50, 0));
		team.Add(CreateStartCharData(1, "Daryl", UnitType.Medic, 50, 0));

#if SkipToScene
		playerProfile.maxChars = 5;
		team.Add(CreateStartCharData(2, "Dillon", UnitType.Rifleman, 50, 0));
		team.Add(CreateStartCharData(3, "Steve", 50, 0));
		team.Add(CreateStartCharData(4, "Steve", 50, 0));
#endif

		return team.ToArray();
	}

	public CharacterData CreateStartCharData(int id, string name, UnitType type, int attackPower, int healthPower)
	{
		CharacterData character = new CharacterData();
		character.Initialize(type);
		character.id = id;
		character.name = name;
		character.attackPower = attackPower;
		character.healPower = healthPower;
		character.experience = 0;
		character.level = 1;
		character.health = 10000; // Will be lowered by MaxHealth Check
		character.originalName = character.name;
		playerProfile.lastId = id;

		return character;
	}

	public CharacterData CreateCharData(int attackPower, int healthPower)
	{
        CharacterData character = new CharacterData();
		character.Initialize();
		character.id = (playerProfile.lastId + 1);
        character.attackPower = attackPower;
        character.healPower = healthPower;
        character.experience = 0;
		character.level = 1;
		character.attackPower = Formula.CalculateAttackPower(character.level, character.type);
        character.health = 10000; // Will be lowered by MaxHealth Check

		try {
			while (IsDuplicateName(character.name)) character.GenerateName();
		} catch {}

		try {
			character.currentHat = UnityEngine.Random.Range(0, playerProfile.Hats.Length);
			character.currentWeapon = UnityEngine.Random.Range(0, playerProfile.Weapons.Length);
			character.currentArmour = UnityEngine.Random.Range(0, playerProfile.Armour.Length);
		} catch {}

		character.originalName = character.name;
		return character;
	}
}
