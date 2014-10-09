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
	public bool Loose = false;
	public Texture ImpasTex;

	public List<Tile> highlightedTiles;
	public bool magiceffect = false;
	
	public int mapSize = 22;
	public float maxHeighDiff = 2;
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

		mapTransform = transform.FindChild("Map");
		generateMap();
		generateUnits();
	}

	// Use this for initialization
	void Start () {		

		unitSelection = (GameObject)Instantiate(selectionRing, units[0].transform.position, Quaternion.Euler(0,0,0));
		unitSelection.transform.parent = units [0].transform;
		Camera.main.GetComponent<CameraOrbit>().pivot = units[currentUnitIndex].transform;
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
		//reset AP
		units[0].actionPoints = units[0].maxActionPoints;
	}
	
	// Update is called once per frame
	void Update () {
		drawPointer();
		if(currentUnit.UnitAction != unitActions.idle)
			AttackOnMouseClick ();
	}

	public void Turn(){

	}

	public void nextTurn() {
		if (currentUnitIndex + 1 < units.Count) {
			currentUnitIndex++;
		} 
		else {
			turnsCounter++;
			currentUnitIndex = 0;
		}
		GUImanager.instance.showAbilities();
		//reset AP
		units[currentUnitIndex].actionPoints = units[currentUnitIndex].maxActionPoints;
		removeTileHighlights();

		//reset & focus camera
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset = Vector3.zero;
		Camera.main.GetComponent<CameraOrbit>().pivot = units[currentUnitIndex].transform;
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
		//set selection ring
		unitSelection.transform.position = units [currentUnitIndex].transform.position;
		unitSelection.transform.parent = units [currentUnitIndex].transform;
		//set state
		if (units[currentUnitIndex].UnitState!=unitStates.dead) {
			units[currentUnitIndex].UnitAction = unitActions.idle;
		}
	}

	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance) {
		highlightTilesAt(originLocation, highlightColor, distance, true);
	}

	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {

		highlightedTiles = new List<Tile>();

		if (ignorePlayers) highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance);
		else highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance, units.Where(x => x.gridPosition != originLocation).Select(x => x.gridPosition).ToArray());
		
		foreach (Tile t in highlightedTiles) {
			t.visual.transform.renderer.materials[1].color = highlightColor;
		}
	}

	public void AttackhighlightTiles(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {
		
		highlightedTiles = new List<Tile>();
		
		if (ignorePlayers) highlightedTiles = TileHighlightAtack.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance);
		else highlightedTiles = TileHighlightAtack.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance, units.Where(x => x.gridPosition != originLocation).Select(x => x.gridPosition).ToArray());

		foreach (Tile t in highlightedTiles) 
			t.visual.transform.renderer.materials[1].color = highlightColor;
		}

	
	public void removeTileHighlights() {
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				map[i][j].visual.transform.renderer.materials[1].color = Color.white;
			}
		}
	}
 	
	public void moveCurrentPlayer(Tile destTile) {
			if ((highlightedTiles.Contains(destTile)) && !destTile.impassible && units[currentUnitIndex].positionQueue.Count == 0) {
			removeTileHighlights();
			units[currentUnitIndex].UnitAction = unitActions.moving;
			units[currentUnitIndex].currentTile.unitInTile = null;
			foreach(Tile t in TilePathFinder.FindPath(map[(int)units[currentUnitIndex].gridPosition.x][(int)units[currentUnitIndex].gridPosition.y],destTile, units.Where(x => x.gridPosition != destTile.gridPosition && x.gridPosition != units[currentUnitIndex].gridPosition).Select(x => x.gridPosition).ToArray())) {
				units[currentUnitIndex].positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position + 0.5f * Vector3.up);
				Debug.Log("(" + units[currentUnitIndex].positionQueue[units[currentUnitIndex].positionQueue.Count - 1].x + "," + units[currentUnitIndex].positionQueue[units[currentUnitIndex].positionQueue.Count - 1].y + ")");
			}			
			units[currentUnitIndex].gridPosition = destTile.gridPosition;
			destTile.unitInTile = units[currentUnitIndex];

		} else {
			Debug.Log ("destination invalid");
		}
	}
	
	public void attackWithCurrentPlayer(Tile destTile) {
		if ((highlightedTiles.Contains(destTile)) && !destTile.impassible) {
			
			Unit target = destTile.unitInTile;
			if(currentUnit.actionPoints > 0){
			if (target != null && (target.UnitState != unitStates.dead) && (!players[currentPlayerIndex].units.Contains(target))) {
				targetPub = target;
				Vector3 targetPos = target.transform.position;
				targetPos.y = 0;
				Vector3 attackerPos = currentUnit.transform.position;
				attackerPos.y = 0;
				
				Quaternion newRotation = Quaternion.LookRotation(targetPos - attackerPos);

				units[currentUnitIndex].transform.rotation = Quaternion.Slerp(units[currentUnitIndex].transform.rotation, newRotation, 1);

				if (units[currentUnitIndex].gridPosition.x >= target.gridPosition.x - units[currentUnitIndex].attackRange && units[currentUnitIndex].gridPosition.x <= target.gridPosition.x + units[currentUnitIndex].attackRange &&
				    units[currentUnitIndex].gridPosition.y >= target.gridPosition.y - units[currentUnitIndex].attackRange && units[currentUnitIndex].gridPosition.y <= target.gridPosition.y + units[currentUnitIndex].attackRange) {

					removeTileHighlights();

					currentUnit.Attack(target);		

					bool hit = Random.Range(0.0f, 1.0f) <= units[currentUnitIndex].attackChance;

					
					if (hit) {
						//damage logic
						int amountOfDamage = (int)Mathf.Floor(units[currentUnitIndex].damageBase + Random.Range(0, units[currentUnitIndex].damageRollSides));

						target.takeDamage(amountOfDamage);

						Debug.Log(units[currentUnitIndex].unitName + " successfuly hit " + target.unitName + " for " + amountOfDamage + " damage!");
					} else {
						Debug.Log(units[currentUnitIndex].unitName + " missed " + target.unitName + "!");
					}
				} else {
					Debug.Log ("Target is not adjacent!");
				}

				units[currentUnitIndex].animation.CrossFade("Idle", 1f);
			}
		}
		} else {
			Debug.Log ("target invalid");
		}
	}


	public void distanceAttackWithCurrentPlayer(Tile destTile) {
		if ((highlightedTiles.Contains(destTile)) && !destTile.impassible) {

			Unit target = destTile.unitInTile;
			if(currentUnit.actionPoints > 0){

			if (units[currentUnitIndex].UnitAction != unitActions.healAttack) {

			if (target != null && (target.UnitState != unitStates.dead) && (!players[currentPlayerIndex].units.Contains(target))) {
				targetPub = target;

				Vector3 targetPos = target.transform.position;
				targetPos.y = 0;
				Vector3 attackerPos = currentUnit.transform.position;
				attackerPos.y = 0;
				Quaternion newRotation = Quaternion.LookRotation(targetPos - attackerPos);
				units[currentUnitIndex].transform.rotation = Quaternion.Slerp(units[currentUnitIndex].transform.rotation, newRotation, 1);

				
				magic = ((GameObject)Instantiate(MagicPrefab, units[currentUnitIndex].transform.position+0.5f*Vector3.up, Quaternion.identity));

				if (units[currentUnitIndex].gridPosition.x >= target.gridPosition.x - units[currentUnitIndex].attackDistance && units[currentUnitIndex].gridPosition.x <= target.gridPosition.x + units[currentUnitIndex].attackDistance &&
				    units[currentUnitIndex].gridPosition.y >= target.gridPosition.y - units[currentUnitIndex].attackDistance && units[currentUnitIndex].gridPosition.y <= target.gridPosition.y + units[currentUnitIndex].attackDistance) {

					
					removeTileHighlights();
							
					currentUnit.Attack(target);	

					units[currentUnitIndex].MP -= AbilitiesManager.instance.getAbility("baseMagic").MPCost;
					//attack logic
					//roll to hit
					bool hit = Random.Range(0.0f, 1.0f) <= units[currentUnitIndex].attackChance;

					if (hit) {

						//damage types goes here
					
						magic.transform.DOMove(target.transform.position+1.0f*Vector3.up, 1f).OnComplete(MoveCompleted);
						magiceffect = true;
						//damage logic
						int amountOfDamage = (int)Mathf.Floor(units[currentUnitIndex].damageBase + Random.Range(0, units[currentUnitIndex].damageRollSides));

						target.takeDamage(amountOfDamage);

						Debug.Log(units[currentUnitIndex].unitName + " successfuly hit " + target.unitName + " for " + amountOfDamage + " damage!");
					} else {
						magic.transform.DOMove(target.transform.position+1.0f*Vector3.up, 1f).OnComplete(MoveCompleted);
						Debug.Log(units[currentUnitIndex].unitName + " missed " + target.unitName + "!");

					}
				} else {
					Debug.Log ("Target is not adjacent!");
				}

				units[currentUnitIndex].animation.CrossFade("Idle", 1f);
			}
				} else {
					//Heal
					Debug.Log("Heal");
					if (target != null && (target.UnitState != unitStates.dead) && (players[currentPlayerIndex].units.Contains(target))) {
						targetPub = target;
						Vector3 targetPos = target.transform.position;
						targetPos.y = 0;
						Vector3 attackerPos = currentUnit.transform.position;
						attackerPos.y = 0;
						
						Quaternion newRotation = Quaternion.LookRotation(targetPos - attackerPos);
						
						magic = ((GameObject)Instantiate(MagicPrefab, units[currentUnitIndex].transform.position+0.5f*Vector3.up, Quaternion.identity));
						
						units[currentUnitIndex].transform.rotation = Quaternion.Slerp(units[currentUnitIndex].transform.rotation, newRotation, 1);
						
						if (units[currentUnitIndex].gridPosition.x >= target.gridPosition.x - units[currentUnitIndex].attackDistance && units[currentUnitIndex].gridPosition.x <= target.gridPosition.x + units[currentUnitIndex].attackDistance &&
						    units[currentUnitIndex].gridPosition.y >= target.gridPosition.y - units[currentUnitIndex].attackDistance && units[currentUnitIndex].gridPosition.y <= target.gridPosition.y + units[currentUnitIndex].attackDistance) {
							
							
							removeTileHighlights();
							
							currentUnit.Attack(target);	
							
							units[currentUnitIndex].MP -= AbilitiesManager.instance.getAbility("baseMagic").MPCost;
							//attack logic
							//roll to hit
							bool hit = Random.Range(0.0f, 1.0f) <= units[currentUnitIndex].attackChance;
							
							if (hit) {
								
								//damage types goes here
								
								magic.transform.DOMove(units[currentUnitIndex].transform.position+1.0f*Vector3.up, 1f).OnComplete(MoveCompleted);
								magiceffect = true;
								//damage logic
								int amountOfDamage = (int)Mathf.Floor(units[currentUnitIndex].damageBase + Random.Range(0, units[currentUnitIndex].damageRollSides));
								
								target.takeHeal(amountOfDamage);
								
								Debug.Log(units[currentUnitIndex].unitName + " successfuly hit " + target.unitName + " for " + amountOfDamage + " damage!");
							} else {
								magic.transform.DOMove(target.transform.position+1.0f*Vector3.up, 1f).OnComplete(MoveCompleted);
								Debug.Log(units[currentUnitIndex].unitName + " missed " + target.unitName + "!");
								
							}
						} else {
							Debug.Log ("Target is not adjacent!");
						}
						
						units[currentUnitIndex].animation.CrossFade("Idle", 1f);
					}
				}
			}
		} else {
			Debug.Log ("destination invalid");
		}
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

				Vector3 temp = new Vector3(0,tile.height,0 );
				tile.transform.position += temp;

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
			units.Add(unit);
			players[0].addUnit(unit);
		}

		for(int i=0; i< unitsCountAI;i++)
		{
			Vector2 position = getRandoMapTileXY(true);
			ai = ((GameObject)Instantiate(AIPlayerPrefab,Vector3.zero,Quaternion.identity)).GetComponent<AIPlayer>();
			ai.placeUnit(position);
			ai.unitName = "Bot-"+i;				
			units.Add(ai);
			players[1].addUnit(ai);
		}

	}

	public void Explode (Unit target, GameObject magictodestroy) {

		Debug.Log(MagicExplosionPrefab.name);
		Debug.Log(target.name);
		GameObject magicExposion = ((GameObject)Instantiate(MagicExplosionPrefab, target.transform.position+0.5f*Vector3.up, Quaternion.identity));
		Debug.Log ("explosion");
		Destroy (magictodestroy);
		magiceffect = false;
		if (target.HP > 0) {
						target.animation.Play ("Damage");
						StartCoroutine (WaitAnimationEnd (target.animation ["Damage"].length));
						target.animation.CrossFade ("Idle", 2f);
		}
		else
		{
			target.makeDead();
		}
	}

	public void MoveCompleted (){
		Explode(targetPub, magic);
	}

	IEnumerator WaitAnimationEnd(float waitTime){

		yield return new WaitForSeconds(waitTime);
		units[currentUnitIndex].checkAP();

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
	
	public void AttackOnMouseClick () {
				
		if ((Input.GetMouseButtonDown(0))&&(!GUImanager.instance.mouseOverGUI))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(ray,out hit))
			{
				if ((hit.collider.gameObject.GetComponent<AIPlayer>() != null)&&(hit.collider.gameObject.GetComponent<AIPlayer> ().UnitState != unitStates.dead)){
					ckeckLineofSign(hit.collider.gameObject.GetComponent<AIPlayer>());
					if (units[currentUnitIndex].UnitAction == unitActions.rangedAttack) {
					GameManager.instance.distanceAttackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.y]);
					}
					if (units[currentUnitIndex].UnitAction == unitActions.meleeAttack) {
					GameManager.instance.attackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.y]);
					}
					if (units[currentUnitIndex].UnitAction == unitActions.magicAttack) {
						GameManager.instance.distanceAttackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.y]);
					}
					if (units[currentUnitIndex].UnitAction == unitActions.healAttack) {
						GameManager.instance.distanceAttackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<Unit> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<Unit> ().gridPosition.y]);
					}

				}
			}
		}

	}

	public void ckeckLineofSign(AIPlayer ai)
	{

		if(Physics.Raycast(units[currentUnitIndex].transform.position+new Vector3(0,0.5f,0),units[currentUnitIndex].transform.position+new Vector3(0,0.5f,0)-ai.transform.position,out target,100f))
		{
			if(target.transform == ai.transform)
			{
				Debug.Log("Line of sign-"+target.transform.name);
				Debug.Break();
			}
			else
				Debug.Log(target.transform.name);
		}
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
