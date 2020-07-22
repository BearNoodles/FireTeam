using UnityEngine;
using System.Collections;

public class Options : MonoBehaviour
{
	private GameManager gm;

	// Use this for initialization
	void Awake ()
	{
		gm = GameManager.instance;
	}
	
	public void GoBack()
	{
		GameObject canvas = GameObject.Find("Canvas").gameObject;
		if (gm.Profile == null) {
			canvas.transform.Find("Main").gameObject.SetActive(true);
		} else {
			canvas.transform.Find("Profile").gameObject.SetActive(true);
		}
	}
}
