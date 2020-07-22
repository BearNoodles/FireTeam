using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
    
// Class to store player data
[Serializable]
public class PlayerProfile
{
	public bool[] mapsComplete;
	public bool showTutorial;
	public string name;

	public int id, lastId, currency,
		maxChars, score, currentSlot;

	public int rLevel, sLevel, mLevel;
	public float rExp, rExpRequired, sExp, 
	sExpRequired, mExp, mExpRequired;

	public ShopItem[] 
		ownedHats,
		ownedWeapons,
		ownedArmour;

	public void ResetMaps()
	{
		for(int i = 0; i < mapsComplete.Length; i++)
			mapsComplete[i] = false;
	}

	public void CompleteCurrentMap()
	{
		if (mapsComplete == null)
			mapsComplete = new bool[9];
        
		int levelNumber = Application.loadedLevel;
		mapsComplete[levelNumber - 1] = true;
	}

	public bool IsMapComplete(int levelNumber)
	{
		// Returns if level is complete or not (from 1 to 9)
		return mapsComplete[levelNumber - 1];
	}

	public int GetProgress()
	{
		int prog = 0;

		for (int i = 0; i < mapsComplete.Length; i++)
		{
			int level = i + 1;
			if (IsMapComplete(level)) {
				prog = ((level / 9) * 100);
			}
		}

		return prog;
	}

	public void UpdateMaxSlots()
	{
		maxChars = (IsMapComplete(6)) ? 5 : (IsMapComplete(3)) ? 4 : 3;
	}

	public void UpdateLevels()
	{
        rExpRequired = 50;
        sExpRequired = 50;
        mExpRequired = 50;

		while (rExp > rExpRequired || sExp > sExpRequired || mExp > mExpRequired) {

            rExpRequired = 50;
            sExpRequired = 50;
            mExpRequired = 50;

			for (int i = 0; i < rLevel + 1; i++)
				rExpRequired *= 1.1f;
			for (int i = 0; i < sLevel + 1; i++)
				sExpRequired *= 1.1f;
			for (int i = 0; i < mLevel + 1; i++)
				mExpRequired *= 1.1f;

			if (rExp > rExpRequired) {
				rExp -= rExpRequired;
				rLevel++;
			}

			if (sExp > sExpRequired) {
				sExp -= sExpRequired;
				sLevel++;
			}

			if (mExp > mExpRequired) {
				mExp -=mExpRequired;
				mLevel++;
			}

		}
		Debug.Log("rifle is " + rLevel + " support is " + sLevel + " medic is " + mLevel);
	}

	public void AddCurrency(int amt)
	{
		currency += amt;
	}

	public void SetCurrent(int id)
	{
		for (int i = 0; i < charData.Length; i++)
		{
			if (charData[i].id == id) currentSlot = i;
		}
	}

	public void ToggleTutorial()
	{
		showTutorial = !showTutorial;
	}

	public CharacterData[] charData;

	public PlayerProfile(int i, string n)
	{
		id = i;
		lastId = 0;
		name = n;
		currency = 15;
		score = 0;
		mapsComplete = new bool[9];
		showTutorial = true;

		UpdateMaxSlots();
	}

	public ShopItem[] Hats { get { return ownedHats; } }
	public ShopItem[] Weapons { get { return ownedWeapons; } }
	public ShopItem[] Armour { get { return ownedArmour; } }
	public CharacterData Current { get { return charData[currentSlot]; } }
	public CharacterData[] Chars { get { return charData; } }
}
