using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SlotType
{
	Active,
	Add,
	Locked
}

public class TeamSlot : MonoBehaviour
{
	public int id, charId;

	private SlotType type;
	private CharacterData chara;
	private bool current;
	private Sprite sprite,
	down_switch, down_delete;

	public SlotType Type
	{
		get { return type; }
	}

	public string Name
	{
		get { return chara.name; }
	}

	void Awake()
	{
		down_switch = Resources.Load<Sprite>("Sprites/overlay_switch") as Sprite;
		down_delete = Resources.Load<Sprite>("Sprites/overlay_delete") as Sprite;
	}

	// Update is called once per frame
	void Update()
	{
		sprite = (type == SlotType.Active) ? 
			Resources.Load<Sprite>("Sprites/portrait_" + chara.type.ToString().ToLower()) as Sprite : null;

		if (type == SlotType.Active)
		transform.Find("Back").GetComponent<Image>().color = 
			chara.type == UnitType.Rifleman ? new Color(0, 1, 0, 0.2f) :
			chara.type == UnitType.Medic ? new Color(1, 0, 0, 0.2f) :
			chara.type == UnitType.Support ? new Color(0, 0, 1, 0.2f) :
			Color.black;

		transform.Find("Class").GetComponent<Image>().sprite = sprite;
		transform.Find("Class").gameObject.SetActive(type == SlotType.Active);
		transform.Find("Overlay").GetComponent<Image>().sprite = 
			(type == SlotType.Active && Input.GetKey(KeyCode.LeftShift)) ? down_switch :
			(type == SlotType.Active && Input.GetKey(KeyCode.LeftControl) && !current) ? down_delete :
			Resources.Load<Sprite>("Sprites/overlay_" + type.ToString().ToLower()) as Sprite;

		transform.Find("Active").gameObject.SetActive(current);
		gameObject.GetComponent<Button>().interactable = !(type == SlotType.Locked);
	}

	public void SetValues(int i, SlotType t)
	{
		id = i;
		type = t;
		charId = -1;
	}

	public void SetValues(int i, SlotType t, CharacterData c)
	{
		id = i;
		type = t;
		chara = c;
		charId = c.id;
	}

	public void SetCurrent(bool val)
	{
		current = val;
	}
}
