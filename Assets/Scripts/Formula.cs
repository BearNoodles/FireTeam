using UnityEngine;
using System.Collections;

public static class Formula{

	private static int maxHeal = 200;
	private static int maxDamage = 10000;
	private static int maxHealth = 10000;
	private static float maxLevel = 99;
	private static int explosionRadius = 75;
	
	public static float CalculateDamage(float attack)
	{
		float damageMultiplier = 1/(1+(Mathf.Exp(-5*((attack/maxLevel)-1))));
		float damage = maxDamage * damageMultiplier;
		damage += (Random.value * 0.15f) * damage;
		return (int)damage;
	}

	public static float CalculateExplosiveDamage(Vector3 targetPosition, Vector3 mousePosition, int attackPower)
	{
		float explosionMultiplier = 1 / (1 + (Mathf.Exp (-5 * ((attackPower / maxLevel) - 1))));
		targetPosition = new Vector3 (targetPosition.x - mousePosition.x, (targetPosition.y - mousePosition.y) * 1.3f, targetPosition.z);
		float distanceMultiplier = 1 - (((targetPosition).magnitude) / explosionRadius);
		float explosionPower = maxDamage * distanceMultiplier * explosionMultiplier * 1.5f;
		if (explosionPower < 0)
			explosionPower = 0;
		return (int)explosionPower;
	}

	public static int CalculateAttackPower(int level, UnitType type)
	{
		float attModifier = 1;
		switch (type) {
		case UnitType.Boss:
			attModifier = 1;
			break;
		case UnitType.Medic:
			attModifier = 0.5f;
			break;
		case UnitType.Rifleman:
			attModifier = 1.5f;
			break;
		case UnitType.Support:
			attModifier = 0.8f;
			break;
		}

		float attack = (level * attModifier) + 1;
		return (int)attack;
	}

	public static int CalculateHealPower(int level, UnitType type)
	{
		float healModifier = 1;
		switch (type) {
		case UnitType.Boss:
			healModifier = 0.4f;
			break;
		case UnitType.Medic:
			healModifier = 1.5f;
			break;
		case UnitType.Rifleman:
			healModifier = 0.02f;
			break;
		case UnitType.Support:
			healModifier = 0.1f;
			break;
		}
		
		float heal = (level * healModifier) + 1;
		return (int)heal;
	}

	public static int CalculateHealing(float heal)
	{
		float healingMultiplier = 1/(1+(Mathf.Exp(-5*((heal/maxHeal)-1))));
		float healing = maxDamage * healingMultiplier;
		return (int)healing;
	}
	
	public static float CalculateHitChance(float distance)
	{
		float hitChance = 1 - Mathf.Pow (((distance + 100) / 500), 2); 
		hitChance *= 100;
		hitChance = Mathf.Round(hitChance);
		
		if (hitChance <= 0)
			hitChance = 0;
		
		return hitChance;
	}

	public static int CalculateMaxHealth(int level, UnitType type)
	{
		float healthModifier = 1;
		switch (type)
		{
		    case UnitType.Boss:
				healthModifier = 1.5f;
				break;
			case UnitType.Medic:
				healthModifier = 1;
				break;
			case UnitType.Rifleman:
				healthModifier = 1.2f;
				break;
			case UnitType.Support:
				healthModifier = 1.8f;
				break;
		}

		float healthMultplier = (1 / (1 + (Mathf.Exp (-5 * ((level * 2 / maxLevel)-1)))));
		float health = (maxHealth * healthMultplier * healthModifier);
		return (int)health;
	}

	public static int ExpRequired(int level)
	{
		float exp = 50;
		for (int i = 0; i < level + 1; i++)
			exp *= 1.1f;
		return (int)exp;
	}

    public static int SpriteOrder(Vector3 pos)
    {
        float yPos = Camera.main.WorldToScreenPoint(pos).y;
        int maxSortingOrder = 600;

        return maxSortingOrder - (int)((yPos / Screen.height) * maxSortingOrder);
    }
}