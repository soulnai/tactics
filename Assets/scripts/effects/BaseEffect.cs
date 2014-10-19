using UnityEngine;
using System.Collections;
using EnumSpace;
using System;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class BaseEffect : ICloneable {

	public string effectName;
	public string effectID;
	public float effectApplyChance;
	public unitStates state;
	public int duration;
	public damageTypes damageType;
	public int damagePerTurn;
	public bool Stun = false;
	public bool canBeDebuffed = true;
	public bool infinite;

	//FX
	public GameObject FXprefab;

	private EffectsController controller;
	private Unit targetUnit;

	public void Activate(bool decreaseDuration = true)
	{
		if((duration > 0)||(infinite)){
			if(FXprefab != null)
				FXmanager.instance.createEffectFX(FXprefab,targetUnit);
			if((Stun)||(state == unitStates.stunned))
			{
				targetUnit.AP = 0;
				targetUnit.APmax = 0;
				Debug.Log("Unit stunned");
			}
			if(damagePerTurn > 0)
			{
				targetUnit.takeDamage(damagePerTurn);
				Debug.Log("Effect damage");
			}
			else if(damagePerTurn < 0)
			{
				targetUnit.takeHeal(damagePerTurn);
				Debug.Log("Effect heal");
			}
			if(decreaseDuration){
				duration--;
				if(duration <=0)
					controller.AddToDeleteList(this);
			}
		}
		else
		{
			controller.AddToDeleteList(this);
		}
	}

	public void Init(EffectsController c,Unit unit)
	{
		controller = c;
		targetUnit = unit;
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}

}
