using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

    int currencyContained;

	// Use this for initialization
	void Start () {
        currencyContained = 5;
        GetComponent<SpriteRenderer>().sortingOrder = Formula.SpriteOrder(transform.position);
	}

    public void ApplyDamage(float dmg)
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Vector3 textOffset = new Vector3(0, 55, 0);

            // Display Currency Text
            BattleManager.GetFloatingTextManager.CreateFloatingText("+" + (currencyContained * Application.loadedLevel).ToString() + " gold", other.transform.position + textOffset, Color.yellow, Color.black);

            GameManager.instance.Profile.AddCurrency(currencyContained * Application.loadedLevel);

            Destroy(gameObject);
        }
    }
}
