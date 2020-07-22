using UnityEngine;
using System.Collections;

public class BearTrap : MonoBehaviour {

    [SerializeField]
    float recoveryAmount;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "TrapDetonator")
        {
            Vector3 textOffset = new Vector3(0, 55, 0);

            other.transform.parent.gameObject.SendMessage("ApplyRecovery", recoveryAmount);
            BattleManager.GetFloatingTextManager.CreateFloatingText("+" + recoveryAmount.ToString() + " Recovery", other.transform.position + textOffset, Color.blue, Color.yellow);
            DmgEffectManager.CreateHit(other.transform.position);

            if (BattleManager.ActiveCharacter.tag == "Player")
                BattleManager.GetPlayerController.CurrentTurnState = TurnState.ExitTurn;
            else if (BattleManager.ActiveCharacter.tag == "Enemy")
                BattleManager.GetEnemyController.HasAttacked = true;

            Destroy(gameObject);
        }
    }

    public void ApplyDamage(float dmg)
    {
        Destroy(gameObject);
    }
}
