using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyController : BasicController {

    public List<Humanoid> TeamAi { get; set; }
    public List<Humanoid> TeamPlayers { get; set; }

    AIState aiState;
    Humanoid Target;
    ActionScore winningAction;

    [SerializeField]
    int optimalDistance = 140;
    [SerializeField]
    float medicHealingAffector = 1;
    [SerializeField]
    float medicAttackingAffector = 1;
    [SerializeField]
    float medicSelfAffector = 1;

    void Awake()
    {
        TeamAi = new List<Humanoid>();
        TeamPlayers = new List<Humanoid>();

        if (optimalDistance <= 100)
            optimalDistance = 101;
    }

    struct ActionScore
    {
        public AIState action;
        public Humanoid Target;
        public float Score;
    }

    [Flags]
    public enum AIState
    {
        NotAssigned,
        Defensive,
        Heal,
        Offensive
    }

    public EnemyController()
    {
        base.NewTurn(null);
    }

    public override void NewTurn(Humanoid activeCharacter)
    {
        base.NewTurn(activeCharacter); // TODO:- Need this?

        winningAction = DecideAction();
        Debug.Log("AI winning state: " + winningAction.action.ToString());
        NewDelay(); // To Slow stuff Down
    }

    public void CallUpdate()
    {
        if (base.IsTimerActive)
        {
            base.UpdateDelayTimer();
        }
        else
        {
            // If enemy ai has performed an attack then end turn and return from method
            if (HasAttacked)
            {
                ActiveHuman.StopMovement();
                EndTurn();
                return;
            }

            if (CurrentState == State.Enabled)
                ActivateAction(winningAction);
        }
    }

    private void ActivateAction(ActionScore winningScore)
    {
        switch(winningScore.action)
        {
            case AIState.Offensive:
                UseAttack(winningScore.Target);
                break;
            case AIState.Heal:
                UseHeal(winningScore.Target);
                break;
            case AIState.Defensive:
                UseDefensive(winningScore.Target);
                break;
            case AIState.NotAssigned:
                Debug.Log("Ai State not assigned? Skip turn maybe?");
                break;
        }
    }

    #region Decision Making

    private ActionScore DecideAction()
    {
        List<ActionScore> actionScores = new List<ActionScore>();

        ActionScore attack;

        // Get weight of each healing if current unity is a medic
        if (ActiveHuman.CharData.type == UnitType.Medic)
        {
            ActionScore heal = GetGeneralScore(TeamAi, SkillCategory.Heal, medicSelfAffector, medicHealingAffector);
            heal.action = AIState.Heal;
            actionScores.Add(heal);

            attack = GetGeneralScore(TeamPlayers, SkillCategory.Attack, medicAttackingAffector);
        }
        else
        {
            // Get weight of attacking action
            attack = GetGeneralScore(TeamPlayers);
        }

        attack.action = AIState.Offensive;
        actionScores.Add(attack);

        // Get weight of defensive action
        ActionScore defensive = GetDefensiveScore();
        defensive.action = AIState.Defensive;
        actionScores.Add(defensive);

        // TODO:- Add Defensive weight

        return GetHighest(actionScores);
    }

    private ActionScore GetGeneralScore(List<Humanoid> targets, SkillCategory category = SkillCategory.Attack, float selfEffector = 1, float targetEffector = 1)
    {
        List<ActionScore> Targets = new List<ActionScore>();
  
        // Calculate healing weight for all alive team members
        foreach (Humanoid h in targets)
        {
            if (h.IsDead)
                continue;

            ActionScore newTarget = new ActionScore();
            newTarget.Target = h;

            // Get chance of hitting target
            float distanceFrom = Vector2.Distance(h.transform.position, ActiveHuman.transform.position);
            float distanceScore = GetDistanceScore(distanceFrom);

            // Get health score applying self effector if active player
            if (h.GetHashCode() == ActiveHuman.GetHashCode())
            {
                float generalScore = GetHealthScore(h.CharData.health, h.MaxHealth, category, selfEffector, targetEffector);             
                newTarget.Score = distanceScore * (generalScore / 50);
                //Debug.Log("General state score: " + newTarget.Score);
            }
            else
            {
                newTarget.Score = distanceScore * (GetHealthScore(h.CharData.health, h.MaxHealth, category, targetEffector) / 50);
                //Debug.Log("General state score: " + newTarget.Score);
            }

            // Cap Score
            if (newTarget.Score > 50)
                newTarget.Score = 50;

            Targets.Add(newTarget);
        }

        return GetHighest(Targets);
    }

    private ActionScore GetDefensiveScore()
    {
        int enemyCount = MembersInRange(TeamPlayers, optimalDistance);

        Humanoid closestFriendly = Closest(TeamAi);

        float enemyCountScore = (enemyCount > 2) ? 50 : 0; // High if more than 2 enemies     
        ActionScore defensiveScore = new ActionScore();
        defensiveScore.Score = enemyCountScore * (GetHealthScore(ActiveHuman.CharData.health, ActiveHuman.MaxHealth) / 50);   
        defensiveScore.Target = closestFriendly;

        if (closestFriendly == null)
            defensiveScore.Score = 0;

        Debug.Log("Enemy Count:" + enemyCount);
        Debug.Log("Defensive state score: " + defensiveScore.Score);
        return defensiveScore;
    }

    private int MembersInRange(List<Humanoid> team, int range)
    {
        int count = 0;

        foreach (Humanoid h in team)
        {
            float DistanceFrom = Vector2.Distance(h.transform.position, ActiveHuman.transform.position);

            if (DistanceFrom < range)
                count++;
        }

        return count;
    }

    private Humanoid Closest(List<Humanoid> team)
    {
        Humanoid closest = null;
        float minDistance = 10000;

        foreach (Humanoid h in team)
        {
            // Skip active player
            if (h.GetHashCode() == ActiveHuman.GetHashCode())
                continue;

            float distance = Vector3.Distance(ActiveHuman.Position, h.Position);

            if (minDistance > distance)
            {
                closest = h;
                minDistance = distance;
            }
        }

        return closest;
    }

    private ActionScore GetHighest(List<ActionScore> targetScores)
    {
        ActionScore highest = new ActionScore();

        foreach(ActionScore ts in targetScores)
        {
            if (ts.Score >= highest.Score)
                highest = ts;
        }

        return highest;
    }

    // Decide how much impact health will have on the decision
    private float GetHealthScore(int health, int maxHealth, SkillCategory category = SkillCategory.Attack, float selfEffector = 1, float targetEffector = 1)
    {
        int score = 0;

        if (category == SkillCategory.Heal && health > maxHealth * 0.8f) // Greater than 80% of MaxHealth
            score = 0;
        else if (health >= maxHealth * 0.5f) // 50% of MaxHealth
            score = 10;
        else if (health >= maxHealth * 0.3f) // 30% of MaxHealth
            score = 20;
        else if (health >= maxHealth * 0.15f) // 15% of MaxHealth
            score = 30;
        else if (health < maxHealth * 0.15f) // Less than 15% of MaxHealth
            score = 50;

        return score * targetEffector * selfEffector;
    }

    // Decide how much impact distance will have on the decision
    private float GetDistanceScore(float distance)
    {
        int score = 0;

        if (distance > optimalDistance * 2) // Twice the optimal distance
            score = 5;
        else if (distance >= optimalDistance * 1.75f)
            score = 10;
        else if (distance >= optimalDistance * 1.50f)
            score = 20;
        else if (distance >= optimalDistance * 1.25f)
            score = 30;
        else if (distance >= optimalDistance * 1 || distance < optimalDistance)
            score = 50;

        return score;
    }

    #endregion

    #region Action Performing

    // Perform an attack or move towards target
    private void UseAttack(Humanoid target)
    {
        if(ActiveHuman.Attack.Type == SkillType.Single)
        {
            float targetDist = Vector2.Distance(target.transform.position, ActiveHuman.Position); // get Target Position 
            //bool moveCloser = false;

            // Perform attack if less than 140% of optimal distance otherwise move towards player
            if (targetDist < optimalDistance * 1.4f)
            {
                ActiveHuman.SetFacingDirection((ActiveHuman.Position - target.Position).x);
                ActiveHuman.PlayShootAnim();

                TextColorPack colorPack = ActiveHuman.Attack.CategoryColor();

                bool performSupportAbility = false;

                if (ActiveHuman.CharData.type == UnitType.Support)
                {
                    float roll = UnityEngine.Random.value * 100;
                    performSupportAbility = (roll >= 50) ? true : false;
                    colorPack = ActiveHuman.Ability.CategoryColor();
                }

                if (IsHit(target.gameObject))
				{

                    if (ActiveHuman.CharData.type != UnitType.Support || !performSupportAbility)
                    {
                        ActiveHuman.Attack.Perform(target.gameObject, ActiveHuman.CharData, base.floatingTextMgr);
                        ActiveHuman.soundStatus = SoundStatus.Hit;
                    }
                    else if(ActiveHuman.CharData.type == UnitType.Support && performSupportAbility)
                    {
                        Debug.Log("Used ability");
                        ActiveHuman.Ability = new SuppressiveFire(SkillType.Single);

                        ActiveHuman.Ability.Perform(target.gameObject, ActiveHuman.CharData, base.floatingTextMgr);
                        ActiveHuman.soundStatus = SoundStatus.Ability;
                    }
				}
                else
				{
                    DisplayMissMsg(target.transform.position, colorPack.fontColor, colorPack.outlineColor);
					ActiveHuman.soundStatus = SoundStatus.Miss;
				}

                HasAttacked = true;

                Debug.Log("Inside Enemy Controller Stuck at ability");
            }
            else
            {
                // Move if possible and hasn't already moved, otherwise 
                AttemptMove(target.Position);
            }
        }
        else if(ActiveHuman.Attack.Type == SkillType.Multiple)
        {
            // is any player members close enough?
            // Get Position between player member
            // is within 140% of optimal range?
            // no? then move closer
			//ActiveHuman.soundStatus = SoundStatus.Ability;
        }

        NewDelay();
    }

    private void AttemptMove(Vector3 position)
    {
        if (!HasMoved)
        {
            if (!ActiveHuman.IsMoving)
			{
                Move(position);
				ActiveHuman.soundStatus = SoundStatus.Walk;
			}

            if(ActiveHuman.MovedPercentage * 100 > 98)
			{
                HasMoved = true;
				ActiveHuman.soundStatus = SoundStatus.None;
			}
        }
        else
        {
            if (!ActiveHuman.IsMoving)
                EndTurn();
        }
    }

    // Perform a heal or move towards target
    private void UseHeal(Humanoid target)
    {
        if (ActiveHuman.Ability.Type == SkillType.Single)
        {
            float targetDist = Vector2.Distance(target.transform.position, ActiveHuman.Position); // get Target Position 
            //bool moveCloser = false;

            // Perform attack if less than 140% of optimal distance otherwise move towards player
            if (targetDist < optimalDistance * 1.4f)
            {
                TextColorPack colorPack = ActiveHuman.Ability.CategoryColor();

                // Test if heal landed or missed
                if (IsHit(target.gameObject))
				{
                    ActiveHuman.Ability.Perform(target.gameObject, ActiveHuman.CharData, base.floatingTextMgr);
					ActiveHuman.soundStatus = SoundStatus.Ability;
				}
                else
                    DisplayMissMsg(target.transform.position, colorPack.fontColor, colorPack.outlineColor);

                HasAttacked = true; // Ends turn
            }
            else
            {
                AttemptMove(target.Position);
            }
        }
        else if (ActiveHuman.Ability.Type == SkillType.Multiple)
        {
            // is any team members close enough?
            // Get Position between player member
            // is within 140% of optimal range?
            // no? then move closer
			//ActiveHuman.soundStatus = SoundStatus.Ability;
        }

        NewDelay();
    }

    private void UseDefensive(Humanoid target)
    {
        if (target == null)
        {
            Debug.Log("Can't find defensive target");
        }

        float targetDist = Vector2.Distance(target.transform.position, ActiveHuman.Position);                                                                                            

        // Move if possible and hasn't already moved
        AttemptMove(target.Position);

        // Attempt to hit nearest target if any movement is complete
        if(HasMoved)
        {
            Humanoid closestTarget = Closest(TeamPlayers);

            if (closestTarget == null)
            {
                HasAttacked = true;
                return;
            }

            targetDist = Vector2.Distance(closestTarget.transform.position, ActiveHuman.Position);

            // Perform attack if less than 140% of optimal distance
            if (targetDist < optimalDistance * 1.4f)
            {
                ActiveHuman.SetFacingDirection((ActiveHuman.Position - closestTarget.Position).x);
                ActiveHuman.PlayShootAnim();

                TextColorPack colorPack = ActiveHuman.Attack.CategoryColor();

                if (IsHit(target.gameObject))
				{
                    ActiveHuman.Attack.Perform(closestTarget.gameObject, ActiveHuman.CharData, base.floatingTextMgr);
					ActiveHuman.soundStatus = SoundStatus.Hit;
				}
                else
				{
                    DisplayMissMsg(closestTarget.transform.position, colorPack.fontColor, colorPack.outlineColor);
					ActiveHuman.soundStatus = SoundStatus.Miss;
				}

                HasAttacked = true;
            }
        }

        Debug.Log("HasMoved: " + HasMoved);
        Debug.Log("HasAttacked: " + HasAttacked);
    }

    // Attempt to move to target position
    private void Move(Vector3 target)
    {
        RaycastHit2D hit = Physics2D.Linecast(ActiveHuman.Position, target, 1 << LayerMask.NameToLayer("Blocking"));

        Vector3 direction = ActiveHuman.Position - target;
        direction.z = 0;

        // Move if nothing is hit
        if (hit.collider == null)
        {
            // Calculate Optimal Distance TODO- Check this (Moving to wrong position
            Vector3 optimalPosition = target + (direction.normalized * optimalDistance);
            ActiveHuman.TargetPosition = optimalPosition;

            HasMoved = true;
        }
        else
        {
            direction *= -1; // Flip the direction to be pointing at enemy

            float SpinAmount = 15; 

            // Get players favourite spin direction
            float favourSpinDir = (ActiveHuman as Enemy).FavourMove; // -1 anti-clockwise, 1 clockwise; 
            int lineLength = 150; // Distance to fire line test 

            float currentRotation = 0;
            bool isSuccess = false;

            Vector3 normalizedDir = direction.normalized;
            Vector3 endPosition = Vector3.zero;

            // Set spin direction
            SpinAmount *= favourSpinDir;

            // Test locations around enemy for a suitable place to move
            while(!isSuccess)
            {
                currentRotation += SpinAmount;

                if (currentRotation > 360 || currentRotation < -360)
                {
                    isSuccess = false;
                    break;
                }

                // Rotate the vector by preset rotation amount
                float cos = Mathf.Cos(Mathf.Deg2Rad * currentRotation);
                float sin = Mathf.Sin(Mathf.Deg2Rad * currentRotation);

                float x = normalizedDir.x * cos - normalizedDir.y * sin;
                float y = normalizedDir.x * sin + normalizedDir.y * cos;

                Vector3 newDirection = new Vector3(x, y, 0);
                endPosition = ActiveHuman.Position + (newDirection * lineLength);

                // Test to see if path is clear
                RaycastHit2D newHit = Physics2D.Linecast(ActiveHuman.Position, endPosition, 1 << LayerMask.NameToLayer("Blocking"));
                Debug.DrawLine(ActiveHuman.Position, endPosition, Color.red, 2f, true);

                // Move if path is clear
                if (newHit.collider == null)
                {
                    isSuccess = true;
                    break;
                }
            }

            // Perform movement if path was found
            if (isSuccess)
            {
                ActiveHuman.TargetPosition = endPosition;
            }
            else
                Debug.Log("Ai could not locate a suitable path.");
        }
    }
    #endregion

    void EndTurn()
    {
        BattleManager.IsTurnOver = true;
    }
}
