using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityButton : MonoBehaviour {

    HUD hud;
    State state;
    Image image;

    Text txt;

    // Use this for initialization
    void Start()
    {
        // Get a reference to HUD
        hud = BattleManager.GetHUD;
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
            BattleManager.Instance.SetPlayerTurnState = TurnState.UsingAbility;
            hud.GetCursorManager.CurrentCursor = CursorsType.Skill;
            BattleManager.GetPlayerController.TurnOffAOE();
        }
    }

    public void OnMouseEnter()
    {
        PlayerController playerController = BattleManager.GetPlayerController;
        IAbility ability = BattleManager.ActiveCharacter.Ability;
        TextColorPack colorPack = ability.CategoryColor();

        txt.text = "Use <color=" + colorPack.fontColor.ColorAsHex() + ">" + ability.Name + "</color> (+" + ability.RecoveryCost + " recovery)";
        Debug.Log("OnMouseEnter() Called.");
        txt.gameObject.SetActive(true);
    }
}
