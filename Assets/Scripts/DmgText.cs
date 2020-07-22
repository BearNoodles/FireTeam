using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DmgText : MonoBehaviour {

    [SerializeField]
    private Animator animator;
    private Text textComponent;
    private Outline textOutlineComponent;

    void OnEnable()
    {
        animator.enabled = true;

        textComponent = animator.GetComponent<Text>();
        textOutlineComponent = animator.GetComponent<Outline>();

        StartCoroutine("SetToDestroy");
    } 

    // Call clipinfo next frame as GetCurrentAnimatorClipInfo was returning nothing
    IEnumerator SetToDestroy()
    {
        yield return new WaitForEndOfFrame();
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length); // Destroy when finished TODO:- Implement Object pooling if time
    }

    public void SetText(string text, Color color)
    {
        textComponent.text = text;
        textComponent.color = color;
        textOutlineComponent.effectColor = color;
    }

    public void SetText(string text, Color color, Color outlineColor)
    {
        textComponent.text = text;
        textComponent.color = color;
        textOutlineComponent.effectColor = outlineColor;
    }
}
