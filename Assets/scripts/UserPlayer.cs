using UnityEngine;
using System.Collections;
using DG.Tweening;
using EnumSpace;

public class UserPlayer : Player {

	private Vector3 direction = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
		gameObject.AddComponent<BaseMelee>();
		//skills.SetValue (new BaseMelee(),0);
		//skills [0] = new BaseMelee();
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

	//Draw GUI
	/**
	public override void TurnOnGUI () {
		float buttonHeight = 50;
		float buttonWidth = 150;
		
		Rect buttonRect = new Rect(0, Screen.height - buttonHeight * 3, buttonWidth, buttonHeight);
		
		
		//move button
		if (GUI.Button(buttonRect, "Move")) {
			Move ();
		}

		buttonRect = new Rect(0, Screen.height - buttonHeight * 4, buttonWidth, buttonHeight);
		
		
		//move button
		if (GUI.Button(buttonRect, "Attack Fireball")) {

			MagicAttack ();

		}
		
		//attack button
		buttonRect = new Rect(0, Screen.height - buttonHeight * 2, buttonWidth, buttonHeight);
		
		if (GUI.Button(buttonRect, "Attack")) {
			Attack ();

		}
		
		//end turn button
		buttonRect = new Rect(0, Screen.height - buttonHeight * 1, buttonWidth, buttonHeight);		
		
		if (GUI.Button(buttonRect, "End Turn")) {
			EndTurn ();
		}

		buttonRect = new Rect(buttonWidth, Screen.height - buttonHeight * 1, buttonWidth, buttonHeight);		

		if (GUI.Button(buttonRect, "Unit Center")) {
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset = Vector3.zero;
			Camera.main.GetComponent<CameraOrbit>().pivot = transform;
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
		}

		buttonRect = new Rect(buttonWidth, Screen.height - buttonHeight * 2, buttonWidth, buttonHeight);
		if (GUI.Button(buttonRect, "Stun Attack")) {

			StunAttack ();
		}

		base.TurnOnGUI ();
	}
	
	**/

	public void Move ()
	{
		if (currentUnitAction != unitActions.moving) {
			GameManager.instance.removeTileHighlights ();
			currentUnitAction = unitActions.moving;
			GameManager.instance.highlightTilesAt (gridPosition, Color.blue, movementPerActionPoint, false);
		}
		else {
			currentUnitAction = unitActions.idle;
			GameManager.instance.removeTileHighlights ();
		}
	}

	public void MagicAttack ()
	{
		if (GetComponent<SkillsSample> ().skillsList.Contains (SkillsSample.skills.fireball)) {
			attackDistance = GetComponent<FireballSkill> ().fireballBaseRange;
			damageBase = GetComponent<FireballSkill> ().fireballBaseDamage;
			burnTimerDuration = GetComponent<FireballSkill> ().fireballBaseDuration;
			burnDamage = true;
			poisonDamage = false;
			stunDamage = false;
			freezeDamage = false;
			GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Fireball;
			GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.FireballExplode;
		}
		if ((currentUnitAction != unitActions.magicAttack)) {
			GameManager.instance.removeTileHighlights ();
			currentUnitAction = unitActions.magicAttack;
			GameManager.instance.AtackhighlightTiles (gridPosition, Color.red, attackDistance, true);
		}
		else {
			burnDamage = false;
			currentUnitAction = unitActions.idle;
			GameManager.instance.removeTileHighlights ();
		}
	}

	public void Attack ()
	{
		burnDamage = false;
		poisonDamage = false;
		stunDamage = false;
		freezeDamage = false;
		if ((currentUnitAction != unitActions.meleeAttack)) {
			GameManager.instance.removeTileHighlights ();
			currentUnitAction = unitActions.meleeAttack;
			GameManager.instance.AtackhighlightTiles (gridPosition, Color.red, attackRange, true);
		}
		else {
			currentUnitAction = unitActions.idle;
			GameManager.instance.removeTileHighlights ();
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
		if (GetComponent<SkillsSample> ().skillsList.Contains (SkillsSample.skills.stun)) {
			attackDistance = GetComponent<StunSkill> ().stunBaseRange;
			damageBase = GetComponent<StunSkill> ().stunBaseDamage;
			stunTimerDuration = GetComponent<StunSkill> ().stunBaseDuration;
			stunDamage = true;
			GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Lightning;
			GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.LightningExplode;
		}
		if ((currentUnitAction != unitActions.rangedAttack)) {
			GameManager.instance.removeTileHighlights ();
			currentUnitAction = unitActions.rangedAttack;
			GameManager.instance.AtackhighlightTiles (gridPosition, Color.red, attackDistance, true);
		}
		else {
			stunDamage = false;
			currentUnitAction = unitActions.idle;
			GameManager.instance.removeTileHighlights ();
		}
	}
}
