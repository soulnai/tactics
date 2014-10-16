using UnityEngine;
using System.Collections;
using EnumSpace;

[System.Serializable]
public class BaseAbility {
	public string abilityID;
	public string abilityName;

	//Cost
	public int APcost;
	public int MPCost;

	public bool endsUnitTurn = true;
	public attackTypes attackType;
	public unitActions unitAction;

	//target flags
	public bool requireTarget = true;
	public bool allyUse = false;
	public bool selfUse = false;

	public int range;
	public bool areaDamage;
	public int areaDamageRadius;
	public areaPatterns areaPattern;

	public int baseDamage;
	public string effectToApply;
	public int effectDamage;
	public float effectApplyChance;
	public damageTypes damageType;
	public int duration;

	//FX
	public GameObject hitFXprefab;
	public GameObject rangedFXprefab;
}


