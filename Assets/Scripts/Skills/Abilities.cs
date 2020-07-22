using UnityEngine;

public enum Abilities
{
    None,
    SingleHeal,
    MultiHeal,
	SuppressiveFire,
	DropBomb
}

public static class AbilityLoader
{
    public static IAbility GetByEnum(Abilities ability)
    {
        switch (ability)
        {
            case Abilities.SingleHeal:
                return new Heal(SkillType.Single);
            case Abilities.MultiHeal:
                return new Heal(SkillType.Multiple);
            case Abilities.SuppressiveFire:
                return new SuppressiveFire(SkillType.Single);
            case Abilities.DropBomb:
                return new DropBomb();
            default:
                return null; // No Ability Found
        }
    }
}

public struct TextColorPack
{
    public Color fontColor;
    public Color outlineColor;
}

public static class Skills
{
    public static bool IsInRange(this IAbility ability, float distance)
    {
        return (distance < ability.MaxRange) ? true : false;
    }

    public static TextColorPack CategoryColor(this IAbility ability)
    {
        TextColorPack skillColor;

        switch(ability.Category)
        {
            case SkillCategory.Attack:
                skillColor.fontColor = Color.red + (Color.white * 0.2f);
                skillColor.outlineColor = Color.yellow;
                break;
            case SkillCategory.Heal:
                skillColor.fontColor = Color.green + (Color.white * 0.2f);
                skillColor.outlineColor = Color.yellow;
                break;
            case SkillCategory.Support:
                skillColor.fontColor = Color.blue + (Color.white * 0.2f);
                skillColor.outlineColor = Color.black;
                break;
            default:
                skillColor.fontColor = Color.white;
                skillColor.outlineColor = Color.black;
                break;
        }

        return skillColor;
    }

    public static TextColorPack CategoryColor(this IAttack attack)
    {
        TextColorPack skillColor;

        switch (attack.Category)
        {
            case SkillCategory.Attack:
                skillColor.fontColor = Color.red + (Color.white * 0.2f);
                skillColor.outlineColor = Color.yellow;
                break;
            case SkillCategory.Heal:
                skillColor.fontColor = Color.green + (Color.white * 0.2f);
                skillColor.outlineColor = Color.yellow;
                break;
            case SkillCategory.Support:
                skillColor.fontColor = Color.blue + (Color.white * 0.2f);
                skillColor.outlineColor = Color.black;
                break;
            default:
                skillColor.fontColor = Color.white;
                skillColor.outlineColor = Color.black;
                break;
        }

        return skillColor;
    }
}


public interface IAbility
{
    SkillType Type { get; set; }
    SkillCategory Category { get; set; }
    int RecoveryCost { get; set; }
    int MaxRange { get; set; } // Note:- Only applicable to aoe attacks
    string Name { get; set; }

    void Perform(GameObject target, CharacterData attackerData, FloatingTextManager floatingTxtMgr);
    void Perform(GameObject[] targets, CharacterData attackerData, FloatingTextManager floatingTxtMgr);
}

public class Heal : IAbility
{
    public Heal(SkillType type)
    {
        Type = type;
        Category = SkillCategory.Heal;
        MaxRange = 150; // AOE Only

        if (type == SkillType.Single)
            Name = "Heal";
        else if (type == SkillType.Multiple)
            Name = "Multi-Heal";
    }

    public SkillType Type { get; set; }
    public SkillCategory Category { get; set; }
    public int RecoveryCost { get; set; }
    public int MaxRange { get; set; }
    public string Name { get; set; }

    public void Perform(GameObject target, CharacterData attackerData, FloatingTextManager floatingTxtMgr)
    {
        float healAmount = 3 * Formula.CalculateHealing(attackerData.healPower);

        // Apply dmg to target
        if (target.tag == "Player" || target.tag == "Enemy")
        {
            target.BroadcastMessage("ApplyHeal", healAmount);


            Vector3 textOffset = new Vector3(0, 30);
            floatingTxtMgr.CreateFloatingText(healAmount.ToString(), target.transform.position + textOffset, this.CategoryColor().fontColor, this.CategoryColor().outlineColor);
        }
    }

    public void Perform(GameObject[] targets, CharacterData attackerData, FloatingTextManager floatingTxtMgr)
    {
        // Apply heal to all targets
        foreach (GameObject t in targets)
        {
			float healAmount = Formula.CalculateExplosiveDamage(t.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), attackerData.healPower);
            if (t.tag == "Player" || t.tag == "Enemy")
            {
                t.BroadcastMessage("ApplyHeal", healAmount);

                Vector3 textOffset = new Vector3(0, 30);
                floatingTxtMgr.CreateFloatingText(healAmount.ToString(), t.transform.position + textOffset, this.CategoryColor().fontColor, this.CategoryColor().outlineColor);
            }
        }
    }
}

public class SuppressiveFire : IAbility
{
    public SuppressiveFire(SkillType type)
    {
        Type = type;
        Category = SkillCategory.Support;
        Name = "Suppressive Fire";
    }

    public SkillType Type { get; set; }
    public SkillCategory Category { get; set; }
    public int RecoveryCost { get; set; }
    public int MaxRange { get; set; }
    public string Name { get; set; }

    public void Perform(GameObject target, CharacterData attackerData, FloatingTextManager floatingTxtMgr)
    {
        float suppressAmount = 5 + (attackerData.level / 10);
        BattleManager.ActiveCharacter.PlayShootAnim();

        // Apply dmg to target
        if (target.tag == "Player" || target.tag == "Enemy")
        {
            target.BroadcastMessage("Suppress", suppressAmount);

            Debug.Log("Suppress fire fired");
            Vector3 textOffset = new Vector3(0, 30);
            floatingTxtMgr.CreateFloatingText("Recovery +" + suppressAmount.ToString(), target.transform.position + textOffset, this.CategoryColor().fontColor, this.CategoryColor().outlineColor);
        }
    }

    public void Perform(GameObject[] targets, CharacterData attackerData, FloatingTextManager floatingTxtMgr)
    {
        // TOO OVERPOWERED TO PERFORM ON MULTIPLE TARGETS
    }
}

public class DropBomb : IAbility
{
    public DropBomb()
    {
        Type = SkillType.Multiple;
        Category = SkillCategory.Attack;
        RecoveryCost = 5;
        MaxRange = 150; // AOE Only
        Name = "Grenade";
    }

    public SkillType Type { get; set; }
    public SkillCategory Category { get; set; }
    public int RecoveryCost { get; set; }
    public int MaxRange { get; set; }
    public string Name { get; set; }

    // Perform this attack on a single target
    public void Perform(GameObject target, CharacterData attackerData, FloatingTextManager floatingTxtMgr)
    {
        // Do nothing
    }

    // Perform this attack on multiple targets
    public void Perform(GameObject[] area, CharacterData attackerData, FloatingTextManager floatingTxtMgr)
    {
        foreach (GameObject t in area)
        {
			float dmgAmount = Formula.CalculateExplosiveDamage(t.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), attackerData.attackPower);
            if (t.tag == "Player" || t.tag == "Enemy" || t.tag == "Trap" || t.tag == "Collectable")
            {
                t.BroadcastMessage("ApplyDamage", dmgAmount);

                Vector3 textOffset = new Vector3(0, 30);
                floatingTxtMgr.CreateFloatingText(dmgAmount.ToString(), t.transform.position + textOffset, this.CategoryColor().fontColor, this.CategoryColor().outlineColor);
                DmgEffectManager.CreateHit(t.transform.position);
            }
        }
    }
}


