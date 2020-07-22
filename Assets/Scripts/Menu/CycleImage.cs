using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CycleImage : MonoBehaviour
{
	private Sprite[] frames;
	private int i, frameLength;

	void Awake()
	{
		frames = Resources.LoadAll<Sprite>("Sprites/static");
		frameLength = frames.Length - 1;
	}
	
	void Update()
	{
		GetComponent<Image>().sprite = frames[i];
		if (i < frameLength) i++; else i = 0;
	}
}
