using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class AttackButton : MonoBehaviour {

    HUD hud;
    State state;
    Image image;

    Text txt;

	// Use this for initialization
	void Start()
    {
        // Get a reference to HUD
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        image = GetComponent<Image>();

        gameObject.GetComponent<Button>().onClick.AddListener(OnMouseDown);
        txt = transform.Find("Text").GetComponent<Text>();
        txt.text = "";
	}
	
    void Update()
    {
        if (BattleManager.CurrentBattleState == BattleState.Playing && !BattleManager.ActiveCharacter.IsDead 
            && BattleManager.Instance.IsPlayersTurn && !BattleManager.ActiveCharacter.IsMoving)
            state = State.Enabled;
        else
            state = State.Disabled;

        image.enabled = (state == State.Enabled) ? true : false;
    }

    void OnMouseDown()
    {
        if (state == State.Enabled)
        {
            BattleManager.Instance.SetPlayerTurnState = TurnState.Attacking;
            hud.GetCursorManager.CurrentCursor = CursorsType.Skill;
            BattleManager.GetPlayerController.TurnOffAOE();
        }
    }

    public void OnMouseEnter()
    {
        PlayerController playerController = BattleManager.GetPlayerController;
        IAttack attack = BattleManager.ActiveCharacter.Attack;
        TextColorPack colorPack = attack.CategoryColor();

        txt.text = "Use <color=" + colorPack.fontColor.ColorAsHex() + ">" + attack.Name + "</color> (+" + attack.RecoveryCost + " recovery)";
        txt.gameObject.SetActive(true);
    }
}
