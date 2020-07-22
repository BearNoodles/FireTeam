using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour {
    public static Text textInstance;

    public string CharacterName { get; set; }
    public Humanoid Character;
    public Color CharColor { get; set; }

    void Start()
    {
        if (textInstance == null)
        {
            GameObject turnBar = GameObject.Find("TurnBar");
            textInstance = turnBar.transform.Find("Text").GetComponent<Text>();
        }
    }

    public void pointerEnter()
    {
        textInstance.text = CharacterName;
        textInstance.color = CharColor;

        Debug.Log("OnCharEnter Fired");
    }

    public void SetPanel(string name, bool isPlayer)
    {
        if(isPlayer)
        {
            CharacterName = name;
            CharColor = Color.green;
        }
        else
        {
            CharacterName= name;
            CharColor = Color.red;
        }
    }
}
