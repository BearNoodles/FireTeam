using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
	public static ItemManager instance = null;

	private GameManager gm;
	private PlayerProfile player;
	private List<ShopItem> hats, weapons, armour;

	public List<ShopItem> Hats
	{
		get { return hats; }
	}

	public List<ShopItem> Weapons
	{
		get { return weapons; }
	}

	public List<ShopItem> Armour
	{
		get { return armour; }
	}

	void Awake()
	{
		instance = this;
		gm = GameManager.instance;
		player = gm.Profile;

		//"Item Name", Attack, Defense, Cost
		hats = new List<ShopItem>();
		hats.Add(new ShopItem(0, "None", 0, 0, 0));
		hats.Add(new ShopItem(1, "Paper Bag", 0, 1, 10));
		hats.Add(new ShopItem(2, "Cowboy Hat", 0, 1, 50));
		hats.Add(new ShopItem(3, "Top Hat", 0, 1, 100));
		hats.Add(new ShopItem(4, "Crown", 10, 10, 500));

		//"Item Name", Attack, Defense, Cost
		weapons = new List<ShopItem>();
		weapons.Add(new ShopItem(0, "Pea Shooter", 0, 0, 0));
		weapons.Add(new ShopItem(1, "Paintball Gun", 3, 0, 30));
		weapons.Add(new ShopItem(2, "Air Rifle", 5, 0, 50));
		weapons.Add(new ShopItem(3, "7mm Pistol", 12, -1, 80));
		weapons.Add(new ShopItem(4, "A Salt Rifle", 25, 1, 110));

		//"Item Name", Attack, Defense, Cost
		armour = new List<ShopItem>();
		armour.Add(new ShopItem(0, "Band Tee", 0, 0, 0));
		armour.Add(new ShopItem(1, "Winter Jacket", -1, 3, 25));
		armour.Add(new ShopItem(2, "Daisy-chain Mail", -1, 6, 40));
		armour.Add(new ShopItem(3, "Nanosuit", 0, 15, 69));
		armour.Add(new ShopItem(4, "Dragon Armour", 1, 20, 100));
	}

	void Start()
	{
		try {
			for (int i = 0; i < hats.Count; i++)
			for (int j = 0; j < player.Hats.Length; j++)
			if (player.Hats[j].id == hats[i].id) {
				hats[i].Buy();
			}
		} catch {}

		try {
			for (int i = 0; i < weapons.Count; i++)
			for (int j = 0; j < player.Weapons.Length; j++)
			if (player.Weapons[j].id == weapons[i].id) {
				weapons[i].Buy();
			}
		} catch {}

		try {
			for (int i = 0; i < armour.Count; i++)
			for (int j = 0; j < player.Armour.Length; j++)
			if (player.Armour[j].id == armour[i].id) {
				armour[i].Buy();
			}
		} catch {}
	}
}
