using UnityEngine;
using System.Collections;
using System;

public class PlayerController : BasicController {
    
    public enum SubState
    {
        None,
        SelectingTarget,
        SelectingArea
    }

    public TurnState CurrentTurnState
    {
        get
        {
            return currentTurnState;
        }
        set
        {
            if (value == TurnState.ExitTurn)
                hud.DisplayHitChance = false;

            currentTurnState = value;
        }
    }
    public SubState CurrentSubState;

    // References to scene object scripts
	public Stats stats;
    private HUD hud;
    private AOEHandler aoeHandler;

    public PlayerController()
    {
        base.NewTurn(null);
    }

    void Awake()
    {
        aoeHandler = GameObject.Find("AoEField").GetComponent<AOEHandler>();
        hud = GameObject.Find("HUD").GetComponent<HUD>();
		stats = new Stats ();
    }

    void Start()
    {
        CurrentSubState = SubState.None;
    }

    public void TurnOffAOE()
    {
        aoeHandler.SetToActive(false);
    }

    public void CallUpdate()
    {
        if (base.IsTimerActive)
        {
            base.UpdateDelayTimer();
        }
        else
        {
            if (CurrentState == State.Enabled)
            {
                UpdateByState(CurrentTurnState);
            }
        }
    }

    // Handle the change between states and any settings that require changing
    public void ChangeState(TurnState gameState)
    {
        hud.DisplayHitChance = false;
        DecideSubState(gameState);
        switch(gameState)
        {
            case TurnState.Attacking:
            case TurnState.UsingAbility:
                if (CurrentSubState == SubState.SelectingTarget)
                    hud.DisplayHitChance = true;
                break;
            case TurnState.ExitTurn:
                base.NewDelay();
                break;
        }

        CurrentTurnState = gameState;
    }

    private void DecideSubState(TurnState gameState)
    {
        if (gameState == TurnState.Attacking)
            CurrentSubState = (ActiveHuman.Attack.Type == SkillType.Single) ? SubState.SelectingTarget : SubState.SelectingArea;
        else if (gameState == TurnState.UsingAbility)
            CurrentSubState = (ActiveHuman.Ability.Type == SkillType.Single) ? SubState.SelectingTarget : SubState.SelectingArea;
        else
            CurrentSubState = SubState.None;
    }

    public void UpdateByState(TurnState gameState)
    {
        switch (gameState)
        {
            case TurnState.None:
                break;
            case TurnState.AwaitingInput:
                UpdateAwaitingInput();
                break;
            case TurnState.Moving:
                UpdateMoving();
                break;
            case TurnState.Attacking:
                CheckCancelActionInput();

                if (CurrentSubState == SubState.SelectingTarget)
                    UpdateAttackTarget();

                else if (CurrentSubState == SubState.SelectingArea)
                    UpdateAttackingArea();
                break;
            case TurnState.UsingAbility:
                CheckCancelActionInput();
                
                // Update Ability based on whether ability is single or multi target
                if (CurrentSubState == SubState.SelectingTarget)
                    UpdateAbilityTarget();
                else if (CurrentSubState == SubState.SelectingArea)
                    UpdateAbilityArea();
                break;
            case TurnState.ExitTurn:
                BattleManager.IsTurnOver = true;
                break;
            default:
                Debug.LogError("ProcessStateChange could not process TurnState specified.");
                break;
        }
    }

    private void CheckCancelActionInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ChangeState(TurnState.AwaitingInput);
            aoeHandler.SetToActive(false);
            hud.GetCursorManager.CurrentCursor = CursorsType.Default;
        }
    }

    private void UpdateMoving()
    {
        // Once player stops moving, revert back to waiting for an action
        if (base.ActiveHuman.IsMoving == false)
        {
			ActiveHuman.soundStatus = SoundStatus.None;
            CurrentTurnState = TurnState.AwaitingInput;
            Debug.Log("Movement finished.");
        }

        DisplayStateMessage(TurnState.Moving, "Moving..");
    }

    private void UpdateAwaitingInput()
    {
        if (base.ActiveHuman.IsMoving == true) 
		{
			ActiveHuman.soundStatus = SoundStatus.Walk;
			CurrentTurnState = TurnState.Moving;
		}

        Debug.Log("Making it inside UpdateByState");
        // Player Update Logic here, most likely use a 'isActive' boolean value to stop it from always updating.
        if (Input.GetMouseButtonDown(0))
        {
            // Skip rest of method if UI is clicked
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            // Set the players target position which calls a coroutine for movement
            ActiveHuman.TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        // Display move message if player can still move
        if(ActiveHuman.MovedPercentage * 100 < 98)
            DisplayStateMessage(TurnState.AwaitingInput, "Click to move.");
        else
            DisplayStateMessage(TurnState.AwaitingInput, "<color=red>Out of stamina</color>, select an action.");

        DisplayMousePointer(TurnState.AwaitingInput, CursorsType.Default);
    }

    #region Attack Update Methods Group

    public void UpdateAttackTarget()
    {
        if (HasAttacked)
            CurrentTurnState = TurnState.ExitTurn;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        // Perform attack when target is clicked on && Display target information when hovering over target
        if (hit != false && hit.collider != null)
        {
            // Return from method if hit item is part of the blocking layer or is the current active human
            if (hit.collider.tag == "Blocking" || hit.collider.gameObject == ActiveHuman.gameObject)
                return;

            TextColorPack colorPack = ActiveHuman.Attack.CategoryColor();

            if (Input.GetMouseButtonDown(0))
            {
                ActiveHuman.SetFacingDirection((ActiveHuman.Position - hit.transform.position).x);
                ActiveHuman.PlayShootAnim();
               

                if (IsHit(hit.collider.gameObject))
                {
					stats.hits++;
					stats.shotsFired++;
                    ActiveHuman.soundStatus = SoundStatus.Hit;
                    ActiveHuman.Attack.Perform(hit.collider.gameObject, ActiveHuman.CharData, base.floatingTextMgr);
                }
                else
                {
					stats.misses++;
					stats.shotsFired++;
                    ActiveHuman.soundStatus = SoundStatus.Miss;
                    base.DisplayMissMsg(hit.transform.position, colorPack.fontColor, colorPack.outlineColor);
                }

                ActiveHuman.Recovery += ActiveHuman.Attack.RecoveryCost;
                ChangeState(TurnState.ExitTurn);
            }

            DisplayStateMessage(TurnState.Attacking, "Use <color=" + colorPack.fontColor.ColorAsHex() + ">" + ActiveHuman.Attack.Name + "</color> On: " + hit.collider.name);
        }
        else
        {
            DisplayStateMessage(TurnState.Attacking, "Select a target.");
        }

        DisplayMousePointer(TurnState.Attacking, CursorsType.Skill);
    }

    private void UpdateAttackingArea()
    {
        // Display Area
        aoeHandler.SetToActive(true);

        aoeHandler.SetPosition(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            ActiveHuman.Attack.Perform(aoeHandler.targets.ToArray(), ActiveHuman.CharData, base.floatingTextMgr);

            aoeHandler.SetToActive(false);

            ActiveHuman.Recovery += ActiveHuman.Attack.RecoveryCost;

            ChangeState(TurnState.ExitTurn);
        }

        DisplayStateMessage(TurnState.Attacking, "Select an area to attack.");
        DisplayMousePointer(TurnState.Attacking, CursorsType.Default);
        DisplayAoeField(TurnState.UsingAbility);
    }

    #endregion

    #region Ability Update Methods Group

    private void UpdateAbilityTarget()
    {
        if (HasAttacked)
            CurrentTurnState = TurnState.ExitTurn;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        // Perform attack when target is clicked on && Display target information when hovering over target
        if (hit != false && hit.collider != null)
        {
            // Return from method if hit item is part of the blocking layer or is the current active human
            if (hit.collider.tag == "Blocking")
                return;

            TextColorPack colorPack = ActiveHuman.Ability.CategoryColor();

            if (Input.GetMouseButtonDown(0))
            {
                // Stop traps and collectables from being hit by heals and support abilities
                if (ActiveHuman.Ability.Category == SkillCategory.Support || ActiveHuman.Ability.Category == SkillCategory.Heal)
                    if (hit.collider.tag == "Trap" || hit.collider.tag == "Collectable")
                        return;           

                // Check if skill will land then either perform attack or display miss msg
                if (IsHit(hit.collider.gameObject))
                    ActiveHuman.Ability.Perform(hit.collider.gameObject, ActiveHuman.CharData, base.floatingTextMgr);
                else
                    base.DisplayMissMsg(hit.transform.position, colorPack.fontColor, colorPack.outlineColor);

				ActiveHuman.soundStatus = SoundStatus.Ability;

                ChangeState(TurnState.ExitTurn);
            }

            if (hit.collider.tag == "Player" || hit.collider.tag == "Enemy")
                DisplayStateMessage(TurnState.UsingAbility, "Use <color=" + colorPack.fontColor.ColorAsHex() + "> " + ActiveHuman.Ability.Name + " </color> on: " + hit.collider.name); 
        }
        else
        {
            DisplayStateMessage(TurnState.UsingAbility, "Select a target.");
        }

        DisplayMousePointer(TurnState.UsingAbility, CursorsType.Skill);
    }

    private void UpdateAbilityArea()
    {
		if (Input.GetMouseButtonDown (0)) {
			//ActiveHuman.soundStatus = SoundStatus.Ability;
		}
        // Display Area and position AOE field
        aoeHandler.SetToActive(true);
        aoeHandler.SetPosition(Input.mousePosition);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float targetDistance = Vector3.Distance(ActiveHuman.Position, mousePos);

        TextColorPack colorPack = ActiveHuman.Ability.CategoryColor();

        if (ActiveHuman.Ability.IsInRange(targetDistance))
        {
            // Perform Ability on targets and change state when mouse is clicked
            if (Input.GetMouseButtonDown(0))
            {
                if (ActiveHuman.Ability.Category == SkillCategory.Attack)
				{
					ActiveHuman.soundStatus = SoundStatus.Ability;
                    DmgEffectManager.CreateHit(aoeHandler.transform.position);
				}

                ActiveHuman.Ability.Perform(aoeHandler.targets.ToArray(), ActiveHuman.CharData, floatingTextMgr);

                aoeHandler.SetToActive(false);

                ActiveHuman.Recovery += ActiveHuman.Ability.RecoveryCost;

                ChangeState(TurnState.ExitTurn);
            }

            aoeHandler.SetColour(Color.green);
            DisplayStateMessage(TurnState.UsingAbility, "Select an area to use <color=" + colorPack.fontColor.ColorAsHex() + ">" + ActiveHuman.Ability.Name + "</color>.");
        }
        else
        {
            aoeHandler.SetColour(Color.red);
            DisplayStateMessage(TurnState.UsingAbility, "<color=red>Out of range</color> of user.");
        }

        DisplayMousePointer(TurnState.UsingAbility, CursorsType.Hide);
        DisplayAoeField(TurnState.UsingAbility);
        //hud.GetCursorManager.CurrentCursor = CursorsType.Hide;
    }

    #endregion

    // Display status msg if input state is still the same as current state
    public void DisplayStateMessage(TurnState state, string msg)
    {
        hud.StatusMsg = (CurrentTurnState == state) ? msg : "";
    }

    public void DisplayMousePointer(TurnState state, CursorsType cursorType)
    {
        BattleManager.GetMouseCursorMgr.CurrentCursor = (CurrentTurnState == state) ? cursorType : CursorsType.Default;
    }

    public void DisplayAoeField(TurnState state)
    {
        aoeHandler.SetToActive(CurrentTurnState == state);
    }

    public override void NewTurn(Humanoid activeCharacter)
    {
        base.NewTurn(activeCharacter);

        hud.GetCursorManager.CurrentCursor = CursorsType.Default;

        ChangeState(CurrentTurnState);
    }
}
