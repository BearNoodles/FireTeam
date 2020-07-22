using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour {

    HUD hud;
    State state;
    Image image;
    Text text;

    // Use this for initialization
    void Start()
    {
        // Get a reference to HUD
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();

        gameObject.GetComponent<Button>().onClick.AddListener(OnMouseDown);
    }

    void Update()
    {
        if (BattleManager.CurrentBattleState == BattleState.Playing && !BattleManager.ActiveCharacter.IsDead
            && BattleManager.Instance.IsPlayersTurn && !BattleManager.ActiveCharacter.IsMoving)
            state = State.Enabled;
        else
            state = State.Disabled;

        image.enabled = (state == State.Enabled) ? true : false;
        text.enabled = (state == State.Enabled) ? true : false;
    }

    void OnMouseDown()
    {
        if (state == State.Enabled)
        {
            BattleManager.GetPlayerController.ChangeState(TurnState.ExitTurn);
            hud.GetCursorManager.CurrentCursor = CursorsType.Default;
            BattleManager.GetPlayerController.TurnOffAOE();
        }
    }
}
