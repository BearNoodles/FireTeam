using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AOEHandler : MonoBehaviour {

    public List<GameObject> targets; // { get; set; }
    SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start () {
        targets = new List<GameObject>();
        spriteRenderer = GetComponent<SpriteRenderer>();


        SetToActive(false);
    }

    void OnDisable()
    {
        targets.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!targets.Contains(other.gameObject))
        {
            targets.Add(other.gameObject);
        }

        // Debug.Log("Ontriggerenter working");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(targets.Contains(other.gameObject))
        {
            targets.Remove(other.gameObject);
        }

       // Debug.Log("Ontriggerexit working");
    }

    public void SetToActive(bool isactive)
    {
        gameObject.SetActive(isactive);
    }

    public void SetColour(Color color)
    {
        spriteRenderer.color = color + (Color.black * 0.6f);
    }

    public void SetPosition(Vector3 newPos)
    {
        transform.position = Camera.main.ScreenToWorldPoint(newPos);
    }
}
