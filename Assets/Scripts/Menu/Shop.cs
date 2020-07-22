using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
	//Class variables
	public GameObject btnEnabled, btnDisabled;
	public Text balance;

	private GameManager gm;
	private ItemManager im;
	private PlayerProfile player;

	void Start()
	{
		Initialize();
	}

	void OnEnable()
	{
		Initialize();
	}

	void Update()
	{
		balance.text = "Credits: " + player.currency;
	}

	void Initialize() {
		gm = GameManager.instance;
		im = ItemManager.instance;
		player = gm.Profile;
		GenerateButtons();
	}

	public void FirstRun()
	{
		Buy(im.Weapons[0], 0);
		Buy(im.Armour[0], 0);
		Buy(im.Hats[0], 0);

		GenerateItems();
		GenerateButtons();
	}

	void GenerateButtons()
	{
		GameObject[] panels = new GameObject[3];
		panels[0] = gameObject.transform.Find("PanelHAT/Scroll/Content").gameObject;
		panels[1] = gameObject.transform.Find("PanelATK/Scroll/Content").gameObject;
		panels[2] = gameObject.transform.Find("PanelDEF/Scroll/Content").gameObject;
		
		for (int i = 0; i < panels.Length; i++)
		{
			for (int j = 0; j < panels[i].transform.childCount; j++)
			{
				Transform child = panels[i].transform.GetChild(j);
				if (child.name != "Title") Destroy(child.gameObject);
			}
		}

		#region Generate Buttons
		int x = -160;
		for (int i = 0; i < im.Hats.Count; i++)
		{
			ShopItem item = im.Hats[i];
			bool canBuy = ((player.currency >= item.GetStat(2)) && !(item.Owned));
			GameObject btn = LoadButtonPrefab(canBuy, new Vector2(-40, x - (i * 70)));
			btn.GetComponent<ShopButton>().SetValues(item);
			btn.GetComponent<Button>().onClick.AddListener(() => Buy(item, item.GetStat(2)));
			btn.transform.SetParent(panels[0].transform, false);
		}

		x = -160;
		for (int i = 0; i < im.Weapons.Count; i++)
		{
			ShopItem item = im.Weapons[i];
			bool canBuy = ((player.currency >= item.GetStat(2)) && !(item.Owned));
			GameObject btn = LoadButtonPrefab(canBuy, new Vector2(-40, x - (i * 70)));
			btn.GetComponent<ShopButton>().SetValues(item);
			btn.GetComponent<Button>().onClick.AddListener(() => Buy(item, item.GetStat(2)));
			btn.transform.SetParent(panels[1].transform, false);
		}

		x = -160;
		for (int i = 0; i < im.Armour.Count; i++)
		{
			ShopItem item = im.Armour[i];
			bool canBuy = ((player.currency >= item.GetStat(2)) && !(item.Owned));
			GameObject btn = LoadButtonPrefab(canBuy, new Vector2(-40, x - (i * 70)));
			btn.GetComponent<ShopButton>().SetValues(item);
			btn.GetComponent<Button>().onClick.AddListener(() => Buy(item, item.GetStat(2)));
			btn.transform.SetParent(panels[2].transform, false);
		}
		#endregion
	}

	void GenerateItems()
	{
		List<ShopItem> hats = new List<ShopItem>();
		for (int i = 0; i < im.Hats.Count; i++)
			if (im.Hats[i].Owned) hats.Add(im.Hats[i]);
		player.ownedHats = hats.ToArray();

		List<ShopItem> weapons = new List<ShopItem>();
		for (int i = 0; i < im.Weapons.Count; i++)
			if (im.Weapons[i].Owned) weapons.Add(im.Weapons[i]);
		player.ownedWeapons = weapons.ToArray();
		
		List<ShopItem> armour = new List<ShopItem>();
		for (int i = 0; i < im.Armour.Count; i++)
			if (im.Armour[i].Owned) armour.Add(im.Armour[i]);
		player.ownedArmour = armour.ToArray();
	}

	void Buy(ShopItem item, int cost)
	{
		if (!item.Owned)
		{
			item.Buy();
			player.AddCurrency(-cost);
		}

		GenerateItems();
		GenerateButtons();
		gm.Save();
	}

	public GameObject LoadButtonPrefab(bool enabled, Vector2 position)
	{
		switch(enabled)
		{
			case false:
				return (GameObject)Instantiate(btnDisabled, position, Quaternion.identity);
			case true:
				return (GameObject)Instantiate(btnEnabled, position, Quaternion.identity);
		}

		return null;
	}
}
