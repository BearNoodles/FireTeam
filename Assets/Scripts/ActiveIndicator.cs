using UnityEngine;
using System.Collections;

public class ActiveIndicator : MonoBehaviour {

    public Transform ActiveTarget { get; set; }
    public bool IsActive { get; set; }
    private Vector3 offset;
	
    void Start()
    {
        // Set position in relation to the target
        offset = new Vector3(0, 65, 0);
    }

	// Update is called once per frame
	void Update () {
        // If active player is alive then position indicator over them
        if (ActiveTarget != null && IsActive)
            transform.position = ActiveTarget.position + offset;
        
        if(BattleManager.CurrentBattleState == BattleState.GameOver)
            gameObject.SetActive(false);
	}

    public void SetTarget(Transform target)
    {
        ActiveTarget = target;
        IsActive = true;
    }
}
