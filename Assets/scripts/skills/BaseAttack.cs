using UnityEngine;
using System.Collections;
using EnumSpace;

[System.Serializable]
public class BaseAttack {
	public string attackID;
	public attackTypes attackType;
	public int range;
	public bool areaDamage;
	public areaPatterns areaPattern;

	public int baseDamage;
	public int effectDamage;
	public damageTypes damageType;
	public int duration;
}


