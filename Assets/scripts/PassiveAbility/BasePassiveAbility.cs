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

	public List<BaseAttributeChanger> attributes;

	public bool useRange = false;
	public int range;

	//target flags
	public bool requireTarget = true;
	public bool allyUse = false;
	public bool selfUse = false;

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
