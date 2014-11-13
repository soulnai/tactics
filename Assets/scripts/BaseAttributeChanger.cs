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
	public bool mod = false;
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
	[HideInInspector]
	public BaseEffect ownerEffect;

	public int getModValue(int attributeValue){
		int modValue = 0;
		if (multiply)
			modValue = UnityEngine.Mathf.RoundToInt ((attributeValue * value) - attributeValue);
		else
			modValue = UnityEngine.Mathf.RoundToInt (value);
		return modValue;
	}
    /// <summary>
    /// apply simple attribute changer value to attribute.
    /// Mod attribute changer is applied in Attribute itself.
    /// </summary>
    /// <param name="targetsList"></param>
    public void applyAttributeMod(List<Unit> targetsList)
    {
        if (mod)
        {
            foreach (Unit u in targetsList)
            {
                if (!u.getAttribute(attribute).modList.Contains(this))
                    u.getAttribute(attribute).addMod(this);
            }
        }
    }

    public void applyAttributeChanger(List<Unit> targetsList)
    {
        if ((applyEachTurn) && (!mod))
        {
            foreach (Unit u in targetsList)
            {
                if (u.playerOwner == GameManager.instance.currentPlayer)
                    u.getAttribute(attribute).Value += Mathf.RoundToInt(value);
            }
        }
    }

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}

