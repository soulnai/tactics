using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;
using System;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class BaseAttributeChanger : ICloneable{
	public unitAttributes attribute;
	/// <summary>
	/// if true attribute*value , false +value to base attribute
	/// </summary>
	public bool multiply = false;
	public float value;

	// apply chance
	public bool useApplyChance = true;
	public float applyChance = 1;

	//if true will be incremented each turn, if false will be static bonus to parameter
	public bool applyEachTurn = false;

	//apply chance
	public bool useDamageTypes;
	public damageTypes damageType;

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}

