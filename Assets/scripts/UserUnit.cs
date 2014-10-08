using UnityEngine;
using System.Collections;
using DG.Tweening;
using EnumSpace;

public class UserUnit : Unit {

	public UnitSkillsManager unitSkills;

	// Use this for initialization
	void Start () {

	}



	public void MagicAttack ()
	{
		GameManager.instance.removeTileHighlights ();
		if(actionPoints > 0){
			if (unitSkills.abilitiesList.Contains ("baseMagic")) {
				attackDistance = AbilitiesManager.instance.getAbility("baseMagic").range;
				damageBase = AbilitiesManager.instance.getAbility("baseMagic").baseDamage;
				GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Freeze;
				GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.FreezeExplode;
				if (MP >= AbilitiesManager.instance.getAbility("baseMagic").MPCost) {
				UnitAction = unitActions.magicAttack;
				GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
				}
			}
		}
	}

	public void MeleeAttack ()
	{
		GameManager.instance.removeTileHighlights ();
		if(actionPoints > 0){
			if (unitSkills.abilitiesList.Contains ("baseMelee")) {
				UnitAction = unitActions.meleeAttack;
				attackDistance = AbilitiesManager.instance.getAbility("baseMelee").range;
				damageBase = AbilitiesManager.instance.getAbility("baseMelee").baseDamage;
				GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
			}
		}
	}

	public void RangedAttack ()
	{
		GameManager.instance.removeTileHighlights ();
		if(actionPoints > 0){
			if (unitSkills.abilitiesList.Contains ("baseRanged")) {
				attackDistance = AbilitiesManager.instance.getAbility("baseRanged").range;
				damageBase = AbilitiesManager.instance.getAbility("baseRanged").baseDamage;
				GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Lightning;
				GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.LightningExplode;

				UnitAction = unitActions.rangedAttack;
				GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
			}
		}
	}

	public void StunAttack ()
	{
		GameManager.instance.removeTileHighlights ();
		if(actionPoints > 0){
			if (unitSkills.abilitiesList.Contains ("baseStun")) {
				attackDistance = AbilitiesManager.instance.getAbility("baseStun").range;
				damageBase = AbilitiesManager.instance.getAbility("baseStun").baseDamage;
				GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Poison;
				GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.PoisonExplode;
				UnitAction = unitActions.rangedAttack;
				GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
			}
		}
	}
}
