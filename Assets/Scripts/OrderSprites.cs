using UnityEngine;
using System.Collections;

// Set the rendering order of all children in the gameObject
public class OrderSprites : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SpriteRenderer[] childsSprites = GetComponentsInChildren<SpriteRenderer>();

        for(int i = 0; i < childsSprites.Length; i++)
        {
            childsSprites[i].sortingOrder = Formula.SpriteOrder(childsSprites[i].transform.position);
        }
	}
}
