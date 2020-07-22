using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[Serializable]
public class ShopItem
{
	public int id;

	private string itemName;
	private int itemATK, itemDEF, itemCost;
	private bool itemOwned;

	public string Name
	{
		get { return itemName; }
	}

	public bool Owned
	{
		get { return itemOwned; }
	}

	public ShopItem(int i, string name, int atk, int def, int cost)
	{
		id = i;
		itemName = name;
		itemATK = atk;
		itemDEF = def;
		itemCost = cost;
	}

	public void Buy()
	{
		itemOwned = true;
	}

	public int GetStat(int i)
	{
		switch (i)
		{
			case 0:
				return itemATK;
			case 1:
				return itemDEF;
			case 2:
				return itemCost;
			default:
				return 0;
		}
	}
}
