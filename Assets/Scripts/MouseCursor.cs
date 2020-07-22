using UnityEngine;
using System.Collections;

public enum CursorsType
{
    Default,
    Hide,
    Wait,
    Skill
}

public class MouseCursor : MonoBehaviour {

    // Change in Inspector
    public Texture2D noneTxr;
    public Texture2D waitTxr;
    public Texture2D skillTxr;

    private PlayerController playerController;

    private CursorsType currentCursor;
    public CursorsType CurrentCursor
    {
        get
        {
            return currentCursor;
        }

        set
        {
            ChangeCursor(value);

            currentCursor = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        CurrentCursor = CursorsType.Default;
        playerController = BattleManager.GetPlayerController;
    }

    private void ChangeCursor(CursorsType newcursor)
    {
        if (newcursor == CursorsType.Hide)
            Cursor.visible = false;
        else
            Cursor.visible = true;
        
        switch(newcursor)
        {
            case CursorsType.Default:
                Vector2 offset = new Vector2(20, 0);
                Cursor.SetCursor(noneTxr, offset, CursorMode.Auto);
                break;
            case CursorsType.Wait:
                Cursor.SetCursor(waitTxr, Vector2.zero, CursorMode.Auto);
                break;
            case CursorsType.Skill:
                Cursor.SetCursor(skillTxr, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
	    // Update Mouse Depending on match status
        if(playerController.CurrentState == State.Enabled)
        {
            //playerController.
        }
	}
}
