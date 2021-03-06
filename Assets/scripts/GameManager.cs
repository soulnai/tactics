using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EnumSpace;

//Events
public delegate void RoundEvent();

public class GameManager : MonoBehaviour {



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
    public GameObject tileSelectionBox;

    public List<Tile> highlightedTiles;
	public List<Tile> highlightedTilesArea;
	
	public int mapSize = 22;
	public Transform mapTransform;
	
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
	private int _currentUnitIndex = 0;
	public int currentUnitIndex{
		set{
			_currentUnitIndex = value;	
			EventManager.UnitSelectionChanged(currentUnit);
		}
		get{
			return _currentUnitIndex;
		}
	}
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

	private GameObject unitSelection;

	private List<Tile> startTilesFirst = new List<Tile>();
	private List<Tile> startTilesSecond = new List<Tile>();

	void Awake() {
		instance = this;
		if(StartScreenPersistentObj.instance != null){
			UserUnitPrefab [0] = StartScreenPersistentObj.instance.UserUnitPrefab [0];
			UserUnitPrefab [1] = StartScreenPersistentObj.instance.UserUnitPrefab [1];
			UserUnitPrefab [2] = StartScreenPersistentObj.instance.UserUnitPrefab [2];
			UserUnitPrefab [3] = StartScreenPersistentObj.instance.UserUnitPrefab [3];
		}
		mapTransform = transform.FindChild("Map");
	}

	void startPlacePhase ()
	{
		GUImanager.instance.Log.addText("<color=green>Placement Phase</color>");
		currentUnitIndex = 0;
		matchState = matchStates.placeUnits;

		startTilesSecond = new List<Tile>();
		for(int i=map.Count/2-3;i<map.Count/2+3;i++)
			for(int j=map[0].Count-4;j<map[0].Count;j++)
				startTilesSecond.Add(map[i][j]);

		startTilesFirst = new List<Tile>();
		for(int i=map.Count/2-3;i<map.Count/2+3;i++)
			for(int j=0;j<4;j++)
				startTilesFirst.Add(map[i][j]);

		foreach (Tile t in startTilesFirst) {
			t.showHighlight(ColorHolder.instance.move);
		}

		placeAIUnits();
	}

	void placeAIUnits ()
	{
		foreach(Player p in players)
			if(p.type == playerType.ai)
			foreach(Unit u in p.units){
				u.placeUnit(getRandoMapTileXY(startTilesSecond));
			}
	}

	void placeMissingUnits ()
	{
		foreach(Player p in players){
			foreach(Unit u in p.units){
				if(u.currentTile == null){
					if(p.type == playerType.player)
					{u.placeUnit(getRandoMapTileXY(startTilesFirst));
						Vector3 centerTilePos = map[11][11].transform.position;
						centerTilePos.y = 0;
						Vector3 unitPos = u.transform.position;
						unitPos.y = 0;
						Quaternion newRotation = Quaternion.LookRotation(centerTilePos - unitPos);
						u.transform.rotation = newRotation;}
					else
					{	u.placeUnit(getRandoMapTileXY(startTilesSecond));
						Vector3 centerTilePos = map[11][11].transform.position;
						centerTilePos.y = 0;
						Vector3 unitPos = u.transform.position;
						unitPos.y = 0;
						Quaternion newRotation = Quaternion.LookRotation(centerTilePos - unitPos);
						u.transform.rotation = newRotation;}
				}
			}
		}
	}

	public void startBattlePhase ()
	{
		GUImanager.instance.Log.addText("<color=green>Battle Phase</color>");
		placeMissingUnits();
		EventManager.UnlockUI();
		matchState = matchStates.battle;
		EventManager.OnUnitTurnStart += TurnLogic;
		EventManager.OnVictoryState += endMatch;

		Camera.main.GetComponent<CameraOrbit>().pivot = currentUnit.transform;
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
		
		firstTurnInit();
		TurnLogic(currentUnit);
	}

	void endMatch (Player loserPlayer)
	{
		matchState = matchStates.victory;
		Player winnerPlayer;
		if(players.IndexOf(loserPlayer)+1 < players.Count)
			winnerPlayer = players[players.IndexOf(loserPlayer)+1];
		else
			winnerPlayer = players[0];
		GUImanager.instance.Log.addText("Player - "+winnerPlayer.playerName+" Wins!");
		GUImanager.instance.victoryPanel.Init(winnerPlayer);
	}

	// Use this for initialization
	void Start () {	
		unitSelection = (GameObject)Instantiate(selectionRing,new Vector3(-1000f,-1000f,-1000f), Quaternion.identity);

        EventManager.onTileClick += selectUnit;
		EventManager.onTileClick += PlaceUnit;
        EventManager.onTileClick += useAbility;

		EventManager.onTileCursorOverChanged += drawPointer;
		EventManager.onTileCursorOverChanged += drawArea;

		EventManager.onUnitClick += useAbility;
        EventManager.onUnitClick += selectUnit;
	    EventManager.OnCursorEnterUnit += drawPointer;

		EventManager.onUnitSelectionChanged += setSelectionRing;

		EventManager.LockUI();

		generateMap();
		generateUnits();
		startPlacePhase();
	}

	void PlaceUnit (Tile t)
	{
		if((matchState == matchStates.placeUnits)&&(!GUImanager.instance.mouseOverGUI)){
			if((!t.impassible)&&(startTilesFirst.Contains(t))&&(t.unitInTile == null)){
				currentUnit.placeUnit(t.gridPosition);
				selectNextUnit();
			}
		}
	}


	void firstTurnInit ()
	{
		currentUnitIndex = 0;
		currentPlayerIndex = 0;
		EventManager.PlayerTurnStart(currentPlayer);
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
		EventManager.PlayerTurnEnd(currentPlayer);
		
		if(currentPlayerIndex + 1 < players.Count)
		{
			currentPlayerIndex++;
			EventManager.PlayerTurnStart(currentPlayer);
		}
		else
		{			
			currentPlayerIndex = 0;

			EventManager.PlayerTurnStart(currentPlayer);
		}
		currentUnitIndex = 0;

		turnsCounter++;

		EventManager.UnitTurnStart(currentUnit);

		if(currentPlayer.type == playerType.ai)
			EventManager.LockUI();
		else
			EventManager.UnlockUI();
	}



	public void selectNextUnit() {
		//End turn event
	    if (matchState == matchStates.battle)
	    {
	        EventManager.UnitTurnEnd(currentUnit);
	    }

		if(currentPlayer.type == playerType.ai){
			if (currentUnitIndex + 1 < currentPlayer.units.Count) {
				currentUnitIndex++;
			} 
			else{
				PlayerEndTurn();
			}
		}
		else if(currentPlayer.type == playerType.player){
			if(matchState == matchStates.battle)
				selectNextUnitWithAP();
			else if(matchState == matchStates.placeUnits){
				if (currentUnitIndex + 1 < currentPlayer.units.Count)
					currentUnitIndex++;
				else
					currentUnitIndex = 0;
			}
		}

		if(matchState == matchStates.battle){
			if(currentUnit.UnitState == unitStates.dead)
			{
				selectNextUnit();
			}
			else
			{
				//Start turn event
				EventManager.UnitTurnStart(currentUnit);
			}
		}
	}

	public void selectNextUnitWithAP(){
		foreach(Unit u in currentPlayer.units)
			if((u.UnitState != unitStates.dead)&&(u.AP > 0)&&(u.CastingDelay == 0)){
				currentUnitIndex = currentPlayer.units.IndexOf(u);
				break;
			}
	}

	void TurnLogic (Unit u)
	{
		if(u.UnitState != unitStates.dead){
			GUImanager.instance.initAbilities ();
			removeTileHighlights ();

			//reset & focus camera
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset = Vector3.zero;
			Camera.main.GetComponent<CameraOrbit> ().pivot = currentUnit.transform;
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;

			if(u.DelayedAbilityReady){
				u.onAbility(u.DelayedAbility);
				u.DelayedAbilityReady = false;
			}
		}
		else
		{
			selectNextUnit();
		}
	}

	public void setSelectionRing(Unit u){
		unitSelection.transform.position = u.transform.position;
		unitSelection.transform.parent = u.transform;
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

		foreach (Tile t in highlightedTiles)
			t.showHighlight(highlightColor);
	}

	public void AttackhighlightTiles(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers,bool addCenterTile = false,float maxHeightDiff = 0.5f) {

				highlightedTiles = new List<Tile> ();
		
				if (ignorePlayers)
			highlightedTiles = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, maxHeightDiff);
				else
			highlightedTiles = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, unitsAll.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray (),maxHeightDiff);
				if(addCenterTile)
					highlightedTiles.Add(map [(int)originLocation.x] [(int)originLocation.y]);
				foreach (Tile t in highlightedTiles) {
					t.showHighlight(highlightColor);
		}
				
		}

	public void AttackhighlightTilesArea(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {
		
		highlightedTilesArea = new List<Tile> ();

		if (ignorePlayers)
			highlightedTilesArea = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance);
		else
			highlightedTilesArea = TileHighlightAtack.FindHighlight (map [(int)originLocation.x] [(int)originLocation.y], distance, unitsAll.Where (x => x.gridPosition != originLocation).Select (x => x.gridPosition).ToArray ());
		
	}
	
	public void removeTileHighlights() {
		GUImanager.instance.hideHighlightRegion();
		GUImanager.instance.hidePath();
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				map[i][j].hideHighlight();
				map[i][j].highlightController.hideContour();
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
			EventManager.UnitPosChanged(currentUnit);
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
			Debug.Log("No target selected");
		}

	    //check Ability distance
		if(_target != null){
		if(checkAbilityRange(ability,unitOwner,_target))
		{
			if(_target != null)
				unitOwner.transform.rotation = getNewLookRotation(unitOwner,_target);
			removeTileHighlights();

			//if hit

				if (GameMath.checkIfAttackSuccesfullyHit(unitOwner,_target)) {
					//damage logic

					int amountOfDamage = 0;

				    if (ability.baseDamage != 0)
				    {
				        amountOfDamage = GameMath.calculateDamage(ability, unitOwner, _target);
				        amountOfDamage = GameMath.ResistToDamage(_target, amountOfDamage, ability);
				    }

				    //log message
					GUImanager.instance.Log.addText("<b>"+unitOwner.unitName+"("+unitOwner.UnitClass+")"+":</b>" + " successfuly used - "+ability.abilityName + " on " + _target.unitName+"("+_target.UnitClass+")" + " for <b><color=red>" + amountOfDamage + " damage</color></b>!");
					unitOwner.playAbility(ability);

					applyAbilityToTarget (ability, _target, amountOfDamage);
					checkIfCastInterrupted(_target);

					FXmanager.instance.createAbilityFX(unitOwner.transform.position,targetUnit.transform.position,ability);

			//if missed
			} else {
					GUImanager.instance.Log.addText(unitOwner.unitName+"("+unitOwner.UnitClass+")" + " missed " + _target.unitName+"("+_target.UnitClass+")" + "!");
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
				GUImanager.instance.Log.addText(_target.unitName+"("+_target.UnitClass+")" + " interrupted and <b><color=red>fails to cast</color></b> " + _target.currentAbility.abilityName);
					Debug.Log ("Cast interrupted!");
					_target.CastingDelay = 0;
					_target.UnitAction = unitActions.idle;
					_target.DelayedAbilityReady = false;
					_target.DelayedAbility = null;
				}
			}
		}

	public void applyAbilityToTarget (BaseAbility ability, Unit target, int amountOfDamage)
	{
		if (ability.attackType == attackTypes.heal) {
			target.takeHeal (amountOfDamage);
		}
		else {
			target.takeDamage (amountOfDamage);
		}

		//Apply Effect
		if(ability.effects.Count > 0)
		{
			foreach(string s in ability.effects){
				BaseEffect ef = BaseEffectsManager.instance.getEffect(s);
                if (ef != null)
                {
                    if (ef.requireTarget)
                    {
                        currentUnit.unitEffects.addEffect(ef, target);
                        GUImanager.instance.Log.addText(ef.name + " <b><color=red>applied on</color></b> " + target.unitName + "(" + target.UnitClass + ")" + " by " + currentUnit.unitName + "(" + currentUnit.UnitClass + ")");
                    }
                    else
                    {
                        currentUnit.unitEffects.addEffect(ef);
                        GUImanager.instance.Log.addText(ef.name + " <b><color=red>applied on</color></b> " + currentUnit.unitName + "(" + currentUnit.UnitClass + ")");
                    }
                }
                else {
                    Debug.Log("No such effect in Effect Manager");
                }

			}
		}
	}

	public bool checkAbilityRange (BaseAbility ability, Unit owner,Unit target)
	{
		if (owner == target)
			return true;

		if (highlightedTiles.Contains (target.currentTile) && currentUnit.currentAbility.areaDamage == true) {
			
			if (owner.gridPosition.x >= target.gridPosition.x - owner.attackRange - currentUnit.currentAbility.areaDamageRadius && owner.gridPosition.x <= target.gridPosition.x + owner.attackRange+ currentUnit.currentAbility.areaDamageRadius &&
			    owner.gridPosition.y >= target.gridPosition.y - owner.attackRange - currentUnit.currentAbility.areaDamageRadius && owner.gridPosition.y <= target.gridPosition.y + owner.attackRange+ currentUnit.currentAbility.areaDamageRadius) 
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
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				map[i][j].generateNeighbors();
			}
		}
	}

	void generateUnits() {
		Unit unit;
		AIPlayer ai;
		for(int i=0; i< unitsCountPlayer;i++)
		{
//			Vector2 position = getRandoMapTileXY();
			unit = ((GameObject)Instantiate(UserUnitPrefab[i],new Vector3(-1000f,-1000f,-1000f),Quaternion.identity)).GetComponent<Unit>();
//			unit.placeUnit(position);
			unit.unitName = "Alice-"+i;
			unit.playerOwner = players[0];
			players[0].addUnit(unit);
			GUImanager.instance.unitPanels[i].Init(unit);
		}

		for(int i=0; i< unitsCountAI;i++)
		{
//			Vector2 position = getRandoMapTileXY();
			ai = ((GameObject)Instantiate(AIPlayerPrefab,new Vector3(-1000f,-1000f,-1000f),Quaternion.identity)).GetComponent<AIPlayer>();
//			ai.placeUnit(position);
			ai.unitName = "Bot-"+i;				
			ai.playerOwner = players[1];
			players[1].addUnit(ai);
		}

	}

	public Vector2 getRandoMapTileXY(List<Tile> tiles = null)
	{
		Vector2 tileXY;
		if(tiles != null)
			tileXY = tiles[Random.Range(0,tiles.Count-1)].gridPosition;
		else
			tileXY = new Vector2(Random.Range(0,mapSize),Random.Range(0,mapSize));

		if ((map[(int)tileXY.x][(int)tileXY.y].impassible == true)||(map[(int)tileXY.x][(int)tileXY.y].unitInTile != null))
		{
			if(tiles != null)
				tileXY = getRandoMapTileXY(tiles);
			else
				tileXY = getRandoMapTileXY();
		}

		return tileXY;
	}

    public void drawPointer(Unit u)
    {
        drawPointer(u.currentTile);
    }
	public void drawPointer(Tile t)
	{
		pointer.SetActive(true);
		pointer.transform.position = (t.transform.position+new Vector3(0,0.5f,0));
		if((currentUnit.UnitAction == unitActions.readyToMove)&&
		   (highlightedTiles.Contains(t))&&
		   (t.impassible == false))
			GUImanager.instance.showPath(TilePathFinder.FindPath(map[(int)currentUnit.gridPosition.x][(int)currentUnit.gridPosition.y],t, unitsAll.Where(x => x.gridPosition != t.gridPosition && x.gridPosition != currentUnit.gridPosition).Select(x => x.gridPosition).ToArray(),currentUnit.maxHeightDiff));
		else
			GUImanager.instance.hidePath();
	}

	void drawArea (Tile t)
	{
        if (currentUnit.currentAbility.areaDamage == true && currentUnit.UnitAction == unitActions.readyToAttack)
        {
            removeTileHighlights();
            AttackhighlightTilesArea(currentUnit.gridPosition, ColorHolder.instance.attack, currentUnit.currentAbility.range, true);
            AttackhighlightTiles(currentUnit.gridPosition, ColorHolder.instance.area, currentUnit.currentAbility.range, true);
            AttackhighlightTiles(t.gridPosition, ColorHolder.instance.area, currentUnit.currentAbility.areaDamageRadius, true, true);
            highlightedTiles.Add(t);
        }

	}

	public void useAbility (Unit u) {
		AttackOnMouseClick(u);
	}

	public void useAbility (Tile t) {
		AttackOnMouseClick(t.unitInTile,t);
	}

	public void AttackOnMouseClick (Unit un = null,Tile t = null) {
		if((currentUnit.UnitAction != unitActions.idle)&&(currentUnit.UnitAction != unitActions.moving)&&(currentUnit.UnitAction != unitActions.readyToMove))
		{
			BaseAbility ability = currentUnit.currentAbility;
			Tile targetTile = null;
			Unit targetUnit = null;

			if(un != null)
				targetUnit = un;
			else
				targetUnit = t.unitInTile;
			if(t != null)
				targetTile = t;
			else
				targetTile = un.currentTile;

			if(ability.requireTarget == true){
			    if (ability.selfUse == true)
			    {
			        if (currentUnit == targetUnit)
			        {
			            GameManager.instance.useAbility(ability, currentUnit, targetTile, targetUnit);
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



	public void checkVictory()
	{
		foreach(Player p in players){ 
			if(p.unitsDead.Count >= p.units.Count){
				EventManager.VictoryState(p);
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

    public void selectUnit(Tile t)
    {
        if ((currentUnit.UnitAction == unitActions.idle) || (currentUnit.UnitAction == unitActions.readyToMove))
        {
            if (t.unitInTile != null)
                selectUnit(t.unitInTile);
        }
    }
	public void selectUnit(Unit u){
	    if ((currentUnit.UnitAction == unitActions.idle) || (currentUnit.UnitAction == unitActions.readyToMove))
	    {
	        if (u.playerOwner == currentPlayer)
	        {
                if (currentUnit.UnitAction == unitActions.readyToMove)
                {
                    currentUnit.UnitAction = unitActions.idle;
                }
	            currentUnit = u;
	            currentUnitIndex = u.playerOwner.units.IndexOf(u);
                if (matchState == matchStates.battle)
                    TurnLogic(currentUnit);
                setSelectionRing(currentUnit);
	        }
	    }
	}

	void OnDestroy(){
		EventManager.OnUnitTurnStart -= TurnLogic;
		EventManager.OnVictoryState -= endMatch;
		EventManager.onTileClick -= PlaceUnit;
		EventManager.onTileCursorOverChanged -= drawPointer;
		EventManager.onTileCursorOverChanged -= drawArea;
		EventManager.onUnitClick -= useAbility;
		EventManager.onTileClick -= useAbility;
		EventManager.onUnitSelectionChanged -= setSelectionRing;
	}
}
