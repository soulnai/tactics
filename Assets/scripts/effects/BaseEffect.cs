using UnityEngine;
using System.Collections;
using EnumSpace;

[System.Serializable]
public class BaseEffect {

	public string effectName;
	public string effectID;
	public unitStates state;
	public int duration;
	public damageTypes damageType;
	public int damagePerTurn;
	public bool Stun = false;
	public bool canBeDebuffed = true;
	public bool infinite;

	private Unit targetUnit;

	public void Activate()
	{
		if((duration > 0)||(infinite)){
			if((Stun)||(state == unitStates.stunned))
			{
				targetUnit.AP = 0;
			}
			if(damagePerTurn > 0)
			{
				targetUnit.takeDamage(damagePerTurn);
			}
			else if(damagePerTurn < 0)
			{
				targetUnit.takeHeal(damagePerTurn);
			}
			duration--;
		}
		else
		{

		}
	}

	public void setTarget(Unit unit)
	{
		targetUnit = unit;
	}
}
