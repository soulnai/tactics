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
	public int Dexterity = 2;
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
	public EnumSpace.unitClass UnitClass;
	public EnumSpace.unitStates UnitState;
	public EnumSpace.unitActions UnitAction;
	public Tile currentTile;
	public BaseAbility currentAbility;
	public Unit currentTarget;
	public int currentStatusEffectDuration;

	//movement animation
	public List<Vector3> positionQueue = new List<Vector3>();	
	//
	public AbilitiesController unitAbilitiesController;
	public PassiveAbilitiesController unitPassiveAbilitiesController;
	public EffectsController unitActiveEffects;
	public Player playerOwner;


	private Vector3 lookDirection = Vector3.zero;
	private float delayAfterAnim = 0.5f;
	private bool canEndTurn = false;
	private List<BasePassiveAbility> unitActivePassiveAbilities = new List<BasePassiveAbility>();	

	void Awake () {
		moveDestination = transform.position;
		AP = APmax;
	}

	public void prepareForTurn()
	{
		unitActiveEffects.ActivateAllEffects();
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

	public void resetAttributes()
	{
		UnitManager um = UnitManager.instance;
		Unit u = um.getUnit(this);
		HPmax = u.HPmax;
		HP = u.HPmax;
		APmax = u.APmax;
		AP = u.AP;
		MPmax = u.MPmax;
		MP = u.MP;
		Strength = u.Strength;
		Dexterity = u.Dexterity;
		Magic = u.Magic;
		PhysicalDefense = u.PhysicalDefense;
		MagicDefense = u.MagicDefense;
	}

	public void initAttributes(BasePassiveAbility pa)
	{
		if(unitActivePassiveAbilities.Contains(pa)){
				foreach(BaseAttributeChanger ac in pa.affectedAttributes)
				{
					switch (ac.attribute) {
					case unitAttributes.AP:
							updateParameter (ac, ref APmax);
						break;
					case unitAttributes.MP:
						updateParameter (ac, ref MPmax);
						break;

					case unitAttributes.HP:
						updateParameter (ac, ref HPmax);
						break;
					case unitAttributes.dexterity:
						updateParameter (ac, ref Dexterity);
						break;
					case unitAttributes.strenght:
						updateParameter (ac, ref Strength);
						break;
					case unitAttributes.magic:
						updateParameter (ac, ref Magic);
						break;
					case unitAttributes.magicDef:
						updateParameter (ac, ref MagicDefense);
						break;
					}
				}
		}
	}

	/// <summary>
	/// Float param
	/// </summary>
	public void updateParameter (BaseAttributeChanger ac, ref float parameter)
	{
		if (ac.multiply)
			parameter = parameter * ac.value;
		else
			parameter = parameter + ac.value;
	}

	/// <summary>
	/// Int param
	/// </summary>
	public void updateParameter (BaseAttributeChanger ac, ref int parameter)
	{
		if (ac.multiply)
			parameter = Mathf.RoundToInt(parameter * ac.value);
		else
			parameter = Mathf.RoundToInt(parameter + ac.value);
	}
	/// <summary>
	/// Updates the passive abilities.
	/// </summary>
	public void updatePassiveAbilities()
	{
		foreach(BasePassiveAbility pa in unitPassiveAbilitiesController.passiveAbilities)
		{
			List<Unit> affectedUnits = new List<Unit>();
			if(pa.selfUse)
				affectedUnits.Add(this);
			if(pa.allyUse){
				foreach(Unit u in playerOwner.units)
			if(u != this)							
				affectedUnits.Add(u);
			}
			if(pa.enemieUse){
				foreach(Unit u in GameManager.instance.units)
					if(!playerOwner.units.Contains(u))
						affectedUnits.Add(u);
			}				
			foreach(Unit u in affectedUnits)
			{
				pa.AddUnit(u);
			}
		}
	}

	public void deleteAllPassiveAbilities()
	{
		foreach(BasePassiveAbility pa in unitPassiveAbilitiesController.passiveAbilities)
		{
			foreach(Unit u in pa.affectedUnits)
			{
				pa.RemoveUnit(u);
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
		
			if(UnitAction == unitActions.moving && GameManager.instance.currentUnit.UnitState != unitStates.dead)
			{
				MoveUnit();
			}

			if (HP <=0){
				makeDead();
			}
		}
	}

	public void onAbility(BaseAbility a)
	{
		GameManager.instance.removeTileHighlights ();

		if (unitAbilitiesController.abilities.Contains(a)) {
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
		deleteAllPassiveAbilities();
		GameManager.instance.checkVictory();
		animation.CrossFade("Death");
		StartCoroutine(WaitAnimationEnd(animation["Death"].length+delayAfterAnim,true));
	}

	public virtual void EndTurn () {
		unitActiveEffects.ClearOldEffects();
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

	public void addPassiveAbility (BasePassiveAbility pa)
	{
		if(!unitActivePassiveAbilities.Contains(pa)){
			unitActivePassiveAbilities.Add(pa);
			initAttributes(pa);
		}
		else
			Debug.Log("This unit already under the same passive ability");
	}

	public void removePassiveAbility (BasePassiveAbility pa)
	{
		if(unitActivePassiveAbilities.Contains(pa))
			unitActivePassiveAbilities.Remove(pa);
		else
			Debug.Log("This unit does not affected by this passive ability");
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
		unitAttributes resistType;
		float resist = 0;

		switch (damageType) {
		case damageTypes.blunt:
			resistType = unitAttributes.strenght;
			resist = Strength;
			break;
		case damageTypes.poison:
			resistType = unitAttributes.dexterity;
			resist = Strength;
			break;
		case damageTypes.electricity:
			resistType = unitAttributes.magic;
			resist = MagicDefense;
			break;
		case damageTypes.fire:
			resistType = unitAttributes.magic;
			resist = MagicDefense;
			break;
		case damageTypes.ice:
			resistType = unitAttributes.magic;
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
