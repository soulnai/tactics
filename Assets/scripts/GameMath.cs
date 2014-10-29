using UnityEngine;
using System.Collections;
using EnumSpace;

public static class GameMath {
	public static int calculateDamage (BaseAbility ability, Unit unitOwner, Unit _target ) {
		int amountOfDamage = 0;
		if (ability.attackType == attackTypes.magic || ability.attackType == attackTypes.heal) {
			amountOfDamage = (int)Mathf.Floor(Random.Range(unitOwner.damageBase, unitOwner.maxDamageBase+1.0f) +(unitOwner.Magic/2) - _target.MagicDef);
			if (Random.Range(0.0f, 1.0f) <= unitOwner.criticalChance){
				amountOfDamage+= (int)amountOfDamage*(int)unitOwner.criticalModifier;
			}
		} else {
			amountOfDamage = (int)Mathf.Floor(Random.Range(unitOwner.damageBase, unitOwner.maxDamageBase+1.0f) +(unitOwner.Strength/2) - _target.PhysicalDef);
			float angle = Vector3.Angle(unitOwner.transform.forward, _target.transform.forward);
			Debug.Log (angle);
			//TODO backstab apply chance
			if (angle <=30 && unitOwner.currentAbility.canBackstan && Random.Range(0.0f, 1.0f) <= unitOwner.Dexterity/100){
				amountOfDamage = amountOfDamage*10;
				Debug.Log ("backstab");
				return amountOfDamage;
			} else if (angle <=30 && unitOwner.currentAbility.canBackstan){
				amountOfDamage = amountOfDamage*5;
				Debug.Log ("backstab");
				return amountOfDamage;
			} else if (angle <=30){
				amountOfDamage = amountOfDamage*2;
				Debug.Log ("backstab");
				return amountOfDamage;
			}
			else if (angle >=30 && angle <=90){
				amountOfDamage = amountOfDamage*2;
				Debug.Log ("flank attack");
				return amountOfDamage;
			}
			else if (angle >90){
				if (Random.Range(0.0f, 1.0f) <= unitOwner.criticalChance){
					amountOfDamage = amountOfDamage*(int)unitOwner.criticalModifier;
				}
				Debug.Log ("front attack");
				return amountOfDamage;
			}
			
		}
		
		if (amountOfDamage <= 0) {
			amountOfDamage = 0;	
			return amountOfDamage;
		}
		return amountOfDamage;
	}

	//If use DamageType then use ResistTo
	public static bool ResistTo (Unit u,BaseAttributeChanger ac)
	{
		damageTypes damageType = ac.damageType;
		unitAttributes resistType = unitAttributes.strenght;
		
		switch (damageType) {
		case damageTypes.blunt:
			resistType = unitAttributes.strenght;
			break;
		case damageTypes.poison:
			resistType = unitAttributes.dexterity;
			break;
		case damageTypes.electricity:
			resistType = unitAttributes.magic;
			break;
		case damageTypes.fire:
			resistType = unitAttributes.magic;
			break;
		case damageTypes.ice:
			resistType = unitAttributes.magic;
			break;
		}
		
		//compare
		if (Random.Range(0.0f, 1.0f)> u.getAttribute(resistType).valueMod/100f)
			return true;
		else
			return false;
	}

	public static int ResistToDamage (Unit target, int damage, BaseAbility ability)
	{
		float defCoef = 0.1f;
		damageTypes damageType = ability.damageType;
		
		switch (damageType) {
		case damageTypes.blunt:
			damage -= Mathf.RoundToInt(damage * (target.getAttribute(unitAttributes.strenght).value*defCoef));
			break;
		case damageTypes.poison:
			damage -= Mathf.RoundToInt(damage * (target.getAttribute(unitAttributes.poisonDef).value*defCoef));
			break;
		case damageTypes.fire:
			damage -= Mathf.RoundToInt(damage * (target.getAttribute(unitAttributes.fireDef).value*defCoef));
			break;
		case damageTypes.ice:
			damage -= Mathf.RoundToInt(damage * (target.getAttribute(unitAttributes.iceDef).value*defCoef));
			break;
		}

		return Mathf.Clamp(damage,0,1000);
	}

	public static bool checkIfAttackSuccesfullyHit(Unit owner,Unit target){
		bool hit = false;
		float missChance = 1; 
		int rerollCount = 3;
		for(int i=0;i<rerollCount;i++)
		{
			float random = Random.Range(0.0f, 1.0f);
			if(random < missChance)
				missChance = random;
		}
		if (owner.currentAbility.attackType == attackTypes.melee) {
			hit = missChance <= owner.attackChance;
		} 
		if (owner.currentAbility.attackType == attackTypes.magic) {
			hit = missChance <= owner.magicAttackChance;
		} 
		if (owner.currentAbility.attackType == attackTypes.ranged) {
			hit = missChance <= owner.rangedAttackChance;
		} 
		if (Random.Range(0.0f, 1.0f) <= target.avoidChance){
			hit = false;
			GUImanager.instance.Log.addText("<b>"+target.unitName+":</b>" + " successfuly avoided - "+owner.currentAbility.abilityID + " of " + owner.unitName+"!");
		} 
		return hit;
	}
}
