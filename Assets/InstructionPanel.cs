using UnityEngine;
using System.Collections;

public class InstructionPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}

    public void OnCloseButtonDown()
    {
        gameObject.SetActive(false);
        // Enable Game
    }
}
