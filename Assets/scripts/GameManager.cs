using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EnumSpace;

//Events

public delegate void UnitEvent(Unit currentUnit);
public delegate void PlayerEvent(Player currentPlayer);
public delegate void RoundEvent();

public class GameManager : MonoBehaviour {
	public event UnitEvent OnUnitPosChange;
	public event UnitEvent OnUnitTurnStart;
	public event UnitEvent OnUnitTurnEnd;
	public event PlayerEvent OnPlayerTurnStart;
	public event PlayerEvent OnPlayerTurnEnd;
	public event PlayerEvent OnVictoryState;

	public static GameManager instance;
	//units count
	public int unitsCountPlayer;
	public int unitsCountAI;
	//prefabs
	public GameObject[] UserUnitPrefab;
	public GameObject AIPlayerPrefab;

	public GameObject MagicPrefab;
	public GameObject MagicExplosionPrefab;
	public GameObject selectionRing;
	public Unit targetPub;
	public Texture ImpasTex;

	public List<Tile> highlightedTiles;
	public List<Tile> highlightedTilesArea;
	
	public int mapSize = 22;
	public Transform mapTransform;
	Transform tileTransform;
	
	public List <List<Tile>> map = new List<List<Tile>>();
	public List <Unit> unitsAll{
		get{
			List<Unit> tempList = new List<Unit>();
			foreach(Player p in players)
			{
				foreach(Unit u in p.units)
				{
					tempList.Add(u);
				}
			}
			return tempList;
		}
	}
	public List <Player> players = new List<Player>();
	public int currentUnitIndex = 0;
	public int currentPlayerIndex = 0;
	public Unit currentUnit{
		set{

		}
		get{
			return currentPlayer.units[currentUnitIndex];
		}
	}
	public Player currentPlayer{
		set{
			
		}
		get{
			return players[currentPlayerIndex];
		}
	}

	public GameObject pointer;
	public int turnsCounter = 1;
	public matchStates matchState;
	private RaycastHit hit;
	private RaycastHit target;
	private GameObject unitSelection;


	void Awake() {
		matchState = matchStates.battle;
		instance = this;
		if(StartScreenPersistentObj.instance != null){
			UserUnitPrefab [0] = StartScreenPersistentObj.instance.UserUnitPrefab [0];
			UserUnitPrefab [1] = StartScreenPersistentObj.instance.UserUnitPrefab [1];
			UserUnitPrefab [2] = StartScreenPersistentObj.instance.UserUnitPrefab [2];
			UserUnitPrefab [3] = StartScreenPersistentObj.instance.UserUnitPrefab [3];
		}
		mapTransform = transform.FindChild("Map");

		OnUnitTurnStart += TurnLogic;
		OnVictoryState += endMatch;
	}

	void endMatch (Player loserPlayer)
	{
		matchState = matchStates.victory;
		Player winnerPlayer;
		if(players[players.IndexOf(loserPlayer)+1] != null)
			winnerPlayer = players[players.IndexOf(loserPlayer)+1];
		else
			winnerPlayer = players[0];
		GUImanager.instance.Log.addText("Player - "+winnerPlayer.playerName+" Wins!");
		GUImanager.instance.victoryPanel.Init(winnerPlayer);
	}

	// Use this for initialization
	void Start () {	
		generateMap();
		generateUnits();
		unitSelection = (GameObject)Instantiate(selectionRing, currentUnit.transform.position, Quaternion.Euler(0,0,0));
		unitSelection.transform.parent = currentUnit.transform;

		Camera.main.GetComponent<CameraOrbit>().pivot = currentUnit.transform;
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;

		StartCoroutine(DelayedFirstTurnLogic(0.1f));
	}

	IEnumerator DelayedFirstTurnLogic (float time)
	{
		yield return new WaitForSeconds(time);
		firstTurnInit();
		TurnLogic(currentUnit);
	}
	
	// Update is called once per frame
	void Update () {
		drawPointer();
		drawArea ();
		if((currentUnit.UnitAction != unitActions.idle)&&(currentUnit.UnitAction != unitActions.moving)&&(currentUnit.UnitAction != unitActions.readyToMove))
			AttackOnMouseClick ();
	}


	void firstTurnInit ()
	{
		currentUnitIndex = 0;
		currentPlayerIndex = 0;
		foreach(Player p in players)
		{
			foreach(Unit u in p.units)
			{
				u.initStartEffects();
			}
		}
		foreach(Player p in players)
		{
			foreach(Unit u in p.units)
			{
				u.initStartAttributes();
			}
		}
	}

	public void PlayerEndTurn(){
		if(OnPlayerTurnEnd != null)
			OnPlayerTurnEnd(currentPlayer);
		
		if(currentPlayerIndex + 1 < players.Count)
		{
			currentPlayerIndex++;
			if(OnPlayerTurnStart != null)
				OnPlayerTurnStart(currentPlayer);
			delayCastLogic();
		}
		else
		{			
			currentPlayerIndex = 0;

			if(OnPlayerTurnStart != null)
				OnPlayerTurnStart(currentPlayer);
			delayCastLogic();
		}
		currentUnitIndex = 0;
		
		turnsCounter++;

		if(OnUnitTurnStart != null)
			OnUnitTurnStart(currentUnit);

		if(currentPlayer.playerName == "AI")
			UnitEvents.LockUI();
		else
			UnitEvents.UnlockUI();
	}

	public void delayCastLogic()
	{	
		//cast delay logic
		foreach(Unit u in currentPlayer.units){
			if (u.UnitAction == unitActions.casting && u.CastingDelay > 0) {
				u.CastingDelay--;
				u.AP = 0;
				selectNextUnit();
			} else if (u.UnitAction == unitActions.casting) {
				u.currentAbility = u.DelayedAbility;
				u.DelayedAbilityReady = true;
			}
		}
	}

	public void selectNextUnit() {
		//End turn event
		if(OnUnitTurnEnd != null)
			OnUnitTurnEnd(currentUnit);

		if (currentUnitIndex + 1 < currentPlayer.units.Count) {
			currentUnitIndex++;
		} 
		else {
			if(currentPlayer.playerName == "AI")
				PlayerEndTurn();
			else
				currentUnitIndex = 0;
		}

		if(matchState == matchStates.battle){
			if(currentUnit.UnitState == unitStates.dead)
			{
				selectNextUnit();
			}
			else
			{
				//Start turn event
				if(OnUnitTurnStart != null)
					OnUnitTurnStart(currentUnit);
			}
		}
	}

	void TurnLogic (Unit u)
	{
		if(u.UnitState != unitStates.dead){
			GUImanager.instance.showAbilities ();
			removeTileHighlights ();

			//reset & focus camera
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset = Vector3.zero;
			Camera.main.GetComponent<CameraOrbit> ().pivot = currentUnit.transform;
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;

			//set selection ring
			unitSelection.transform.position = currentUnit.transform.position;
			unitSelection.transform.parent = currentUnit.transform;
			if(u.DelayedAbilityReady)
				u.onAbility(u.DelayedAbility);
		}
		else
		{
			selectNextUnit();
		}
	}

	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance) {
		highlightTilesAt(originLocation, highlightColor, distance, true);
	}

	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers, float maxHeightDiff = 100f) {
						highlightedTiles = new List<Tile> ();

						if (ignorePlayers)
								highlightedTiles = TileHighlight.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, maxHeightDiff);
						else
								highlightedTiles = TileHighlight.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, unitsAll.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray (), maxHeightDiff);
		
						foreach (Tile t in highlightedTiles) {
								t.visual.transform.renderer.materials [1].color = highlightColor;
						}
				
	}

	public void AttackhighlightTiles(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {

				highlightedTiles = new List<Tile> ();
		
				if (ignorePlayers)
					highlightedTiles = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance);
				else
					highlightedTiles = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, unitsAll.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray ());
				foreach (Tile t in highlightedTiles) 
					t.visual.transform.renderer.materials [1].color = highlightColor;
				
		}

	public void AttackhighlightTilesArea(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {
		
		highlightedTilesArea = new List<Tile> ();

		if (ignorePlayers)
			highlightedTilesArea = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance);
		else
			highlightedTilesArea = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, unitsAll.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray ());
		
	}
	
	public void removeTileHighlights() {
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				map[i][j].visual.transform.renderer.materials[1].color = Color.white;
			}
		}
	}
 	
	public void moveUnitTo(Tile destTile) {
			if ((highlightedTiles.Contains(destTile)) && !destTile.impassible && currentUnit.positionQueue.Count == 0) {
			removeTileHighlights();
			currentUnit.UnitAction = unitActions.moving;
			currentUnit.currentTile.unitInTile = null;
			foreach(Tile t in TilePathFinder.FindPath(map[(int)currentUnit.gridPosition.x][(int)currentUnit.gridPosition.y],destTile, unitsAll.Where(x => x.gridPosition != destTile.gridPosition && x.gridPosition != currentUnit.gridPosition).Select(x => x.gridPosition).ToArray(),currentUnit.maxHeightDiff)) {
				currentUnit.positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position + 0.5f * Vector3.up);
//				Debug.Log("(" + currentUnit.positionQueue[currentUnit.positionQueue.Count - 1].x + "," + currentUnit.positionQueue[currentUnit.positionQueue.Count - 1].y + ")");
			}			
			currentUnit.gridPosition = destTile.gridPosition;
			destTile.unitInTile = currentUnit;
			currentUnit.currentTile = destTile;
			if(OnUnitPosChange != null)
				OnUnitPosChange(currentUnit);
		} else {
			Debug.Log ("destination invalid");
		}
	}



	public int calculateDamage (BaseAbility ability, Unit unitOwner, Unit _target ) {
		int amountOfDamage = 0;
		if (ability.attackType == attackTypes.magic || ability.attackType == attackTypes.heal) {
			amountOfDamage = Mathf.RoundToInt(Random.Range(unitOwner.damageBase, unitOwner.maxDamageBase+1.0f) +(unitOwner.Magic/2) - _target.MagicDef);
			if (Random.Range(0.0f, 1.0f) <= unitOwner.criticalChance){
				amountOfDamage+= Mathf.RoundToInt(amountOfDamage*unitOwner.criticalModifier);
			}
			//return amountOfDamage;
		} else {
			amountOfDamage = Mathf.RoundToInt(Random.Range(unitOwner.damageBase, unitOwner.maxDamageBase+1.0f) +(unitOwner.Strength/2) - _target.PhysicalDef);
			float angle = Vector3.Angle(GameManager.instance.currentUnit.transform.forward, GameManager.instance.targetPub.transform.forward);
			Debug.Log (angle);
			//TODO backstab apply chance
			if (angle <=30 && currentUnit.currentAbility.canBackstan && Random.Range(0.0f, 1.0f) <= 1f){
				amountOfDamage = amountOfDamage*10;
				Debug.Log ("backstab");
				return amountOfDamage;
			} else if (angle <=30 && currentUnit.currentAbility.canBackstan){
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
					amountOfDamage+= Mathf.RoundToInt(amountOfDamage*unitOwner.criticalModifier);
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

	public void useAbility(BaseAbility ability,Unit unitOwner,Tile targetTile = null,Unit targetUnit = null){

		unitOwner.attackRange = ability.range;
		Unit _target = null;
		//used on target tile or unit
		if(targetTile != null){
			if (highlightedTiles.Contains(targetTile)) {
				_target = targetTile.unitInTile;
			}
		}
		if(targetUnit != null){
			_target = targetUnit;
		}
		//used on self
		if(unitOwner == targetUnit){
			_target = currentUnit;
		}
		if(_target==null)
		{
			Debug.Log("No target selected");
		}
		targetPub = _target;
		//find new look angle
		Quaternion newOwnerRotation;
		if(_target != null)
			newOwnerRotation = getNewLookRotation(unitOwner,_target);
		else
			newOwnerRotation = unitOwner.transform.rotation;
		//rotate to target if needed
		if(newOwnerRotation != unitOwner.transform.rotation)
			unitOwner.transform.rotation = Quaternion.Slerp(unitOwner.transform.rotation, newOwnerRotation, 1f);

		//check Ability distance
		if(_target != null){
		if(checkAbilityRange(ability,unitOwner,_target))
		{
			removeTileHighlights();

			//if hit

				if (GameMath.checkIfAttackSuccesfullyHit(unitOwner,_target)) {
					//damage logic

					int amountOfDamage = 0;

					amountOfDamage = GameMath.calculateDamage(ability, unitOwner, _target);
					amountOfDamage = GameMath.ResistToDamage(_target, amountOfDamage, ability);
					//log message
					GUImanager.instance.Log.addText("<b>"+unitOwner.unitName+":</b>" + " successfuly used - "+ability.abilityID + " on " + _target.unitName + " for <b><color=red>" + amountOfDamage + " damage</color></b>!");
					unitOwner.playAbility(ability);

					applyAbilityToTarget (ability, _target, amountOfDamage);
					checkIfCastInterrupted(_target);


					if((ability.attackType != attackTypes.melee)&&(ability.rangedFXprefab != null))
					{
						FXmanager.instance.createAbilityFX(ability.rangedFXprefab,unitOwner.transform.position,targetUnit.transform.position,ability);
					}
					if((ability.attackType == attackTypes.melee)&&(ability.hitFXprefab != null))
					{
						FXmanager.instance.createAbilityFX(ability.hitFXprefab,targetUnit.transform.position,targetUnit.transform.position,ability);
					}



			//if missed
			} else {
				GUImanager.instance.Log.addText(unitOwner.unitName + " missed " + _target.unitName + "!");
				unitOwner.playAbility(ability,true);
			}

		}
		else
		{
			Debug.Log("Ability range is less than target");
		}
		}
	}

	public void checkIfCastInterrupted(Unit _target){
			if (_target.UnitAction == unitActions.casting){
				if (Random.Range(0.0f, 1.0f) >= currentUnit.Magic/100){
					Debug.Log ("Cast interrupted!");
					_target.CastingDelay = 0;
					_target.UnitAction = unitActions.idle;
					_target.DelayedAbilityReady = false;
					_target.DelayedAbility = null;
				}
			}
		}

	public void applyAbilityToTarget (BaseAbility ability, Unit _target, int amountOfDamage)
	{
		if (ability.attackType == attackTypes.heal) {
			_target.takeHeal (amountOfDamage);
		}
		else {
			_target.takeDamage (amountOfDamage);
		}

		//Apply Effect
		if(ability.effects.Count > 0)
		{
			foreach(string s in ability.effects){
				BaseEffect ef = BaseEffectsManager.instance.getEffect(s);
				if(ef != null){
					if(ef.requireTarget)
						currentUnit.unitBaseEffects.addEffect(ef,_target);
					else
						currentUnit.unitBaseEffects.addEffect(ef);
				}
			}
		}
	}

	public bool checkAbilityRange (BaseAbility ability, Unit owner,Unit target)
	{
		if (owner == target)
			return true;

		if (highlightedTiles.Contains (target.currentTile) && currentUnit.currentAbility.areaDamage == true) {
			
			if (owner.gridPosition.x >= target.gridPosition.x - owner.attackRange- currentUnit.currentAbility.areaDamageRadius/2 && owner.gridPosition.x <= target.gridPosition.x + owner.attackRange+ currentUnit.currentAbility.areaDamageRadius/2 &&
			    owner.gridPosition.y >= target.gridPosition.y - owner.attackRange- currentUnit.currentAbility.areaDamageRadius/2 && owner.gridPosition.y <= target.gridPosition.y + owner.attackRange+ currentUnit.currentAbility.areaDamageRadius/2) 
			{
				return true;
			} else
				return false;
		}

		if (highlightedTiles.Contains (target.currentTile)) {
						
						if (owner.gridPosition.x >= target.gridPosition.x - owner.attackRange && owner.gridPosition.x <= target.gridPosition.x + owner.attackRange &&
								owner.gridPosition.y >= target.gridPosition.y - owner.attackRange && owner.gridPosition.y <= target.gridPosition.y + owner.attackRange) {
								return true;
						} else
								return false;
		}	else
			return false;
	}

	Quaternion getNewLookRotation (Unit owner, Unit target)
	{
		if(owner != target){
			Vector3 targetPos = target.transform.position;
			targetPos.y = 0;
			Vector3 attackerPos = currentUnit.transform.position;
			attackerPos.y = 0;
			Quaternion newRotation = Quaternion.LookRotation(targetPos - attackerPos);
			return newRotation;
		}
		else
			return owner.transform.rotation;
	}
	

	void generateMap() {
		loadMapFromXml();
	}

	void loadMapFromXml() {
		MapXmlContainer container = MapSaveLoad.Load("/Asets/Resources/map.xml");
		
		mapSize = container.size;
		
		//initially remove all children
		for(int i = 0; i < mapTransform.childCount; i++) {
			Destroy (mapTransform.GetChild(i).gameObject);
		}
		
		map = new List<List<Tile>>();
		for (int i = 0; i < mapSize; i++) {
			List <Tile> row = new List<Tile>();
			for (int j = 0; j < mapSize; j++) {
				float tileHeight = container.tiles.Where(x => x.locX == i && x.locY == j).First().height;
				Tile tile = ((GameObject)Instantiate(PrefabHolder.instance.BASE_TILE_PREFAB, new Vector3(i - Mathf.Floor(mapSize/2),tileHeight, -j + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();

				tile.transform.parent = mapTransform;
				tile.gridPosition = new Vector2(i, j);
				tile.setType((TileType)container.tiles.Where(x => x.locX == i && x.locY == j).First().id);
				tile.height = tileHeight;
				row.Add (tile);
			}
			map.Add(row);
		}
	}

	void generateUnits() {
		Unit unit;
		AIPlayer ai;
		for(int i=0; i< unitsCountPlayer;i++)
		{
			Vector2 position = getRandoMapTileXY(true);
			unit = ((GameObject)Instantiate(UserUnitPrefab[i],Vector3.zero,Quaternion.identity)).GetComponent<Unit>();
			unit.placeUnit(position);
			unit.unitName = "Alice-"+i;
			unit.playerOwner = players[0];
			players[0].addUnit(unit);
			GUImanager.instance.unitPanels[i].Init(unit);
		}

		for(int i=0; i< unitsCountAI;i++)
		{
			Vector2 position = getRandoMapTileXY(true);
			ai = ((GameObject)Instantiate(AIPlayerPrefab,Vector3.zero,Quaternion.identity)).GetComponent<AIPlayer>();
			ai.placeUnit(position);
			ai.unitName = "Bot-"+i;				
			ai.playerOwner = players[1];
			players[1].addUnit(ai);
		}

	}

	public Vector2 getRandoMapTileXY(bool passible = false)
	{
		Vector2 tileXY = new Vector2(Random.Range(0,mapSize),Random.Range(0,mapSize));

		if ((passible == true)&&(map[(int)tileXY.x][(int)tileXY.y].impassible == true))
		{
			tileXY = getRandoMapTileXY(passible);
		}

		for (int i =0; i<unitsAll.Count; i++) {
			if (map[(int)tileXY.x][(int)tileXY.y].gridPosition == unitsAll[i].gridPosition){
				tileXY = getRandoMapTileXY();
			}		
		}
		return tileXY;
	}

	public void drawPointer()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask mask = 1<<LayerMask.NameToLayer("tiles");

		if((Physics.Raycast(ray,out hit,1000f,mask))&&(!GUImanager.instance.mouseOverGUI))
		{
			if(hit.transform.gameObject.GetComponent<Tile>() != null)
			{
				pointer.SetActive(true);
				Tile t = hit.transform.gameObject.GetComponent<Tile>();
				pointer.transform.position = Vector3.Lerp(pointer.transform.position,(t.transform.position+new Vector3(0,0.5f,0)),0.5f);
			}
			else
			{
				pointer.SetActive(false);
			}
		}
		else
		{
			pointer.SetActive(false);
		}
	}

	void drawArea ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask mask = 1<<LayerMask.NameToLayer("tiles");
		if (currentUnit.currentAbility.areaDamage == true && currentUnit.UnitAction == unitActions.readyToAttack) {
			if ((Physics.Raycast (ray, out hit, 1000f, mask)) && (!GUImanager.instance.mouseOverGUI)) {
				if (hit.transform.gameObject.GetComponent<Tile> () != null) {
					removeTileHighlights ();
					Tile t = hit.transform.gameObject.GetComponent<Tile> ();
					AttackhighlightTilesArea (currentUnit.gridPosition, Color.red, currentUnit.currentAbility.range, true);
					AttackhighlightTiles (currentUnit.gridPosition, Color.red, currentUnit.currentAbility.range, true);
					AttackhighlightTiles (t.gridPosition, Color.green, currentUnit.currentAbility.areaDamageRadius, true);
					highlightedTiles.Add (t);
					//TODO better solution for Highlighted tiles center tile
					t.visual.transform.renderer.materials [1].color = Color.green;
					}
				}
			}

	}
	
	public void AttackOnMouseClick () {
		if ((Input.GetMouseButtonDown(0))&&(!GUImanager.instance.mouseOverGUI))
		{
			BaseAbility ability = currentUnit.currentAbility;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Tile targetTile = null;
			Unit targetUnit = null;

			if(Physics.Raycast(ray,out hit))
			{
				if(hit.collider.GetComponent<Tile>())
				{
					targetTile = hit.collider.GetComponent<Tile>();
					targetUnit = targetTile.unitInTile;
				}
				if(hit.collider.GetComponent<Unit>())
				{
					targetUnit = hit.collider.GetComponent<Unit>();
					targetTile = targetUnit.currentTile;
				}

			if(ability.requireTarget == true){
					if(ability.selfUse == true)
					{
						if(currentUnit == targetUnit){
							GameManager.instance.useAbility(ability,currentUnit,targetTile,targetUnit);
						}
						else
							Debug.Log("Not self selected");
					}



					if(ability.allyUse == true)
					{
						if((currentPlayer.units.Contains(targetUnit))&&(currentUnit != targetUnit)){
							GameManager.instance.useAbility(ability,currentUnit,targetTile,targetUnit);
						}
						else
							Debug.Log("Not ally selected");
					}
					else
					{
						if((!currentPlayer.units.Contains(targetUnit))&&(currentUnit != targetUnit)){
							GameManager.instance.useAbility(ability,currentUnit,targetTile,targetUnit);
						}
						else
							Debug.Log("Ally selected");
					}

				}
			 else {
				if(currentUnit.currentAbility.areaDamage == true)
				{
					if (highlightedTilesArea.Contains(targetTile)) {
						currentUnit.UnitAction = unitActions.idle;
						removeTileHighlights ();
							foreach (Unit u in findTargets(currentUnit,currentUnit.currentAbility.areaDamageRadius,currentUnit.currentAbility.enemieUse,currentUnit.currentAbility.allyUse,currentUnit.currentAbility.selfUse,targetTile)) {
							if (u.UnitState!=unitStates.dead){
								GameManager.instance.useAbility(currentUnit.currentAbility,currentUnit,null ,u);}
						}
					}
				}
			}
			}
		}

	}



	public void checkVictory()
	{
		foreach(Player p in players){ 
			if(p.unitsDead.Count >= p.units.Count){
				if(OnVictoryState != null)
				{
					OnVictoryState(p);
				}
			}
		}
	}

	public List<Unit> findTargets (Unit owner, int radius, bool enemieUse, bool allyUse, bool selfUse, Tile centerTile = null)
	{
		if(centerTile == null)
			centerTile = owner.currentTile;
		List<Tile> tempTiles = TileHighlightAtack.FindHighlight (map [(int)centerTile.gridPosition.x] [(int)centerTile.gridPosition.y], radius);
		tempTiles.Add(centerTile);
		List<Unit> tempUnits = new List<Unit>();
//		Debug.Log(tempTiles.Count);
		foreach(Tile t in tempTiles)
		{
			if(t.unitInTile!=null){
				Unit tempUnit = t.unitInTile;
				if(tempUnit.UnitState != unitStates.dead){
					if((enemieUse)&&(tempUnit.playerOwner != owner.playerOwner))
						tempUnits.Add(tempUnit);
					if((allyUse)&&(tempUnit.playerOwner == owner.playerOwner)&&(tempUnit != owner))
						tempUnits.Add(tempUnit);
					if((selfUse)&&(tempUnit == owner))
						tempUnits.Add(tempUnit);
				}
			}

		}
		return tempUnits;
	}

	public void ShowMovementDistance(Unit u){
		if (!GUImanager.instance.mouseOverGUI) {
			if(currentUnit.UnitAction == unitActions.idle){
				highlightTilesAt(u.gridPosition, Color.magenta, u.movementPerActionPoint*2,false,u.maxHeightDiff);
				highlightTilesAt(u.gridPosition, Color.cyan, u.movementPerActionPoint,false,u.maxHeightDiff);
			}
		}
	}

	public void selectUnit(Unit u){
		if(u.playerOwner == currentPlayer){
			currentUnit = u;
			currentUnitIndex = u.playerOwner.units.IndexOf(u);
		}
		TurnLogic(currentUnit);
	}
}
