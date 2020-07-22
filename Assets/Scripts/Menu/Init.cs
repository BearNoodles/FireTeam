using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Init : MonoBehaviour
{
	void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		UnloadPanels();
		LoadPrimaryPanels();
	}

	public void UnloadPanels()
	{
		GameObject canvas = GameObject.Find("Canvas");
		for (int i = 1; i < canvas.transform.childCount; i++)
			canvas.transform.GetChild(i).gameObject.SetActive(false);
	}

	public void LoadPrimaryPanels()
	{
		GameObject[] primary = GameObject.FindGameObjectsWithTag("MenuPanel1");

		for (int i = 0; i < primary.Length; i++)
		{
			primary[i].SetActive(true);
			primary[i].SetActive(false);
		}

		GameObject.Find("Canvas").transform.Find("Main").gameObject.SetActive(true);
	}

	public void LoadSecondaryPanels()
	{
		GameObject[] secondary = GameObject.FindGameObjectsWithTag("MenuPanel2");
		
		for (int i = 0; i < secondary.Length; i++)
		{
			secondary[i].SetActive(true);
			secondary[i].SetActive(false);
		}

		GameObject.Find("Canvas").transform.Find("Profile").gameObject.SetActive(true);
	}
}
