using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public enum UnitType
{
	None,
    Rifleman,
    Medic,
    Support,

    Boss
}

public enum SoundStatus
{
	Hit,
	Miss,
	Critical,
	Ability,
	Walk,
	None
}

public class Humanoid : MovableObject
{
    public void ReloadSkills()
    {
        // Load Ability or default ability if none specified
        Ability = (charData.abilityToUse == Abilities.None) ? new Heal(SkillType.Single) : AbilityLoader.GetByEnum(charData.abilityToUse);

        // Load Attack or default attack if none specified
        Attack = (charData.attackToUse == Attacks.None) ? new Peow() : AttackLoader.GetByEnum(charData.attackToUse);
    }

    protected void GetHumanoidReferences()
    {
        Shadow = transform.Find("Shadow").GetComponent<SpriteRenderer>();
    }

	public SoundStatus soundStatus = SoundStatus.None;
    public Text hitChanceText { get; set; }
    private SpriteRenderer Shadow { get; set; }
    public Color ShadowColour { get { return Shadow.color; } set { Shadow.color = value; } }

    // Holds information about Health, Ability (loaded from GameManager's playerProfile member), Stats (Create a struct to hold these), etc
    [SerializeField]
    private CharacterData charData;
    public Sprite portrait;
	public int turn;
	public const int SPEED = 10; //just put this here for end turn recovery for now

    public CharacterData CharData { get { return charData; } set { charData = value; } }
    public IAttack Attack { get; set; }
    public IAbility Ability { get; set; }
    public bool IsDead { get { return CharData.health <= 0; } }
	public int Recovery;
	public int MaxHealth { get { return Formula.CalculateMaxHealth(charData.level, charData.type) + charData.addedHealth; } }

	public int expTarget = 50;
    public Vector3 Position { get { return transform.position; } }
	public bool destroyMeNow;
	public bool isEnemy;

    public void Update()
    {
		base.UpdateMovableObj();
        CapHealth();
		UpdateStats();

        if (IsDead)
        {
            base.anim.SetTrigger("IsDead");
        }

        base.UpdateSortingOrder(transform.position);
		hatRend.sprite = charData.GetCurrentHat(isEnemy);
    }

    private void CapHealth()
    {
        if (CharData.health > (int)MaxHealth)
        {
            CharData.health = (int)MaxHealth;
        }
    }

    public void ApplyDamage(float dmg)
    {
        CharData.health -= (int)Mathf.Floor(dmg);

        if (IsDead)
        {
            CharData.health = 0;
            anim.SetTrigger("IsHurt");
            StopCoroutine("MoveTo");
        }
        else
            anim.SetTrigger("IsHurt");
    }

    public void ApplyRecovery(int recovery)
    {
        Recovery += recovery;
    }

    public void ApplyHeal(float heal)
    {
        CharData.health += (int)Mathf.Floor(heal);

        CapHealth();
    }

    public void PlayShootAnim()
    {
        base.anim.SetTrigger("IsShooting");
        Debug.Log("Bang");
    }

    // Sets their turn lower
    public void Suppress(int amount)
    {
        Recovery += amount;
    }

	private void UpdateStats()
	{
		charData.attackPower = Formula.CalculateAttackPower(charData.level, charData.type) + charData.addedAttack;
		charData.healPower = Formula.CalculateHealPower (charData.level, charData.type);
	}

    protected void LoadHealthBar()
    {
        HealthBar healthBarPref = (HealthBar)Resources.Load("Prefabs/HealthBar", typeof(HealthBar));

        HealthBar healthBar = (HealthBar)Instantiate<HealthBar>(healthBarPref);
        healthBar.SetHumanParent(gameObject);
        healthBar.Offset = new Vector3(0, 55, 0);

        healthBar.transform.SetParent(GameObject.Find("HUD/HealthBars").transform, false);
    }

	public static string GetClassColourHex(UnitType type)
	{
		Color unitColour;

		switch (type) 
		{
		    case UnitType.Medic:
			    unitColour = new Color((float)28 / 255, (float)91 / 255, (float)170 / 255); ;
			    break;
		    case UnitType.Rifleman:
			    unitColour = new Color((float)162 /255, (float)103 /255, (float)24 /255);
			    break;
		    case UnitType.Support:
			    unitColour = new Color((float)35/255, (float)115 /255, (float)83 /255);
			    break;
            case UnitType.Boss:
                unitColour = Color.red;
                break;
		default:
			unitColour = Color.black;
			break;
		}

		return String.Format("#{0:x2}{1:x2}{2:x2}", ToByte(unitColour.r), ToByte(unitColour.g), ToByte(unitColour.b));
	}

	private static byte ToByte(float value)
	{
		value = Mathf.Clamp01 (value);
		return (byte)(value * 255);
	}
}

[Serializable]
public class CharacterData
{
	private int currentClass;

    public string name, originalName;
    public UnitType type;
    public Abilities abilityToUse;
    public Attacks attackToUse;
	public int id, health, attackPower,
		healPower, experience, level;
	public int addedAttack, addedHealth;

	public int
		currentWeapon,
		currentArmour,
		currentHat;

	public void Initialize()
	{
		currentClass = UnityEngine.Random.Range(0, 3);
		GenerateName();
		UpdateValues();
	}

	public void Initialize(UnitType type)
	{
		currentClass = 
				type == UnitType.Rifleman ? 0 :
				type == UnitType.Medic ? 1 :
				type == UnitType.Support ? 2 : 0;
		UpdateValues();
	}

	void UpdateValues()
	{
		type =  currentClass == 0 ? UnitType.Rifleman :
				currentClass == 1 ? UnitType.Medic :
				currentClass == 2 ? UnitType.Support :
				UnitType.None;
		
		attackToUse = 
				type == UnitType.Rifleman ? Attacks.Peow :
				type == UnitType.Medic ? Attacks.None :
				type == UnitType.Support ? Attacks.Peow :
				Attacks.None;
		
		abilityToUse = 
				type == UnitType.Rifleman ? Abilities.DropBomb :
				type == UnitType.Medic ? Abilities.SingleHeal :
				type == UnitType.Support ? Abilities.SuppressiveFire :
				Abilities.None;
	}

	public void GenerateName()
	{
		string[] NAMES = {
			"Justin", "Peter", "John", "Craig", "Brian",
			"Chris", "Thomas", "Patrick", "Jack", "Louis",
			"Dennis", "Jason", "Scott", "Billy", "Eric",
			"Sean", "James", "Steve", "Daryl", "Dillon" };

		name = NAMES[UnityEngine.Random.Range(0, NAMES.Length)];
	}

	public Sprite GetCurrentHat(bool isEnemy)
	{
		int current = GameManager.instance.Profile.ownedHats[currentHat].id;
		Sprite[] hats = Resources.LoadAll<Sprite>("Sprites/Hats") as Sprite[];

		Sprite sprite = 
				isEnemy ? hats[0] :
				current == 1 ? hats[3] :
				current == 2 ? hats[2] :
				current == 3 ? hats[4] :
				current == 4 ? hats[1] :
				null;

		return sprite;
	}

	public void SwitchClass()
	{
		if (currentClass < 2) currentClass++;
		else currentClass = 0;
		
		UpdateValues();
	}
}
