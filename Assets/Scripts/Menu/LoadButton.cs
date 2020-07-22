using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadButton : MonoBehaviour
{
	public void SetValues(PlayerProfile player)
	{
		transform.Find("Name").GetComponent<Text>().text = player.name;
		transform.Find("Progress").GetComponent<Text>().text = "Progress: " + player.GetProgress() + "%";
	}
}
