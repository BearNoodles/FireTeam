using UnityEngine;
using System.Collections;
using System;

[Serializable]
public abstract class MovableObject : MonoBehaviour {

    // Class fields
    private Vector3 targetPosition;
    private Vector3 currentPosition;
    private bool isMoving;
    private int distanceMoved;
    private bool isFacingLeft;

    private bool IsMaxDistance { get { return (distanceMoved > maxMove); } }

    // Inspector Settings
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private int maxMove;

    // Accessors
    public bool IsMoving { get { return isMoving; } }
    public float MovedPercentage { get { return (float)distanceMoved / maxMove; } }

    protected Animator anim;
    protected SpriteRenderer hatRend, headRend, bodyRend, triggerHandRend, gunRend, gunHandRend, leftLeg, rightLeg;

    protected void GetMovableObjectRefs()
    {
        anim = GetComponent<Animator>();

        // Get references to all the character pieces for use in adjusting sorting order
        hatRend = transform.Find("Head/Hat").GetComponent<SpriteRenderer>();
        headRend = transform.Find("Head").GetComponent<SpriteRenderer>();
        bodyRend = transform.Find("Body").GetComponent<SpriteRenderer>();
        triggerHandRend = transform.Find("GunAndHands/TriggerHand").GetComponent<SpriteRenderer>();
        gunRend = transform.Find("GunAndHands/Gun").GetComponent<SpriteRenderer>();
        gunHandRend = transform.Find("GunAndHands/GunHand").GetComponent<SpriteRenderer>();
        leftLeg = transform.Find("LeftLeg").GetComponent<SpriteRenderer>();
        rightLeg = transform.Find("RightLeg").GetComponent<SpriteRenderer>();

        UpdateSortingOrder(transform.position);
    }

    // Update the sorting order for each of the body parts
    protected void UpdateSortingOrder(Vector3 pos)
    {
        Vector3 offset = Vector3.zero; //new Vector3(pos.x, pos.y - 15, 0);
        int mainSortOrder = Formula.SpriteOrder(pos + offset);

        leftLeg.sortingOrder = mainSortOrder;
        rightLeg.sortingOrder = mainSortOrder;
        bodyRend.sortingOrder = mainSortOrder + 2;
        headRend.sortingOrder = mainSortOrder + 3;
        hatRend.sortingOrder = mainSortOrder + 4;
        gunRend.sortingOrder = mainSortOrder + 5;
        triggerHandRend.sortingOrder = mainSortOrder + 6;
        gunHandRend.sortingOrder = mainSortOrder + 6;
    }

    protected void UpdateMovableObj()
    {
        anim.SetBool("IsRunning", IsMoving); // TODO:- Convert string to hash as string is probably slow
    }

    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }
        set
        {
            currentPosition = targetPosition;
            targetPosition = value;

            StopCoroutine("MoveTo");
            StartCoroutine("MoveTo", targetPosition);
        }
    }

    public void SetFacingDirection(float xmovement)
    {
        transform.eulerAngles = (xmovement <= 0) ? new Vector3(0, 180, 0) : Vector3.zero;
    }

    public void ResetMovement()
    {
        distanceMoved = 0;
    }

    public virtual IEnumerator MoveTo(Vector3 target)
    {
        isMoving = true;

        // Ensure path is clear
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target, 1 << LayerMask.NameToLayer("Blocking"));

        // Break if path is blocked 
        if (hit.collider != null && hit.collider.tag != "TrapDetonator")
        {
            targetPosition = currentPosition;
            isMoving = false;
            yield break;
        }

        float initDirX = (transform.position - target).normalized.x;
        bool isMovingRight = initDirX > 0;

        // Move till less than 15 units away
        while (Vector3.Distance(transform.position, target) > 15f)
        {
            // Postpone movement if paused
            if (PauseMenu.IsPaused)
                yield return new WaitForSeconds(0.02f);

            // If max distance is reached then break from loop
            if (IsMaxDistance)
                break;

            distanceMoved++;

            Vector3 moveDir = (transform.position - target);
            SetFacingDirection(moveDir.x);

            // Stop movement if player starts to spam right and left fast
            bool isMoveDirSame = moveDir.normalized.x > 0;
            if (!(isMovingRight == isMoveDirSame))
                break;

            Vector3 direction = target - transform.position;
            direction.z = 0;

            transform.position += direction.normalized * movementSpeed * Time.fixedDeltaTime;
            yield return null;
        }

        isMoving = false;
    }

    public void StopMovement()
    {
        StopCoroutine("MoveTo");
        isMoving = false;
    }
}
