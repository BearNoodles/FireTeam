using UnityEngine;
using System.Collections;

public class HitAnim : MonoBehaviour {

	[SerializeField]
	bool isDestroyTime;

	// Update is called once per frame
	void Update () {
		if (isDestroyTime)
			Destroy (gameObject);
	}
}
