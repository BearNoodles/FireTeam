using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TeamBuilder : MonoBehaviour
{
	public GameObject btn, tutorial;
	public Text debug;
	private GameManager gm;
	private PlayerProfile player;
	private GameObject panelATK, panelDEF, panelHAT, panelTeam;
	private CharacterData currentChar;
	private CharacterData[] chars;

	void Awake()
	{
		panelHAT = gameObject.transform.Find("Hat").gameObject;
		panelATK = gameObject.transform.Find("Attack").gameObject;
		panelDEF = gameObject.transform.Find("Defense").gameObject;
		panelTeam = gameObject.transform.Find("TeamPanel").gameObject;

		try { Initialize(); }
		catch {}
	}

	void OnEnable()
	{
		try { Initialize(); }
		catch {}
	}

	void OnDisable()
	{
		Unload();
	}

	void Initialize()
	{
		gm = GameManager.instance;
		player = gm.Profile;
		player.UpdateMaxSlots();
		chars = player.Chars;
		currentChar = player.Current;
		GenerateSlots();
		ChangeSlot(GetSlot(0).charId);

		try { Destroy(transform.Find("TutorialPanel/Tutorial(Clone)").gameObject); }
		catch {}

		if (player.showTutorial) {
			GameObject tut = Instantiate(tutorial, Vector2.zero, Quaternion.identity) as GameObject;
			Toggle toggle = tut.transform.Find("Page4/Panel/Toggle").GetComponent<Toggle>();
			toggle.isOn = !player.showTutorial;
			toggle.onValueChanged.AddListener(delegate {
				player.ToggleTutorial();
			});

			tut.transform.SetParent(transform.Find("TutorialPanel"), false);
		}
	}

	void Update()
	{
		try {
			chars = player.Chars;
			currentChar = player.Current;
		} catch {}

		try { panelHAT.transform.Find("Text").GetComponent<Text>().text = player.Hats[currentChar.currentHat].Name; }
		catch { panelHAT.transform.Find("Text").GetComponent<Text>().text = "~"; }
		try { panelATK.transform.Find("Text").GetComponent<Text>().text = player.Weapons[currentChar.currentWeapon].Name; }
		catch { panelATK.transform.Find("Text").GetComponent<Text>().text = "~"; }
		try { panelDEF.transform.Find("Text").GetComponent<Text>().text = player.Armour[currentChar.currentArmour].Name; }
		catch { panelDEF.transform.Find("Text").GetComponent<Text>().text = "~"; }
		try { UpdateStatsPanel(); }
		catch {}
		
		for (int i = 0; i < chars.Length; i++)
		{
			chars[i].addedAttack = player.Weapons[chars[i].currentWeapon].GetStat(0);
			chars[i].addedHealth = player.Armour[chars[i].currentArmour].GetStat(1) + player.Hats[chars[i].currentHat].GetStat(1);
		}

		GetSlot(player.currentSlot).SetCurrent(true);
	}

	public void Unload()
	{
		for (int i = 0; i < panelTeam.transform.childCount; i++)
		{
			for (int j = 0; j < panelTeam.transform.Find("Slot" + i).transform.childCount; j++)
			{
				try { Destroy(panelTeam.transform.Find("Slot" + i).transform.GetChild(j).gameObject); }
				catch {}
			}
		}

		try { Destroy(transform.Find("TutorialPanel/Tutorial(Clone)").gameObject); }
		catch {}

		currentChar = null;
	}

	void GenerateSlots()
	{
		chars = player.Chars;

		for (int i = 0; i < panelTeam.transform.childCount; i++)
		{
			for (int j = 0; j < panelTeam.transform.Find("Slot" + i).transform.childCount; j++)
			{
				try { Destroy(panelTeam.transform.Find("Slot" + i).transform.GetChild(j).gameObject); }
				catch {}
			}

			GameObject button = (GameObject)Instantiate(btn, Vector2.zero, Quaternion.identity);
			TeamSlot slot = button.GetComponent<TeamSlot>();

			try {
				if (i < chars.Length) {
					slot.SetValues(i, SlotType.Active, chars[i]);
					button.GetComponent<Button>().onClick.AddListener(() => { OnActiveClicked(slot.id); });
				} else if (i < player.maxChars) {
					slot.SetValues(i, SlotType.Add);
					button.GetComponent<Button>().onClick.AddListener(() => { OnAddClicked(); });
				} else {
					slot.SetValues(i, SlotType.Locked);
				}
			} catch {}

			button.transform.SetParent(panelTeam.transform.Find("Slot" + i), false);
		}
	}

	TeamSlot GetSlot(int id) {
		return panelTeam.transform.Find("Slot" + id).transform.GetChild(0).GetComponent<TeamSlot>();
	}

	void OnActiveClicked(int id)
	{
		if (Input.GetKey(KeyCode.LeftShift))
			chars[id].SwitchClass();
		else if (Input.GetKey(KeyCode.LeftControl)
		         && id != player.currentSlot)
			DelChar(id);
		else
			ChangeSlot(GetSlot(id).charId);

		gm.Save();
	}

	void OnAddClicked() {
		List<CharacterData> team = new List<CharacterData>();
		try { for (int i = 0; i < chars.Length; i++) team.Add(chars[i]); } catch {}
		team.Add(gm.CreateCharData(50, 0));
		player.charData = team.ToArray();
		player.lastId++;
		GenerateSlots();
		ChangeSlot(player.lastId);
		gm.Save();
	}
	
	void DelChar(int id) {
		int currId = player.Current.id;
		List<CharacterData> team = new List<CharacterData>();
		for (int i = 0; i < chars.Length; i++) team.Add(chars[i]);
		team.RemoveAt(id);
		player.charData = team.ToArray();
		GenerateSlots();
		ChangeSlot(currId);
	}
	
	void ChangeSlot(int id)
	{
		for (int i = 0; i < panelTeam.transform.childCount; i++)
		{
			try { GetSlot(i).SetCurrent(false); } 
			catch {}
		}

		player.SetCurrent(id);
	}

	public void RandomizeCharName()
	{
		currentChar.GenerateName();

		switch(gm.IsDuplicateName(currentChar.name))
		{
			case true:
				return;
			case false:
				currentChar.GenerateName();
				break;
		}
	}

	void UpdateStatsPanel()
	{
		transform.Find("StatsPanel").transform.Find("Name").transform.Find("Text").GetComponent<Text>().text = currentChar.name;
		transform.Find("StatsPanel").transform.Find("Name").GetComponent<Button>().interactable = 
			((currentChar.originalName != "Steve") && 
			 (currentChar.originalName != "Daryl") && 
			 (currentChar.originalName != "Dillon"));

		transform.Find("StatsPanel").transform.Find("ATK").GetComponent<Text>().text = "ATK: " + (
            Formula.CalculateAttackPower(currentChar.level, currentChar.type) + GetAddedAttack()) + " (" +
            Formula.CalculateAttackPower(currentChar.level, currentChar.type) + GetAddedAttack().ToString("+#;-#;+0") + ")";
		transform.Find("StatsPanel").transform.Find("HLTH").GetComponent<Text>().text = "HLTH: " + (
			Formula.CalculateMaxHealth(currentChar.level, currentChar.type) + GetAddedDefense()) + " (" + 
			Formula.CalculateMaxHealth(currentChar.level, currentChar.type) + GetAddedDefense().ToString("+#;-#;+0") + ")";
		transform.Find("StatsPanel").transform.Find("Class").GetComponent<Text>().text = "Class: " +
			currentChar.type.ToString();
		transform.Find("StatsPanel").transform.Find("Skill").GetComponent<Text>().text = "Skill: " + 
			currentChar.abilityToUse.ToString();
	}

	int GetAddedAttack()
	{
		return (player.Hats[currentChar.currentHat].GetStat(0) +
		        player.Weapons[currentChar.currentWeapon].GetStat(0) +
		        player.Armour[currentChar.currentArmour].GetStat(0));
	}

	int GetAddedDefense()
	{
		return (player.Hats[currentChar.currentHat].GetStat(1) +
		        player.Weapons[currentChar.currentWeapon].GetStat(1) +
		        player.Armour[currentChar.currentArmour].GetStat(1));
	}

	public void CycleLeft(string item)
	{
		switch (item)
		{
			case "HAT":
				if (currentChar.currentHat > 0)
					currentChar.currentHat--;
				break;
			case "ATK":
				if (currentChar.currentWeapon > 0)
					currentChar.currentWeapon--;
				break;
			case "DEF":
				if (currentChar.currentArmour > 0)
					currentChar.currentArmour--;
				break;
		}

		currentChar = player.Current;
		gm.Save();
	}
    
	public void CycleRight(string item)
	{
		switch (item)
		{
			case "HAT":
				if (currentChar.currentHat < player.ownedHats.Length - 1)
					currentChar.currentHat++;
				break;
			case "ATK":
				if (currentChar.currentWeapon < player.ownedWeapons.Length - 1)
					currentChar.currentWeapon++;
				break;
			case "DEF":
				if (currentChar.currentArmour < player.ownedArmour.Length - 1)
					currentChar.currentArmour++;
				break;
		}

		currentChar = player.Current;
		gm.Save();
	}
}
