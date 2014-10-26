using UnityEngine;
using System.Collections;
using EnumSpace;

public static class GameMath {
	public static int calculateDamage (BaseAbility ability, Unit unitOwner, Unit _target ) {
		int amountOfDamage = 0;
		if (ability.attackType == attackTypes.magic || ability.attackType == attackTypes.heal) {
			amountOfDamage = (int)Mathf.Floor(Random.Range(unitOwner.damageBase, unitOwner.maxdamageBase+1.0f) +(unitOwner.Magic/2) - _target.MagicDef);
			if (Random.Range(0.0f, 1.0f) <= unitOwner.criticalChance){
				amountOfDamage+= (int)amountOfDamage*(int)unitOwner.criticalModifier;
			}
			//return amountOfDamage;
		} else {
			amountOfDamage = (int)Mathf.Floor(Random.Range(unitOwner.damageBase, unitOwner.maxdamageBase+1.0f) +(unitOwner.Strength/2) - _target.PhysicalDef);
			float angle = Vector3.Angle(GameManager.instance.currentUnit.transform.forward, GameManager.instance.targetPub.transform.forward);
			Debug.Log (angle);
			//TODO backstab apply chance
			if (angle <=30 && unitOwner.currentAbility.canBackstan && Random.Range(0.0f, 1.0f) <= 1f){
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
					amountOfDamage+= amountOfDamage*(int)unitOwner.criticalModifier;
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
}
