using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;
using System;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class BasePassiveAbility : ICloneable {
	public string abilityID;
	public string abilityName;

	public List<BaseAttributeChanger> affectedAttributes;

	public bool useRange = false;
	public int range;

	//target flags
	public bool requireTarget = true;
	public bool enemieUse = false;
	public bool allyUse = false;
	public bool selfUse = false;

	public Unit owner;
	public List<Unit> affectedUnits = new List<Unit>();

	public void AddUnit(Unit u)
	{
		if(!affectedUnits.Contains(u)){
			u.addPassiveAbility(this);
			affectedUnits.Add(u);
		}
		else
			Debug.Log("Unit already under effect");
	}

	public void RemoveUnit(Unit u)
	{
		if(affectedUnits.Contains(u)){
			u.removePassiveAbility(this);
			affectedUnits.Remove(u);
		}
		else
			Debug.Log("Unit is not under effect");
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
