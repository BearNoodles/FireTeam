using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class New : MonoBehaviour
{
	public GameObject btnLoad, btnNew;

	private GameManager gm;
	private GameObject panel;
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

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Refresh();
	}

	public void Refresh()
	{
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

				if (saves[i] == null) {
					GameObject button = (GameObject)Instantiate(btnNew, Vector2.zero, Quaternion.identity);
					button.GetComponent<NewButton>().SetValues(id);
					button.GetComponent<Button>().onClick.AddListener(() => { ActivateButton(id, button); });
					button.transform.SetParent(panel.transform.Find("Save" + id), false);
				} else {
					GameObject button = (GameObject)Instantiate(btnLoad, Vector2.zero, Quaternion.identity);
					button.GetComponent<LoadButton>().SetValues(saves[i]);
					button.GetComponent<Button>().interactable = false;
					button.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(() => { Delete(id); });
					button.transform.SetParent(panel.transform.Find("Save" + id), false);
				}
			}
		} catch {}
	}

	void Delete(int id)
	{
		gm.Delete(id);
		Refresh();
	}

	void ActivateButton(int id, GameObject button)
	{
		for (int i = 0; i < 4; i++)
		{
			try {
				Button btn = panel.transform.Find("Save" + i).GetChild(0).gameObject.GetComponent<Button>();
				btn.interactable = false;
			} catch {}
		}
		
		button.GetComponent<Button>().onClick.RemoveAllListeners();
		button.GetComponent<Button>().enabled = false;
		button.GetComponent<NewButton>().SetActive(true);
	}
}
