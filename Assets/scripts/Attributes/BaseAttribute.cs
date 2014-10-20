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
	public int value;

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
			return (value+mod);
		}
	}

	public List<int> modList;

	public void addMod(int val)
	{
		modList.Add(val);
	}

	public void clearMods()
	{
		modList.Clear();
	}



	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
