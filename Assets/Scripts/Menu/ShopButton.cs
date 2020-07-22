using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopButton : MonoBehaviour
{
	// Use this for initialization
	public void SetValues(ShopItem item)
	{
		transform.Find("Name").gameObject.GetComponent<Text>().text = item.Name;
		transform.Find("Stats").gameObject.GetComponent<Text>().text =
			"ATK" + item.GetStat(0).ToString("+#;-#;+0") + " | DEF" + item.GetStat(1).ToString("+#;-#;+0");
		transform.Find("Price").gameObject.GetComponent<Text>().text = item.GetStat(2) + " CR";

		try {
			transform.Find("Checkbox/Border").GetComponent<Image>().color = item.Owned ? Color.green : Color.black;
			transform.Find("Checkbox/Check").gameObject.SetActive(item.Owned);
		} catch {}
	}
}
