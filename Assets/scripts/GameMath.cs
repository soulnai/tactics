﻿using UnityEngine;
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
}