using System.Collections;
using UnityEngine;

/* Note from Stephen
    Added an enum so that we can change attacks through editor.
    We will most likly be loading enemies and players from script. (loaders could still be used however)
    It would be easy to implement a method inside humanoid for loading abilities directly by new ClassName().
    So we can change it later.

    Adding an Attack
    1. Add class that inherits from IAttack and has a 'Use' method (We can rename later, powering through this atm) 
    2. Add Attack name to end of enum 
    3. Add case statment to AttackLoader returning a new instance of the Attack class i.e. return new FireBall();
    4. Profit
*/

public enum Attacks
{
    None,
    Peow
}

public enum SkillType
{
    None,
    Single,
    Multiple,
    Passive
}

public enum SkillCategory
{
    Attack,
    Support,
    Heal
}

public static class AttackLoader
{
    public static IAttack GetByEnum(Attacks attacks)
    {
        switch (attacks)
        {
            case Attacks.Peow:
                return new Peow();
            default:
                return null; // No Attack was found
        }
    }
}

public interface IAttack
{
    SkillType Type { get; set; }
    SkillCategory Category { get; set; }
    int RecoveryCost { get; set; }
    string Name { get;  set; }

    void Perform(GameObject target, CharacterData attackerData, FloatingTextManager floatingTxtMgr);
    void Perform(GameObject[] targets, CharacterData attackerData, FloatingTextManager floatingTxtMgr);
}

public class Peow : IAttack
{
    public Peow()
    {
        Type = SkillType.Single;
        Category = SkillCategory.Attack;
        RecoveryCost = 3;
        Name = "Peow";
    }

    public SkillType Type { get; set; }
    public SkillCategory Category { get; set; }
    public int RecoveryCost { get; set; }
    public string Name { get; set; }

    // Perform this attack on a single target
    public void Perform(GameObject target, CharacterData attackerData, FloatingTextManager floatingTxtMgr)
    {
        float dmgAmount = Formula.CalculateDamage(attackerData.attackPower);

        // Apply dmg to target
        if (target.tag == "Player" || target.tag == "Enemy" || target.tag == "Trap" || target.tag == "Collectable")
        {
            target.BroadcastMessage("ApplyDamage", dmgAmount);

            Vector3 textOffset = new Vector3(0, 30);
            floatingTxtMgr.CreateFloatingText(dmgAmount.ToString(), target.transform.position + textOffset, Color.red, Color.yellow);
            DmgEffectManager.CreateHit(target.transform.position);
        }
    }

    // Perform this attack on multiple targets
    public void Perform(GameObject[] targets, CharacterData attackerData, FloatingTextManager floatingTxtMgr)
    {
        // TODO:- Add logic to this too?
    }
}