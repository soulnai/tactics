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
				EventManager.UnitAttributeChanged(owner,this);
		}
	}

	public int mod{
		get{
			int _mod = 0;
			foreach(BaseAttributeChanger am in modList)
				_mod += am.getModValue(Value);
			return _mod;
		}
	}

	public int valueMod
	{
		get{
			return (Value+mod);
		}
	}

	public List<BaseAttributeChanger> modList;

	private Unit owner;

	public void addMod(BaseAttributeChanger ac)
	{
        if (!modList.Contains(ac))
            modList.Add(ac);
        else
            Debug.Log("attribute already contains this mod");
        if (owner != null)
            EventManager.UnitAttributeChanged(owner, this);
	}

	public void removeMod(BaseAttributeChanger ac)
	{

        if (modList.Contains(ac))
            modList.Remove(ac);
        else
            Debug.Log("No such mod on the attribute");		
        if(owner != null)
			EventManager.UnitAttributeChanged(owner,this);
	}

	public void clearAllMods()
	{
		modList.Clear();
		EventManager.UnitAttributeChanged(owner,this);
	}

	public void setOwner(Unit u)
	{
        EventManager.OnPlayerTurnStart += updateMods;
		if(u != null)
			owner = u;	
		else
			Debug.Log("Attribute doesnt have owner - " + this.attribute);
	}

    void updateMods(Player player)
    {
        if(modList.Count > 0){
            foreach(BaseAttributeChanger ac in modList) {
                ac.getModValue(Value);
            }
        }
    }

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
