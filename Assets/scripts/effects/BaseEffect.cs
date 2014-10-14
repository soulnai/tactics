using UnityEngine;
using System.Collections;
using EnumSpace;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class BaseEffect {

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

	private EffectsController controller;
	private Unit targetUnit;

	public void Activate()
	{
		if((duration > 0)||(infinite)){
			if((Stun)||(state == unitStates.stunned))
			{
				targetUnit.AP = 0;
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
			duration--;
		}
		else
		{
			controller.DeleteEffect(this);
		}
	}

	public void Init(EffectsController c,Unit unit)
	{
		controller = c;
		targetUnit = unit;
	}

	public BaseEffect getEffect()
	{
		return this;
	}
}
