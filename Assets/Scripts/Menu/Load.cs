using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Load : MonoBehaviour
{
	public GameObject btn;

	private GameManager gm;
	private GameObject panel;
	private Animator anim;
	private PlayerProfile[] saves;

	// Use this for initialization
	void Awake()
	{
		gm = GameManager.instance;
		panel = transform.Find("Saves").gameObject;
		Refresh();
	}

	void OnEnable()
	{
		Refresh();
	}

	void OnLevelWasLoaded(int level)
	{
		if (level == 0) Refresh();
	}

	public void Refresh()
	{
		anim = GameObject.FindGameObjectWithTag("TableInner").GetComponent<Animator>();

		for (int i = 0; i < 4; i++)
		{
			try {
				GameObject obj = panel.transform.Find("Save" + i).transform.GetChild(0).gameObject;
				if (obj != null) Destroy(obj);
			} catch {}
		}

		try {
			saves = gm.LoadSaves();
			GenerateButtons();
		} catch {}
	}

	void GenerateButtons()
	{
		try {
			for (int i = 0; i < saves.Length; i++)
			{
				int id = i;

				if (saves[i] != null) {
					try {
						GameObject button = (GameObject)Instantiate(btn, Vector2.zero, Quaternion.identity);
						button.GetComponent<LoadButton>().SetValues(saves[i]);
						button.GetComponent<Button>().onClick.AddListener(() => { LoadProfile(id); });
						button.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(() => { Delete(id); });
						button.transform.SetParent(panel.transform.Find("Save" + id), false);
					} catch {}
				}
			}
		} catch {}
	}

	void Delete(int id)
	{
		gm.Delete(id);
		Refresh();
	}

	void LoadProfile(int id)
	{
		try { Destroy(gm.GetComponent<ItemManager>()); }
		catch { Debug.Log("@Load(): Couldn't destroy old ItemManager"); }

		gm.Load(id);
		gm.GetComponent<Init>().UnloadPanels();
		gm.gameObject.AddComponent<ItemManager>();
		gm.GetComponent<Init>().LoadSecondaryPanels();
		anim.Play("TableIdleLoaded");
	}

	public void Restart()
	{
		gm.Save();
		gm.GetComponent<Init>().UnloadPanels();
		gm.UnloadProfile();

		try { Destroy(gm.GetComponent<ItemManager>()); }
		catch { Debug.Log("@Restart(): Couldn't destroy old ItemManager"); }

		gm.GetComponent<Init>().LoadPrimaryPanels();
		anim.Play("TableIdleUnloaded");
	}
}
