using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

[System.Serializable]
public class BaseAbility : ICloneable{
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

	public bool enemieUse = false;
	public bool allyUse = false;
	public bool selfUse = false;

	public int range;
	public bool areaDamage;
	public int areaDamageRadius;
	public areaPatterns areaPattern;

	public int baseDamage;
	public bool canBackstan = false;
	public damageTypes damageType;
	
	public List<string> effects;

	//FX
	public GameObject hitFXprefab;
	public GameObject rangedFXprefab;

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}


