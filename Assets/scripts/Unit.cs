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
	public int HPmax = 25;
	public int HP = 25;
	public int APmax = 2;
	public int AP = 2;
	public int MPmax = 25;
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
	public float maxHeightDiff = 0.5f;

	public EnumSpace.unitStates UnitState;
	public EnumSpace.unitActions UnitAction;
	public Tile currentTile;
	public BaseAbility currentAbility;
	public Unit currentTarget;
	public int currentStatusEffectDuration;

	//movement animation
	public List<Vector3> positionQueue = new List<Vector3>();	
	//
	public AbilitiesController unitAbilities;
	public EffectsController unitEffects;

	private Vector3 lookDirection = Vector3.zero;
	private float delayAfterAnim = 0.5f;
	private bool canEndTurn = false;

	void Awake () {
		moveDestination = transform.position;
		AP = APmax;
	}

	public void checkEndTurn()
	{
		if(GameManager.instance.currentUnit == this)
		{
			if((AP<=0)&&(canEndTurn == true))
			{
				canEndTurn = false;
				StartCoroutine(delayedEndTurn(delayAfterAnim));
			}
			else if(UnitState == unitStates.dead)
			{
				EndTurn();
			}
		}
	}

	public void ReactionsEnd (Unit unit)
	{
		UnitEvents.onUnitReactionEnd -= ReactionsEnd;
		canEndTurn = true;
		Debug.Log("This - "+this.unitName+" // target - " +unit.unitName+"Reaction End");
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	public virtual void Update () {		
		if(GameManager.instance.currentUnit == this)
		{
			checkEndTurn();
		
			if(UnitAction == unitActions.moving)
			{
				MoveUnit();
			}
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
				UnitAction = a.unitAction;
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

	public void playAbility(BaseAbility a,bool missed = false)
	{
		canEndTurn = false;
		UnitEvents.onUnitReactionEnd += ReactionsEnd;
		UnitEvents.onUnitFXEnd += FXend;

		if(a.endsUnitTurn){
			AP = 0;
		}
		else
		{
			AP -= a.APcost;
		}
		animation.Play("Attack");
		StartCoroutine(WaitAnimationEnd(animation["Attack"].length));

		if(missed)
		{
			StartCoroutine(WaitAnimationEnd(animation["Attack"].length,true));
		}
	}

	public void FXend(Unit owner,Unit target)
	{
		if(owner = this)
			Debug.Log("FX ended");
	}

	public void tryMove ()
	{
		GameManager.instance.removeTileHighlights ();
		if(AP > 0){
			UnitAction = unitActions.readyToMove;
			GameManager.instance.highlightTilesAt (gridPosition, Color.blue, movementPerActionPoint, false, maxHeightDiff);
		}
	}

	public void MoveUnit()
	{
		if (positionQueue.Count > 0) {
			canEndTurn = false;
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
					canEndTurn = true;
				}
			}	
		}
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
		if(UnitState != unitStates.dead)
			UnitAction = unitActions.idle;
		canEndTurn = false;
		GameManager.instance.nextTurn ();
	}

	public void OnGUI() {
		//display HP
		Vector3 location = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 55;
		GUI.Label(new Rect(location.x, Screen.height - location.y, 30, 20), HP.ToString());
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
		if (HP>=HPmax)
			HP = HPmax;
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

	public bool ResistTo (BaseEffect ef)
	{
		damageTypes damageType = ef.damageType;
		resistTypes resistType;
		float resist = 0;

		switch (damageType) {
		case damageTypes.blunt:
			resistType = resistTypes.strenght;
			resist = Strength;
			break;
		case damageTypes.poison:
			resistType = resistTypes.dexterity;
			resist = Strength;
			break;
		case damageTypes.electricity:
			resistType = resistTypes.magic;
			resist = MagicDefense;
			break;
		case damageTypes.fire:
			resistType = resistTypes.magic;
			resist = MagicDefense;
			break;
		case damageTypes.ice:
			resistType = resistTypes.magic;
			resist = MagicDefense;
			break;
		}

		//compare
		if (Random.Range(0.0f, 1.0f)> resist/100)
			return true;
		else
			return false;
	}

	IEnumerator WaitAnimationEnd(float waitTime,bool triggerReactionEnd = false){
		yield return new WaitForSeconds(waitTime);
		if(triggerReactionEnd){
			UnitEvents.ReactionEnd(this);
		}
		if(UnitState != unitStates.dead)
			animation.CrossFade("Idle",0.2f);
	}

	IEnumerator delayedEndTurn (float t)
	{
		yield return new WaitForSeconds(t);
		EndTurn();
	}
}
