using UnityEngine;
using System.Collections;

public class DmgEffectManager : MonoBehaviour {

    private static Transform hit;

	// Use this for initialization
	void Start () {
        hit = Resources.Load<Transform>("Prefabs/Hit");
	}
	
    public static void CreateHit(Vector3 position)
    {
        position.y += 30;

        position.y += (Random.value * 2 - 1) * 10;
        position.x += (Random.value * 2 - 1) * 10;
        Instantiate(hit, position, Quaternion.identity);
    }
}
