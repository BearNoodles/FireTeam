using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private Humanoid parent;

    [SerializeField]
    private Image healthImg;
    [SerializeField]
    private Image staminaImg;
    [SerializeField]
    private Text hitChanceTxt;
    [SerializeField]
    private Text txt; // Name

    public Vector3 Offset { get; set; }

    Transform staminaBar;

    void Start()
    {
        string color = Humanoid.GetClassColourHex(parent.CharData.type);
        txt.text = "<color='" + color + "'>" + parent.CharData.type + "</color>" + parent.CharData.name;
        staminaBar = transform.Find("StaminaBar");

        // Give character class a reference to the hit chance display item
        parent.hitChanceText = hitChanceTxt;
    }

    // Update is called once per frame
    void Update ()
    {
        if (parent.IsDead)
        {
            staminaBar.gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }

        // Update bar fill amount and position if player object exists and is alive
        if(parent != null)
        {
            healthImg.fillAmount = (float)parent.CharData.health / parent.MaxHealth;

            if (parent == BattleManager.ActiveCharacter && !parent.IsDead)
            {
                staminaBar.gameObject.SetActive(true);
                staminaImg.fillAmount = 1 - BattleManager.ActiveCharacter.MovedPercentage;
            }
            else
                staminaBar.gameObject.SetActive(false);

            transform.position = parent.transform.position + Offset;
        }
	}

    public void SetHumanParent(GameObject parentObj)
    {
        parent = parentObj.GetComponent<Humanoid>();
    }
}
