using UnityEngine;
using System.Collections;

public class Player : Humanoid {  
    
    void Awake()
    {
        base.GetMovableObjectRefs();
        base.ReloadSkills();
        base.LoadHealthBar();
        base.GetHumanoidReferences();
    }  

    void KillMe()
    {
        base.anim.SetTrigger("IsDead");
        float timeBeforeKill = base.anim.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log("Time before death: " + timeBeforeKill);
        Destroy(gameObject, timeBeforeKill);
    }
}
