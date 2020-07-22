using UnityEngine;
using System.Collections;

public class FloatingTextManager : MonoBehaviour {

    DmgText floatingTxtPrefab;
    GameObject hudReference;

	// Use this for initialization
	void Start () {
        floatingTxtPrefab = (DmgText)Resources.Load("Prefabs/DmgTextParent", typeof(DmgText));
        
        hudReference = GameObject.Find("HUD");
    }

    // Create a new instance of the floating text prefab, setting it's text and color (Set it's parent on hierarchy to HUD to make use of Canvas Renderer)
    public void CreateFloatingText(string txt, Vector3 position, Color color)
    {
        DmgText floatingText = (DmgText)Instantiate<DmgText>(floatingTxtPrefab);

        floatingText.name = "FloatingText";
        floatingText.SetText(txt, color);
        floatingText.transform.position = position;
        floatingText.transform.SetParent(hudReference.transform);
    }

    public void CreateFloatingText(string txt, Vector3 position, Color color, Color outlineColor)
    {
        DmgText floatingText = (DmgText)Instantiate<DmgText>(floatingTxtPrefab);

        floatingText.name = "FloatingText";
        floatingText.SetText(txt, color, outlineColor);
        floatingText.transform.position = position;
        floatingText.transform.SetParent(hudReference.transform);
    }
}
