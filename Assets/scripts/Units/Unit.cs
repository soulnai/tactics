using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

[System.Serializable]
public class Unit : MonoBehaviour {

	public Sprite icon;

	public bool isFlying = false;

	public Vector2 gridPosition = Vector2.zero;
	
	public Vector3 moveDestination;
	public float moveSpeed = 5.0f;
	
	public int movementPerActionPoint = 5;
	public int attackRange = 1;
	public int attackDistance = 5;

	public string unitName = "George";
	public List<BaseAttribute> attributes = new List<BaseAttribute>();

	//Attributes
	public int HPmax{
		get{
			return getAttribute(unitAttributes.HPmax).valueMod;
		}
	}
	public int HP{
		set{
			setAttribute(unitAttributes.HP,value);
			checkHP();
		}
		get{
			return getAttribute(unitAttributes.HP).valueMod;
		}
	}
	public int APmax{
		get{
			return getAttribute(unitAttributes.APmax).valueMod;
		}
	}
	public int AP{
		set{
			setAttribute(unitAttributes.AP,value);
		}
		get{
			return getAttribute(unitAttributes.AP).valueMod;
		}
	}
	public int MPmax{
		get{
			return getAttribute(unitAttributes.MPmax).valueMod;
		}
	}
	public int MP{
		set{
			setAttribute(unitAttributes.MP,value);
		}
		get{
			return getAttribute(unitAttributes.MP).valueMod;
		}
	}
	public int Strength{
		get{
			return getAttribute(unitAttributes.strenght).valueMod;
		}
	}
	public int Dexterity{
		get{
			return getAttribute(unitAttributes.dexterity).valueMod;
		}
	}
	public int Magic{
		get{
			return getAttribute(unitAttributes.magic).valueMod;
		}
	}
	public int PhysicalDef{
		set{
			setAttribute(unitAttributes.PhysicalDef,value);
		}
		get{
			return getAttribute(unitAttributes.PhysicalDef).valueMod;
		}
	}
	public int MagicDef{
		set{
			setAttribute(unitAttributes.magicDef,value);
		}
		get{
			return getAttribute(unitAttributes.magicDef).valueMod;
		}
	}

	public float attackChance = 0.75f;
	public float magicAttackChance = 0.75f;
	public float rangedAttackChance = 0.75f;
	public float avoidChance = 0.15f;
	public float criticalChance = 0.1f;
	public float criticalModifier = 1.5f;
	public int damageBase = 1;
	public int maxDamageBase = 5;
	public float damageRollSides = 6; //d6
	public float maxHeightDiff = 0.5f;
	public EnumSpace.unitClass UnitClass;
	public EnumSpace.unitStates UnitState;
	public EnumSpace.unitActions UnitAction;
	public Tile currentTile;
	public BaseAbility currentAbility;
	public int CastingDelay;
	public BaseAbility DelayedAbility;
	public bool DelayedAbilityReady;
	public Unit currentTarget;

	//movement animation
	public List<Vector3> positionQueue = new List<Vector3>();	
	//
	public AbilitiesController unitAbilitiesController;
	public BaseEffectController unitBaseEffects;
	public Player playerOwner;


	private Vector3 lookDirection = Vector3.zero;
	private float delayAfterAnim = 0.5f;
	private bool canEndTurn = false;

	private static GameManager gm{
		get{
			return GameManager.instance;
		}
	}

	public Dictionary<unitAttributes,BaseAttribute> attributesDictionary = new Dictionary<unitAttributes, BaseAttribute>();
	public Dictionary<unitAttributes,BaseAttribute> attributesModDictionary = new Dictionary<unitAttributes, BaseAttribute>();

	void Awake () {
		moveDestination = transform.position;
		getAttribute(unitAttributes.AP).value = APmax;
	}

	public void prepareForTurn()
	{
		getAttribute(unitAttributes.AP).value = APmax;
	}

	public void checkEndTurn()
	{
		if(gm.currentUnit == this)
		{
			if((getAttribute(unitAttributes.AP).value<=0)&&(canEndTurn == true)&&(UnitState != unitStates.dead))
			{
				canEndTurn = false;
				positionQueue.Clear();
				StartCoroutine(delayedEndTurn(delayAfterAnim));
			}
		}
	}

	public void ReactionsEnd (Unit unit)
	{
		UnitEvents.onUnitReactionEnd -= ReactionsEnd;
		canEndTurn = true;
		Debug.Log("This - "+this.unitName+" // target - " +unit.unitName+"Reaction End");
	}

	// Update is called once per frame
	public virtual void Update () {		
		if(gm.currentUnit == this)
		{
			checkEndTurn();
			if(UnitAction == unitActions.moving && GameManager.instance.currentUnit.UnitState != unitStates.dead)
			{
				MoveUnit();
			}
		}

	}

	/// <summary>
	/// Activates the "a" ability.
	/// </summary>
	public void onAbility(BaseAbility a)
	{
		UnitAction = unitActions.readyToAttack;
		gm.removeTileHighlights ();

		if (unitAbilitiesController.abilities.Contains(a)) {
			if((AP > 0)&&(MP >= a.MPCost)){

					GameManager.instance.checkDelayedAbility (a);
				DelayedAbilityReady = false;
				if((a.selfUse)&&(!a.allyUse)&&(!a.enemieUse)){
					gm.useAbility(a,this,currentTile,this);
				}
				else
				{
					currentAbility = a;
					attackDistance = a.range;
					damageBase = a.baseDamage;
//					UnitAction = a.unitAction;
					gm.AttackhighlightTiles (gridPosition, Color.red, attackDistance, true);
				}
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

		if(a.MPCost > 0)
		{
			MP -= a.MPCost;
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
		gm.removeTileHighlights ();
		if(AP > 0){
			UnitAction = unitActions.readyToMove;
			gm.highlightTilesAt (gridPosition, Color.blue, movementPerActionPoint, false, maxHeightDiff);
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
		UnitState = unitStates.dead;
		unitBaseEffects.deleteAllEffects(true);
		animation.CrossFade("Death");
		StartCoroutine(WaitAnimationEnd(animation["Death"].length+delayAfterAnim));
	}

	public virtual void EndTurn () {
		gm.removeTileHighlights ();
		if(UnitState != unitStates.dead)
			UnitAction = unitActions.idle;
		canEndTurn = false;
		gm.nextTurn ();
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
		currentTile = gm.map[(int)position.x][(int)position.y];
		currentTile.unitInTile = this;
		transform.position = currentTile.transform.position + new Vector3(0,0.5f,0);
	}
	
	IEnumerator WaitAnimationEnd(float waitTime,bool triggerReactionEnd = false){
		yield return new WaitForSeconds(waitTime);
		if(triggerReactionEnd){
			UnitEvents.ReactionEnd(this);
		}
		if(UnitState != unitStates.dead)
			animation.CrossFade("Idle",0.2f);
		else
		{
			StartCoroutine(delayedEndTurn(delayAfterAnim));
		}
	}

	IEnumerator delayedEndTurn (float t)
	{
		yield return new WaitForSeconds(t);
		EndTurn();
	}

	public BaseAttribute getAttribute(unitAttributes a)
	{
		return attributes.Find(BaseAttribute => BaseAttribute.attribute == a);
	}

	public void setAttribute(unitAttributes a,int val)
	{
		attributes.Find(BaseAttribute => BaseAttribute.attribute == a).value = val;
		if(a == unitAttributes.HP)
			checkHP();
	}

	public void initStartEffects()
	{
		unitBaseEffects.initEffects();
	}

	public void initStartAttributes()
	{
		unitBaseEffects.updateModsFromAppliedEffects();
		
		HP = HPmax;
		MP = MPmax;
		AP = APmax;
	}

	public void checkHP ()
	{
		if(HP<=0)
		{
			makeDead();
		}
	}

	void OnMouseEnter(){
//		gm.ShowMovementDistance(this);
	}

	void OnMouseExit(){
//		gm.removeTileHighlights();
	}
}
