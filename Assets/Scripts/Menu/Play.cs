using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Play : MonoBehaviour
{
	private GameManager gm;
	private GameObject buttons, info;

	void Awake()
	{
		gm = GameManager.instance;
		buttons = transform.Find("Map").transform.Find("Buttons").gameObject;
		info = transform.Find("Map").transform.Find("LevelInfo").gameObject;
		
		for (int i = 1; i < buttons.transform.childCount + 1; i++)
		{
			int id = i;
			buttons.transform.Find("Level" + i).GetComponent<Button>()
				.onClick.AddListener(() => { ShowInfo(id); });
		}

		for (int i = 1; i < info.transform.childCount; i++)
		{
			int id = i;
			info.transform.GetChild(i).transform.Find("Play").GetComponent<Button>()
				.onClick.AddListener(() => { PlayGame(id); });
		}
	}

	void OnEnable()
	{
		Clear();
	}

	void Update()
	{
		for (int i = 1; i < info.transform.childCount; i++)
		{
			Color col1 = Color.white;
			if (gm.Profile.IsMapComplete(i)) col1 = Color.yellow;
			else col1 = Color.white;
			buttons.transform.Find("Level" + i).GetComponent<Image>().color = col1;
			
			Color col2 = Color.black;
			if (info.transform.Find("Info" + i).gameObject.activeSelf) col2 = Color.red;
			else col2 = Color.black;
			buttons.transform.Find("Level" + i).transform.GetChild(0).GetComponent<Image>().color = col2;
			
			try { buttons.transform.Find("Level" + i).GetComponent<Button>().interactable = gm.Profile.IsMapComplete(i - 1); }
			catch {}
		}
	}

	void ShowInfo(int l)
	{
		for (int i = 0; i < info.transform.childCount; i++)
			info.transform.GetChild(i).gameObject.SetActive(false);

		try { info.transform.Find("Info" + l).gameObject.SetActive(true); }
		catch {}
	}

	void PlayGame(int i)
	{
		try { Application.LoadLevel(i); }
		catch { Debug.LogError("Failed to load level " + i); }
	}

	public void Clear()
	{
		for (int i = 0; i < info.transform.childCount; i++)
			info.transform.GetChild(i).gameObject.SetActive(false);

		try { info.transform.Find("InfoDefault").gameObject.SetActive(true); }
		catch {}
	}
}
