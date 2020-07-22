using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewButton : MonoBehaviour
{
	private GameManager gm;
	private bool active = false;
	private int id;
	private string field = "";
	private Button btnSubmit;
	private Animator anim;

	void Awake()
	{
		gm = GameManager.instance;
		field = transform.Find("Input").GetComponent<InputField>().text;
		anim = GameObject.FindGameObjectWithTag("TableInner").GetComponent<Animator>();
		btnSubmit = transform.Find("Submit").GetComponent<Button>();
	}

	void OnLevelWasLoaded(int level)
	{
		if (level == 0)
			anim = GameObject.FindGameObjectWithTag("TableInner").GetComponent<Animator>();
	}

	void Update()
	{
		transform.Find("Name").gameObject.SetActive(!active);
		transform.Find("Status").gameObject.SetActive(!active);
		transform.Find("Input").gameObject.SetActive(active);
		transform.Find("Submit").gameObject.SetActive(active);

		field = transform.Find("Input").GetComponent<InputField>().text;
		btnSubmit.interactable = (field != "");
	}

	public void SetValues(int i)
	{
		id = i;
	}

	public void SetActive(bool val)
	{
		active = val;
	}

	public void CreateNew()
	{
		if (field != "") {
			gm.CreateProfile(id, field);
			try { Destroy(gm.GetComponent<ItemManager>()); }
			catch { Debug.Log("Couldn't destroy old ItemManager"); }
			gm.gameObject.AddComponent<ItemManager>();
			gm.GetComponent<Init>().UnloadPanels();
			gm.GetComponent<Init>().LoadSecondaryPanels();
			//anim.SetTrigger("Load");
			anim.Play("TableIdleLoaded");
			GameObject.Find("Canvas").transform.Find("Shop").GetComponent<Shop>().FirstRun();
		}
	}
}
