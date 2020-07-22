using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Data : MonoBehaviour
{
	public GameObject btn;

	private GameManager gm;

	// Use this for initialization
	void Awake ()
	{
		gm = GameManager.instance;
		GenerateButtons();
	}

	void OnEnable()
	{
		GenerateButtons();
	}

	void GenerateButtons()
	{
		GameObject panel = transform.Find("Buttons/Scroll/Content/Container").gameObject;

		for (int i = 0; i < panel.transform.childCount; i++)
		{
			try {
				Transform child = panel.transform.GetChild(i).Find("InfoButton(Clone)");
				Destroy(child.gameObject);
			} catch {}
		}
		
		#region Generate Buttons
		for (int i = 0; i < panel.transform.childCount; i++)
		{
			int id = i;
			try {
				if (gm.Profile.IsMapComplete(i + 1)) {
					GameObject button = (GameObject)Instantiate(btn, Vector2.zero, Quaternion.identity);
					button.GetComponent<InfoButton>().SetValues(i + 1);
					button.GetComponent<Button>().onClick.AddListener(() => ShowInfo(id));
					button.transform.SetParent(panel.transform.GetChild(i).transform, false);
				}
			} catch {}
		}
		#endregion
	}

	void ShowInfo(int id)
	{
		GameObject panel = transform.Find("Info/Scroll/Content/Container").gameObject;

		for (int i = 0; i < panel.transform.childCount; i++)
			panel.transform.GetChild(i).gameObject.SetActive(false);

		transform.Find("Buttons").gameObject.SetActive(false);
		transform.Find("Info").gameObject.SetActive(true);
		panel.transform.GetChild(id).gameObject.SetActive(true);
	}
}
