using UnityEngine;
using System.Collections;
using DG.Tweening;
using EnumSpace;

public class UserPlayer : Player {

	public UnitSkillsManager unitSkills;

	private Vector3 direction = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
		gameObject.AddComponent<BaseMelee>();
	}
	
	// Update is called once per frame
	public override void Update () {
		if (GameManager.instance.players[GameManager.instance.currentPlayerIndex] == this) {
			transform.renderer.material.color = Color.green;
		} else {
			transform.renderer.material.color = Color.white;
		}

		base.Update();
	}
	
	public override void TurnUpdate ()
	{
		if (positionQueue.Count > 0) {
			direction = (positionQueue[0] - transform.position).normalized;
			direction.y = 0;
//			Debug.DrawRay(positionQueue[0],direction*1000f,Color.red);

			transform.rotation = Quaternion.Lerp(transform.rotation,(Quaternion.LookRotation((direction).normalized)),0.1f);
			transform.position += (positionQueue[0] - transform.position).normalized * moveSpeed * Time.deltaTime;
			if (!animation.IsPlaying("Run")) {animation.CrossFade("Run", 0.2F);}
			if (Vector3.Distance(positionQueue[0], transform.position) <= 0.1f) {
				//transform.position = positionQueue[0];
				positionQueue.RemoveAt(0);
				if (positionQueue.Count == 0) {
					animation.Stop();
					animation.CrossFade("Idle", 0.2F);
					actionPoints--;
				}
			}		
		}
		
		base.TurnUpdate ();
	}

	public void Move ()
	{
		GameManager.instance.removeTileHighlights ();
		currentUnitAction = unitActions.moving;
		GameManager.instance.highlightTilesAt (gridPosition, Color.blue, movementPerActionPoint, false);
	}

	public void MagicAttack ()
	{
		GameManager.instance.removeTileHighlights ();
		if (unitSkills.skillsList.Contains ("baseMagic")&& currentUnitAction != unitActions.magicAttack) {
			attackDistance = AbilitiesManager.instance.getAbility("baseMagic").range;
			damageBase = AbilitiesManager.instance.getAbility("baseMagic").baseDamage;
			GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Fireball;
			GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.FireballExplode;

			currentUnitAction = unitActions.magicAttack;
<<<<<<< HEAD
			GameManager.instance.AtackhighlightTiles (gridPosition, Color.red, attackDistance, true);
		} else {
			GameManager.instance.removeTileHighlights ();
			currentUnitAction = unitActions.idle;
=======
			GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
>>>>>>> origin/Chewie
		}
	}

	public void MeleeAttack ()
	{
		GameManager.instance.removeTileHighlights ();
		if (unitSkills.skillsList.Contains ("baseMelee")&& currentUnitAction != unitActions.meleeAttack) {
			currentUnitAction = unitActions.meleeAttack;
			attackDistance = AbilitiesManager.instance.getAbility("baseMelee").range;
			damageBase = AbilitiesManager.instance.getAbility("baseMelee").baseDamage;
<<<<<<< HEAD
			GameManager.instance.AtackhighlightTiles (gridPosition, Color.red, attackDistance, true);
		}else {
			GameManager.instance.removeTileHighlights ();
			currentUnitAction = unitActions.idle;
=======
			GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
>>>>>>> origin/Chewie
		}
	}

	public void RangedAttack ()
	{
		GameManager.instance.removeTileHighlights ();
		if (unitSkills.skillsList.Contains ("baseRanged")&& currentUnitAction != unitActions.rangedAttack) {
			currentUnitAction = unitActions.rangedAttack;
			attackDistance = AbilitiesManager.instance.getAbility("baseRanged").range;
			damageBase = AbilitiesManager.instance.getAbility("baseRanged").baseDamage;
<<<<<<< HEAD
			GameManager.instance.AtackhighlightTiles (gridPosition, Color.red, attackDistance, true);
		} else {
			GameManager.instance.removeTileHighlights ();
			currentUnitAction = unitActions.idle;
=======
			GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
>>>>>>> origin/Chewie
		}
	}

	public void EndTurn ()
	{
		GameManager.instance.removeTileHighlights ();
		actionPoints = 2;
		currentUnitAction = unitActions.idle;
		GameManager.instance.nextTurn ();
	}

	public void StunAttack ()
	{
		GameManager.instance.removeTileHighlights ();
<<<<<<< HEAD
		if (unitSkills.skillsList.Contains ("baseStun") && currentUnitAction != unitActions.rangedAttack) {
						attackDistance = AbilitiesManager.instance.getAbility ("baseStun").range;
						damageBase = AbilitiesManager.instance.getAbility ("baseStun").baseDamage;
						GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Lightning;
						GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.LightningExplode;
						currentUnitAction = unitActions.rangedAttack;
						GameManager.instance.AtackhighlightTiles (gridPosition, Color.red, attackDistance, true);
				} else {
			GameManager.instance.removeTileHighlights ();
			currentUnitAction = unitActions.idle;
				}
=======
		if (unitSkills.skillsList.Contains ("baseStun")) {
			attackDistance = AbilitiesManager.instance.getAbility("baseStun").range;
			damageBase = AbilitiesManager.instance.getAbility("baseStun").baseDamage;
			GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Lightning;
			GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.LightningExplode;
			currentUnitAction = unitActions.rangedAttack;
			GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
	}
>>>>>>> origin/Chewie
	}
}
