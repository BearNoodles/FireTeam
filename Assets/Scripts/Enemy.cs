using UnityEngine;
using System.Collections;

public class Enemy : Humanoid {

    public int FavourMove { get; private set; } // Used to decide to check clockwise or counter clock-wise first
    public string[] GermanNames;

	void Awake () {
        GermanNames = new string[] { "Müller", "Weber", "Richter", "Becker", "Koch", "Klien", "Schwarz",
                                    "Lange", "Fischer", "Krause", "Kung", "Hahn", "Schubert", "Ludwig",
                                    "Günther", "Vogel", "Jorg", "Wolfgang", "Lukas", "Hans" };

        base.GetMovableObjectRefs();
        base.ReloadSkills();
        base.LoadHealthBar();
        base.GetHumanoidReferences();

        FavourMove = (((UnityEngine.Random.value * 2) - 1) > 0) ? 1 : -1;
    }

	void Start()
	{
        if(CharData.type != UnitType.Boss)
		    CharData.name = GermanNames[Random.Range(0, GermanNames.Length - 1)];

        gameObject.name = CharData.name;
    }

    void KillMe()
    {
        base.anim.SetTrigger("IsDead");
        float timeBeforeKill = base.anim.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, timeBeforeKill);
    }
}
