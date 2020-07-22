using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {

    [SerializeField]
    float dmgAmount;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "TrapDetonator")
        {
            Vector3 textOffset = new Vector3(0, 55, 0);

            other.transform.parent.gameObject.SendMessage("ApplyDamage", dmgAmount);
            BattleManager.GetFloatingTextManager.CreateFloatingText(dmgAmount.ToString(), other.transform.position + textOffset, Color.red, Color.yellow);
            DmgEffectManager.CreateHit(other.transform.position);

            Destroy(gameObject);
        }
    }

    public void ApplyDamage(float dmg)
    {
        Destroy(gameObject);
    }
}
