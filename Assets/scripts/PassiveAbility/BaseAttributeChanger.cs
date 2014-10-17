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
	
	public object Clone()
	{
		return this.MemberwiseClone();
	}
}

