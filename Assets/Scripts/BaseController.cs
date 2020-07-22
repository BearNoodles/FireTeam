using UnityEngine;
using System.Collections;

public enum State
{
    Enabled,
    Disabled
}

public class BasicController : MonoBehaviour {

    protected TurnState currentTurnState;

    protected bool HasMoved { get; set; }
    public bool HasAttacked { get; set; }
    public State CurrentState { get; set; }
    protected Humanoid ActiveHuman { get; set; }

    // TODO:- Implement delay timer
    protected float DelayTime { get; set; }
    protected bool IsTimerActive { get { return timeElapsed <= DelayTime; } }
    protected float timeElapsed;

    protected FloatingTextManager floatingTextMgr;

    public virtual void NewTurn(Humanoid activeCharacter)
    {
        HasMoved = false;
        HasAttacked = false;
        CurrentState = State.Enabled;
        ActiveHuman = activeCharacter;
        DelayTime = 0.5f;
        timeElapsed = 0;

        currentTurnState = TurnState.AwaitingInput;
    }

    protected void UpdateDelayTimer()
    {
        timeElapsed += Time.deltaTime;
    }

    protected void NewDelay()
    {
        timeElapsed = 0;
    }

    // Decide whether skill landed based on hit chance formula
    protected bool IsHit(GameObject target)
    {
        float hitRoll = Random.Range(0, 100);

        float hitChance = Formula.CalculateHitChance(Vector2.Distance(target.transform.position, ActiveHuman.transform.position));

        return (hitChance > hitRoll) ? true : false; 
    }

    protected void DisplayMissMsg(Vector3 targetPosition, Color textColor, Color outlineColor)
    {
        Vector3 textOffset = new Vector3(0, 30);
        floatingTextMgr.CreateFloatingText("Miss", targetPosition + textOffset, textColor, outlineColor);
    }

    public void SetFloatingTextMgr(FloatingTextManager ftm)
    {
        floatingTextMgr = ftm;
    }
}
