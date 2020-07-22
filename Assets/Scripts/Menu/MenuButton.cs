using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuButton : MonoBehaviour
{
	public GameObject panel, LED;

	private GameManager gm;
	private GameObject canvas, glow, label;

	[TextArea(2, 10)]
	public string action;

	void Start()
	{
		gm = GameManager.instance;
		canvas = GameObject.Find("Canvas");
		glow = transform.Find("Glow").gameObject;
		label = canvas.transform.Find("Main").transform.Find("Action").gameObject;
	}

	void Update()
	{
		try {
			if (panel.activeSelf) LED.GetComponent<Light>().color = Color.green;
			else LED.GetComponent<Light>().color = Color.red;
		} catch {}
	}

	void OnMouseOver()
	{
		glow.GetComponent<Light>().enabled = true;
		if (canvas.transform.Find("Main").gameObject.activeSelf)
			label.GetComponent<Text>().text = action;
	}

	void OnMouseExit()
	{
		glow.GetComponent<Light>().enabled = false;
		label.GetComponent<Text>().text = "hOI!";
	}

	void OnMouseDown()
	{
		gm.GetComponent<Init>().UnloadPanels();
		GetComponent<AudioSource>().Play();
		panel.SetActive(true);
	}
}
