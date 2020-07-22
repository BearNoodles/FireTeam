using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoButton : MonoBehaviour
{
	// Use this for initialization
	public void SetValues(int i)
	{
		int a = (i >= 7) ? 3: (i >= 4) ? 2 : (i >= 1) ? 1 : 0;
		transform.Find("Info").gameObject.GetComponent<Text>().text = 
			"Act: " + a + "\n" +
			"Level: " + i;
	}
}
