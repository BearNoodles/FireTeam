using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProfileScreen : MonoBehaviour
{
	private GameManager gm;
	private Text values;

	// Use this for initialization
	void Awake()
	{
		gm = GameManager.instance;
		values = transform.Find("Values").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update()
	{
		try {
			values.text = 
					gm.Profile.name + "\n" +
					gm.Profile.GetProgress() + "%\n" +
					gm.Profile.currency + "CR\n" +
					gm.Profile.score;
		} catch {}
	}
}
