using UnityEngine;
using System.Collections;
using EnumSpace;

[System.Serializable]
public class BaseAbility {
	public string attackID;
	public int APcost;
	public bool endsUnitTurn = true;
	public attackTypes attackType;
	public int range;
	public bool areaDamage;
	public areaPatterns areaPattern;

	public int baseDamage;
	public int effectDamage;
	public damageTypes damageType;
	public int duration;

	public int MPCost;
}


