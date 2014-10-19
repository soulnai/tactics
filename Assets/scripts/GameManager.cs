using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EnumSpace;

//Events
public delegate void VictoryState(GameManager gm,Player p);

public class GameManager : MonoBehaviour {
	public event VictoryState OnVictoryState;
	public static GameManager instance;
	//units count
	public int unitsCountPlayer;
	public int unitsCountAI;
	//prefabs
	public GameObject TilePrefab;
	public GameObject[] UserUnitPrefab;
	public GameObject AIPlayerPrefab;

	public GameObject MagicPrefab;
	public GameObject MagicExplosionPrefab;
	public GameObject magic;
	public GameObject selectionRing;
	public Unit targetPub;
	public List <Unit> targetsForAreaDamage;
	public bool Loose = false;
	public Texture ImpasTex;

	public List<Tile> highlightedTiles;
	public List<Tile> highlightedTilesArea;
	public bool magiceffect = false;
	
	public int mapSize = 22;
	public Transform mapTransform;
	Transform tileTransform;
	
	public List <List<Tile>> map = new List<List<Tile>>();
	public List <Unit> units = new List<Unit>();
	public List <Player> players = new List<Player>();
	public int currentUnitIndex = 0;
	public int currentPlayerIndex = 0;
	public Unit currentUnit{
		set{

		}
		get{
			return units[currentUnitIndex];
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

	private RaycastHit hit;
	private RaycastHit target;
	private GameObject unitSelection;


	void Awake() {
		instance = this;
		if(StartScreenPersistentObj.instance != null){
			UserUnitPrefab [0] = StartScreenPersistentObj.instance.UserUnitPrefab [0];
			UserUnitPrefab [1] = StartScreenPersistentObj.instance.UserUnitPrefab [1];
			UserUnitPrefab [2] = StartScreenPersistentObj.instance.UserUnitPrefab [2];
			UserUnitPrefab [3] = StartScreenPersistentObj.instance.UserUnitPrefab [3];
		}
		mapTransform = transform.FindChild("Map");
		generateMap();
		generateUnits();
	}

	// Use this for initialization
	void Start () {		

		unitSelection = (GameObject)Instantiate(selectionRing, units[0].transform.position, Quaternion.Euler(0,0,0));
		unitSelection.transform.parent = units [0].transform;
		Camera.main.GetComponent<CameraOrbit>().pivot = currentUnit.transform;
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
		updatePassiveAbilities();
		//reset AP
		units[0].AP = units[0].APmax;
	}
	
	// Update is called once per frame
	void Update () {
		drawPointer();
		if((currentUnit.UnitAction != unitActions.idle)&&(currentUnit.UnitAction != unitActions.moving)&&(currentUnit.UnitAction != unitActions.readyToMove))
			AttackOnMouseClick ();
	}

	public void updatePassiveAbilities(){
		foreach(Unit u in units)
		{
			u.updatePassiveAbilities();
		}
	}



	public void nextTurn() {
		if (currentUnitIndex + 1 < units.Count) {
			currentUnitIndex++;
			if(!currentPlayer.units.Contains(currentUnit))
				currentPlayerIndex++;
				if(currentPlayerIndex + 1 > players.Count)
				currentPlayerIndex = 0;
		} 
		else {
			turnsCounter++;
			currentUnitIndex = 0;
			currentPlayerIndex = 0;
		}
		if(currentUnit.UnitState == unitStates.dead)
		{
			nextTurn();
		}
		else
		{
			updatePassiveAbilities();
			currentUnit.prepareForTurn();
//			currentUnit.unitActiveEffects.ActivateAllEffects();
			GUImanager.instance.showAbilities();
			//reset AP
//			currentUnit.AP = currentUnit.APmax;
			if (currentUnit.currentStatusEffectDuration>0 || currentUnit.UnitState != unitStates.normal) {
				if (currentUnit.UnitState == unitStates.stunned) {
					currentUnit.AP = 0;
				}
				currentUnit.currentStatusEffectDuration--;
				if (currentUnit.currentStatusEffectDuration<=0 ){
					currentUnit.UnitState = unitStates.normal;
				}
			}
			removeTileHighlights();

			//reset & focus camera
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset = Vector3.zero;
			Camera.main.GetComponent<CameraOrbit>().pivot = currentUnit.transform;
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
			//set selection ring
			unitSelection.transform.position = units [currentUnitIndex].transform.position;
			unitSelection.transform.parent = units [currentUnitIndex].transform;
			//set state
			currentUnit.UnitAction = unitActions.idle;
			currentUnit.positionQueue.Clear();
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
								highlightedTiles = TileHighlight.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, units.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray (), maxHeightDiff);
		
						foreach (Tile t in highlightedTiles) {
								t.visual.transform.renderer.materials [1].color = highlightColor;
						}
				
	}

	public void AttackhighlightTiles(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {

				highlightedTiles = new List<Tile> ();

		/*		if (currentUnit.currentAbility.areaPattern == areaPatterns.line) {

						RaycastHit[] hit;
						LayerMask mask = 1 << LayerMask.NameToLayer ("tiles");
			hit = (Physics.RaycastAll (currentUnit.transform.position- 0.5f * Vector3.up, currentUnit.transform.forward, (float)currentUnit.currentAbility.range, mask));

				for (int i=0; i<hit.Length; i++)	{
					if (hit[i].transform.gameObject.GetComponent<Tile> () != null) {
						highlightedTiles.Add (hit[i].transform.gameObject.GetComponent<Tile> ());
						Tile t = hit[i].transform.gameObject.GetComponent<Tile> ();
						t.visual.transform.renderer.materials [1].color = highlightColor;
					}
				}
								
			} else {*/
				if (ignorePlayers)
					highlightedTiles = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance);
				else
					highlightedTiles = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, units.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray ());
				foreach (Tile t in highlightedTiles) 
					t.visual.transform.renderer.materials [1].color = highlightColor;
			//}
				
		}

	public void AttackhighlightTilesArea(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {
		
		highlightedTilesArea = new List<Tile> ();
		
		/*		if (currentUnit.currentAbility.areaPattern == areaPatterns.line) {

						RaycastHit[] hit;
						LayerMask mask = 1 << LayerMask.NameToLayer ("tiles");
			hit = (Physics.RaycastAll (currentUnit.transform.position- 0.5f * Vector3.up, currentUnit.transform.forward, (float)currentUnit.currentAbility.range, mask));

				for (int i=0; i<hit.Length; i++)	{
					if (hit[i].transform.gameObject.GetComponent<Tile> () != null) {
						highlightedTiles.Add (hit[i].transform.gameObject.GetComponent<Tile> ());
						Tile t = hit[i].transform.gameObject.GetComponent<Tile> ();
						t.visual.transform.renderer.materials [1].color = highlightColor;
					}
				}
								
			} else {*/
		if (ignorePlayers)
			highlightedTilesArea = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance);
		else
			highlightedTilesArea = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, units.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray ());
		/*foreach (Tile t in highlightedTiles) 
			t.visual.transform.renderer.materials [1].color = highlightColor;
		//}*/
		
	}
	
	public void removeTileHighlights() {
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				map[i][j].visual.transform.renderer.materials[1].color = Color.white;
			}
		}
	}
 	
	public void moveCurrentPlayer(Tile destTile) {
			if ((highlightedTiles.Contains(destTile)) && !destTile.impassible && currentUnit.positionQueue.Count == 0) {
			removeTileHighlights();
			currentUnit.UnitAction = unitActions.moving;
			currentUnit.currentTile.unitInTile = null;
			foreach(Tile t in TilePathFinder.FindPath(map[(int)currentUnit.gridPosition.x][(int)currentUnit.gridPosition.y],destTile, units.Where(x => x.gridPosition != destTile.gridPosition && x.gridPosition != currentUnit.gridPosition).Select(x => x.gridPosition).ToArray(),currentUnit.maxHeightDiff)) {
				currentUnit.positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position + 0.5f * Vector3.up);
				Debug.Log("(" + currentUnit.positionQueue[currentUnit.positionQueue.Count - 1].x + "," + currentUnit.positionQueue[currentUnit.positionQueue.Count - 1].y + ")");
			}			
			currentUnit.gridPosition = destTile.gridPosition;
			destTile.unitInTile = currentUnit;
			currentUnit.currentTile = destTile;
		} else {
			Debug.Log ("destination invalid");
		}
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
//			_target = unitOwner;
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


			//ckeck if hit
			bool hit = Random.Range(0.0f, 1.0f) <= unitOwner.attackChance;//replace with Ability chance
				if (Random.Range(0.0f, 1.0f) <= unitOwner.avoidChance){
					hit = false;
					GUImanager.instance.Log.addText("<b>"+_target.unitName+":</b>" + " successfuly avoided - "+ability.abilityID + " of " + unitOwner.unitName+"!");
				} 
				if (unitOwner.currentAbility.attackType == attackTypes.magic) {
					hit = true;
				} 
			//if hit
			if (hit) {
				//damage logic
				//int amountOfDamage = (int)Mathf.Floor(unitOwner.damageBase + Random.Range(0, unitOwner.damageRollSides));
					int amountOfDamage = 0;
					if (ability.attackType == attackTypes.magic || ability.attackType == attackTypes.heal || ability.attackType == attackTypes.ranged) {
						amountOfDamage = (int)Mathf.Floor(Random.Range(unitOwner.damageBase, unitOwner.maxdamageBase+1.0f) +(unitOwner.Magic/2) - _target.MagicDefense);
					} else {
						amountOfDamage = (int)Mathf.Floor(Random.Range(unitOwner.damageBase, unitOwner.maxdamageBase+1.0f) +(unitOwner.Strength/2) - _target.PhysicalDefense);
						float angle = Vector3.Angle(GameManager.instance.currentUnit.transform.forward, GameManager.instance.targetPub.transform.forward);
						Debug.Log (angle);
						if (angle <=30 && currentUnit.currentAbility.attackType == attackTypes.backstab && Random.Range(0.0f, 1.0f) <= ability.effectApplyChance){
							amountOfDamage = amountOfDamage*10;
							Debug.Log ("backstab");
						} else if (angle <=30 && currentUnit.currentAbility.attackType == attackTypes.backstab){
							amountOfDamage = amountOfDamage*5;
							Debug.Log ("backstab");
						} else if (angle <=30){
							amountOfDamage = amountOfDamage*2;
							Debug.Log ("backstab");
						}
						if (angle >=30 && angle <=90){
							amountOfDamage = amountOfDamage*2;
							Debug.Log ("flank attack");
						}
						if (angle >90){
							amountOfDamage = amountOfDamage;
							Debug.Log ("front attack");
						}

					}
				
					applyAbilityToTarget (ability, _target, amountOfDamage);

					if((ability.attackType != attackTypes.melee)&&(ability.rangedFXprefab != null))
					{
						FXmanager.instance.createAbilityFX(ability.rangedFXprefab,unitOwner.transform.position,targetUnit.transform.position,ability);
					}
					if((ability.attackType == attackTypes.melee)&&(ability.hitFXprefab != null))
					{
						FXmanager.instance.createAbilityFX(ability.hitFXprefab,targetUnit.transform.position,targetUnit.transform.position,ability);
					}


					applyAbilityEffectToTarget (ability, _target);


				GUImanager.instance.Log.addText("<b>"+unitOwner.unitName+":</b>" + " successfuly used - "+ability.abilityID + " on " + _target.unitName + " for <b><color=red>" + amountOfDamage + " damage</color></b>!");
				unitOwner.playAbility(ability);
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

	public void applyAbilityEffectToTarget (BaseAbility ability, Unit _target)
	{
		if (ability.damageType == damageTypes.blunt && currentUnit.UnitState == unitStates.normal) {
			if (Random.Range(0.0f, 1.0f) <= ability.effectApplyChance && Random.Range(0.0f, 1.0f)> _target.Strength/100) {
				_target.UnitState = unitStates.stunned;
				_target.currentStatusEffectDuration = ability.duration;
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
		if(ability.effectToApply != null)
		{
			BaseEffect ef = EffectsManager.instance.getEffect(ability.effectToApply);
			if(ef != null){
				//check - can be applied?
				if (Random.Range(0.0f, 1.0f) <= ef.effectApplyChance && _target.ResistTo(ef)) {
					_target.unitActiveEffects.AddEffect(ef);
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
			units.Add(unit);
			players[0].addUnit(unit);
		}

		for(int i=0; i< unitsCountAI;i++)
		{
			Vector2 position = getRandoMapTileXY(true);
			ai = ((GameObject)Instantiate(AIPlayerPrefab,Vector3.zero,Quaternion.identity)).GetComponent<AIPlayer>();
			ai.placeUnit(position);
			ai.unitName = "Bot-"+i;				
			ai.playerOwner = players[1];
			units.Add(ai);
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

		for (int i =0; i<units.Count; i++) {
			if (map[(int)tileXY.x][(int)tileXY.y].gridPosition == units[i].gridPosition){
				tileXY = getRandoMapTileXY();
			}		
		}
		return tileXY;
	}

	public void drawPointer()
	{
		Tile previousTile = map[0][0];
		targetsForAreaDamage = new List<Unit>();
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

		if (currentUnit.UnitAction == unitActions.magicAttack && currentUnit.currentAbility.areaDamage == true && currentUnit.currentAbility.areaPattern == areaPatterns.circle && currentUnit.UnitAction == unitActions.magicAttack) {

			if ((Physics.Raycast (ray, out hit, 1000f, mask)) && (!GUImanager.instance.mouseOverGUI)) {
				if(hit.transform.gameObject.GetComponent<Tile>() != null && hit.transform.gameObject.GetComponent<Tile>().gridPosition != previousTile.gridPosition )
				{
					//Debug.Log ("tiles hitted");
								//targetsForAreaDamage = new List<Unit>();
								removeTileHighlights ();
				
								Tile t = hit.transform.gameObject.GetComponent<Tile> ();
					AttackhighlightTilesArea(currentUnit.gridPosition, Color.red, currentUnit.currentAbility.range, true);
					AttackhighlightTiles(currentUnit.gridPosition, Color.red, currentUnit.currentAbility.range, true);
					AttackhighlightTiles(t.gridPosition, Color.green, currentUnit.currentAbility.areaDamageRadius, true);
								
								highlightedTiles.Add(t);
								highlightedTiles.Add(hit.transform.gameObject.GetComponent<Tile>());
							hit.transform.gameObject.GetComponent<Tile>().visual.transform.renderer.materials[1].color = Color.green;
							highlightedTiles.Remove(t);
						//	t.visual.transform.renderer.materials[1].color = Color.white;


					foreach (Tile tile in highlightedTiles) {
						if (tile.unitInTile != null && tile.unitInTile != currentUnit) {
							targetsForAreaDamage.Add (tile.unitInTile);
						}
						previousTile = hit.transform.gameObject.GetComponent<Tile>();
					}
					/*			highlightedTiles = TileHighlightAtack.FindHighlight (map [(int)t.gridPosition.x] [(int)t.gridPosition.y], currentUnit.currentAbility.range);
					foreach (Tile tile in highlightedTiles) 
						t.visual.transform.renderer.materials [1].color = Color.red;*/
				}		
				}
				}
	}
	
	public void AttackOnMouseClick () {

	/*	if ((Input.GetMouseButtonDown (2)) && (!GUImanager.instance.mouseOverGUI)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit))
			{
				Quaternion newRotation = Quaternion.LookRotation(hit.transform.position - currentUnit.transform.position);
				currentUnit.transform.rotation = Quaternion.Slerp(currentUnit.transform.rotation, newRotation, 1f);
				//currentUnit.transform.rotation = newRotation;
				Debug.Log("click");
			}
		}*/

				
		if ((Input.GetMouseButtonDown(0))&&(!GUImanager.instance.mouseOverGUI))
		{
			BaseAbility ability = currentUnit.currentAbility;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Tile targetTile = null;
			Unit targetUnit = null;
			if(ability.requireTarget == true){
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
			} else {
				if(currentUnit.currentAbility.areaDamage == true && currentUnit.currentAbility.areaPattern == areaPatterns.circle)
				{
					if(hit.collider.GetComponent<Tile>())
					{
						targetTile = hit.collider.GetComponent<Tile>();
						targetUnit = targetTile.unitInTile;
					}
					if (highlightedTilesArea.Contains(targetTile)) {
						currentUnit.UnitAction = unitActions.idle;
						removeTileHighlights ();
					foreach (Unit u in targetsForAreaDamage) {

						GameManager.instance.useAbility(currentUnit.currentAbility,currentUnit,null ,u);
						
						}
					}
				}
			}
		}

	}

	public void ckeckLineofSign(AIPlayer ai)
	{
//
//		if(Physics.Raycast(currentUnit.transform.position+new Vector3(0,0.5f,0),currentUnit.transform.position+new Vector3(0,0.5f,0)-ai.transform.position,out target,100f))
//		{
//			if(target.transform == ai.transform)
//			{
//				Debug.Log("Line of sign-"+target.transform.name);
//				Debug.Break();
//			}
//			else
//				Debug.Log(target.transform.name);
//		}
	}

	public void checkVictory()
	{
		int deadCount = 0;
		foreach(Player p in players){ 
			foreach(Unit u in p.units){
				if(u.UnitState == unitStates.dead)
					deadCount++;
			}
			if(deadCount == p.units.Count){
				if(OnVictoryState != null)
				{
					OnVictoryState(this,p);
				}
			}
			else
			{
				deadCount = 0;
			}
		}
	}
}
