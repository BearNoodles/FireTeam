using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HitChance : MonoBehaviour {

    Animator anim;
    int enterAnimHash;

    Image outerCircle;
    [SerializeField]
    Image innerCircle;
    [SerializeField]
    Text text;

    
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        enterAnimHash = Animator.StringToHash("IsAnimating");
        outerCircle = GetComponent<Image>();
    }
	
    void OnEnable()
    {
        if (anim != null)
        {
            anim.ResetTrigger(enterAnimHash);
            anim.SetTrigger(enterAnimHash);
        }

        innerCircle.color = text.color;
    }

    public static Color GetColour(float hitchance)
    {
        Color hitchanceColor = Color.black;

        if (hitchance > 60)
            hitchanceColor = Color.green + (Color.white * 0.3f);
        else if (hitchance > 20)
            hitchanceColor = Color.red + Color.yellow + (Color.white * 0.3f);
        else if (hitchance >= 0)
            hitchanceColor = Color.red + (Color.white * 0.3f);

        return hitchanceColor;
    }
}
