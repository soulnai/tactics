using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

[System.Serializable]
public class Unit : MonoBehaviour {

	public bool isFlying = false;

	public Vector2 gridPosition = Vector2.zero;
	
	public Vector3 moveDestination;
	public float moveSpeed = 5.0f;
	
	public int movementPerActionPoint = 5;
	public int attackRange = 1;
	public int attackDistance = 5;
	
	public string unitName = "George";
	public int MaxHP = 25;
	public int HP = 25;
	public int AP
	{
		get {return _AP;}
		set {
			_AP = value;
			if(AP <= 0)
			{
				//send event no AP
			}
		}
	}
	public int MP = 25;
	public int Strength = 2;
	public int Magic = 2;
	public int PhysicalDefense = 2;
	public int MagicDefense = 2;
	
	public float attackChance = 0.75f;
	public float avoidChance = 0.15f;
	public float criticalChance = 0.1f;
	public float criticalModifier = 1.5f;
	public int damageBase = 1;
	public int maxdamageBase = 5;
	public float damageRollSides = 6; //d6
	
	public int maxActionPoints = 2;


	public EnumSpace.unitStates UnitState;
	public EnumSpace.unitActions UnitAction;
	public Tile currentTile;
	public BaseAbility currentAbility;
	public Unit currentTarget;

	//movement animation
	public List<Vector3> positionQueue = new List<Vector3>();	
	//
	public UnitSkillsManager unitAbilities;

	private int _AP;
	private Vector3 lookDirection = Vector3.zero;
	private float delayAfterAnim = 0.5f;
	void Awake () {
		moveDestination = transform.position;
		AP = maxActionPoints;

	}

	public void ReactionsEnd (Unit unit)
	{
		UnitEvents.onUnitReactionEnd -= ReactionsEnd;
		if(AP <= 0)
			EndTurn();
		Debug.Log("This - "+this.unitName+" // target - " +unit.unitName+"Reaction End");
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	public virtual void Update () {		
		if (UnitState == unitStates.dead&&(GameManager.instance.units[GameManager.instance.currentUnitIndex] == this)) {
			EndTurn();
		}
		if(UnitAction == unitActions.moving)
		{
			MoveUnit();
		}
	}

	public void onAbility(BaseAbility a)
	{
		GameManager.instance.removeTileHighlights ();

		if (unitAbilities.abilities.Contains(a)) {
			if((AP > 0)&&(MP >= a.MPCost)){
				currentAbility = a;
				attackDistance = a.range;
				damageBase = a.baseDamage;
				//magic
				if(a.attackType == attackTypes.magic){
					UnitAction = unitActions.magicAttack;
					GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Freeze;
					GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.FreezeExplode;
				}
				//ranged
				else if(a.attackType == attackTypes.ranged){
					UnitAction = unitActions.rangedAttack;
					GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Lightning;
					GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.LightningExplode;
				}
				//melee
				else if(a.attackType == attackTypes.melee){
					UnitAction = unitActions.meleeAttack;
				}
				//stun
				else if(a.attackType == attackTypes.ranged){
					UnitAction = unitActions.rangedAttack;
					GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Poison;
					GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.PoisonExplode;
				}
				else if(a.attackType == attackTypes.heal){
					UnitAction = unitActions.healAttack;
					GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Heal;
					GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.HealExplode;
				}

			GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
			}
			else
			{	if(AP <= 0){
					GUImanager.instance.Log.addText("Not enought <b><color=red>AP</color></b>");
				} else if(MP < a.MPCost){
					GUImanager.instance.Log.addText("Not enought <b><color=blue>MP</color></b>");
				}
			}
		}
		else
		{
			Debug.Log("No such ability - " + a.abilityID);
		}
	}

	public void Ability(BaseAbility a,Unit target)
	{
		UnitEvents.onUnitReactionEnd += ReactionsEnd;

		if(a.endsUnitTurn){
			AP = 0;
		}
		else
		{
			AP -= a.APcost;
		}

		animation.Play("Attack");
		if((a.attackType != attackTypes.melee)&&(a.rangedFXprefab != null))
		{
			FXmanager.instance.createFX(a.rangedFXprefab,this.transform.position,target.transform.position,currentAbility);
		}
		if((a.attackType == attackTypes.melee)&&(a.hitFXprefab != null))
		{
			FXmanager.instance.createFX(a.hitFXprefab,target.transform.position,target.transform.position,currentAbility);
		}
		
		StartCoroutine(WaitAnimationEnd(animation["Attack"].length));
	}

	public void tryMove ()
	{
		GameManager.instance.removeTileHighlights ();
		if(AP > 0){
			UnitAction = unitActions.readyToMove;
			GameManager.instance.highlightTilesAt (gridPosition, Color.blue, movementPerActionPoint, false);
		}
	}

	public void MoveUnit()
	{
		if (positionQueue.Count > 0) {
			lookDirection = (positionQueue[0] - transform.position).normalized;
			lookDirection.y = 0;
			
			transform.rotation = Quaternion.Lerp(transform.rotation,(Quaternion.LookRotation((lookDirection).normalized)),0.1f);
			transform.position += (positionQueue[0] - transform.position).normalized * moveSpeed * Time.deltaTime;
			if (!animation.IsPlaying("Run")) {animation.CrossFade("Run", 0.2F);}
			if (Vector3.Distance(positionQueue[0], transform.position) <= 0.1f) {
				positionQueue.RemoveAt(0);
				if (positionQueue.Count == 0) {
					animation.Stop();
					animation.CrossFade("Idle", 0.2F);
					AP--;
					UnitAction = unitActions.idle;
					checkAP();
				}
			}	
		}
//		UnitAction = unitActions.idle;
	}

	public void makeDead()
	{
		HP = 0;
		UnitState = unitStates.dead;
		GameManager.instance.checkVictory();
		animation.CrossFade("Death");
		StartCoroutine(WaitAnimationEnd(animation["Death"].length+delayAfterAnim,true));
	}

	public virtual void EndTurn () {
		GameManager.instance.removeTileHighlights ();
		UnitAction = unitActions.idle;
		GameManager.instance.nextTurn ();
	}

	public void OnGUI() {
		//display HP
		Vector3 location = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 55;
		GUI.Label(new Rect(location.x, Screen.height - location.y, 30, 20), HP.ToString());
	}

	public void checkAP()
	{
//		Debug.Log(actionPoints);
	}

	public void takeDamage(int damage)
	{
		HP -= damage;
		if (HP<=0)
			makeDead();
		else
		{
			animation.CrossFade("Damage",0.2f);
			StartCoroutine(WaitAnimationEnd(animation["Damage"].length+delayAfterAnim,true));
		}
	}

	public void takeHeal(int heal)
	{
		HP += heal;
		if (HP>=MaxHP)
			HP = MaxHP;
		animation.CrossFade("Damage");
		StartCoroutine(WaitAnimationEnd(animation["Damage"].length+delayAfterAnim,true));
	}

	public void placeUnit(Vector2 position)
	{
		gridPosition = position;
		currentTile = GameManager.instance.map[(int)position.x][(int)position.y];
		currentTile.unitInTile = this;
		transform.position = currentTile.transform.position + new Vector3(0,0.5f,0);
	}
	IEnumerator WaitAnimationEnd(float waitTime,bool target = false){
		yield return new WaitForSeconds(waitTime);
		if(target){
			UnitEvents.ReactionEnd(this);
		}
		if(UnitState != unitStates.dead)
			animation.CrossFade("Idle",0.2f);
	}

	public void activateReactionEnd(){
		UnitEvents.ReactionEnd(this);
	}
}
