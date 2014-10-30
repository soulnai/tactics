using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;
using System;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class BaseAttribute : ICloneable {
	public string name;
	public unitAttributes attribute;
	public int _value;
	public int Value{
		get{return _value;}
		set{
			_value = value;
			if(owner != null)
				UnitEvents.UnitAttributeChanged(owner,this);
		}
	}

	public int mod{
		get{
			int _mod = 0;
			foreach(int i in modList)
				_mod += i;
			return _mod;
		}
	}

	public int valueMod
	{
		get{
			return (Value+mod);
		}
	}

	public List<int> modList;

	private Unit owner;

	public void addMod(int val)
	{
		modList.Add(val);
	}

	public void clearMods()
	{
		modList.Clear();
	}

	public void setOwner(Unit u)
	{
		if(u != null)
			owner = u;	
		else
			Debug.Log("Attribute doesnt have owner - " + this.attribute);
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
