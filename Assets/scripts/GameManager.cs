using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EnumSpace;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	//units count
	public int unitsCountPlayer;
	public int unitsCountAI;
	//prefabs
	public GameObject TilePrefab;
	public GameObject[] UserPlayerPrefab;
	public GameObject AIPlayerPrefab;
	//public string MagicPrefab;
	//public string MagicExplosionPrefab;
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
	public Transform mapTransform;
	Transform tileTransform;
	
	public List <List<Tile>> map = new List<List<Tile>>();
	public List <Unit> units = new List<Unit>();
	public int currentUnitIndex = 0;
	public int currentPlayerIndex = 0;

	public GameObject pointer;

	private RaycastHit hit;
	private RaycastHit target;
	private GameObject unitSelection;

	void Awake() {
		instance = this;

		mapTransform = transform.FindChild("Map");
	}

	// Use this for initialization
	void Start () {		
		generateMap();
		generatePlayers();
		unitSelection = (GameObject)Instantiate(selectionRing, units[0].transform.position, Quaternion.Euler(0,0,0));
		unitSelection.transform.parent = units [0].transform;
		Camera.main.GetComponent<CameraOrbit>().pivot = units[currentUnitIndex].transform;
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
	}
	
	// Update is called once per frame
	void Update () {
		drawPointer();
		AttackOnMouseClick ();
	}

	public void Turn(){

	}

	public void nextTurn() {
		if (currentUnitIndex + 1 < units.Count) {
			currentUnitIndex++;
		} 
		else {
			currentUnitIndex = 0;
		}
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
				if (!map[i][j].impassible) map[i][j].visual.transform.renderer.materials[1].color = Color.white;
			}
		}
	}
 	
	public void moveCurrentPlayer(Tile destTile) {
			if ((highlightedTiles.Contains(destTile)) && !destTile.impassible && units[currentUnitIndex].positionQueue.Count == 0) {
			removeTileHighlights();
			units[currentUnitIndex].UnitAction = unitActions.moving;
			foreach(Tile t in TilePathFinder.FindPath(map[(int)units[currentUnitIndex].gridPosition.x][(int)units[currentUnitIndex].gridPosition.y],destTile, units.Where(x => x.gridPosition != destTile.gridPosition && x.gridPosition != units[currentUnitIndex].gridPosition).Select(x => x.gridPosition).ToArray())) {
				units[currentUnitIndex].positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position + 0.5f * Vector3.up);
				Debug.Log("(" + units[currentUnitIndex].positionQueue[units[currentUnitIndex].positionQueue.Count - 1].x + "," + units[currentUnitIndex].positionQueue[units[currentUnitIndex].positionQueue.Count - 1].y + ")");
			}			
			units[currentUnitIndex].gridPosition = destTile.gridPosition;

		} else {
			Debug.Log ("destination invalid");
		}
	}
	
	public void attackWithCurrentPlayer(Tile destTile) {
		if ((highlightedTiles.Contains(destTile)) && !destTile.impassible) {
			
			Unit target = null;
			foreach (Unit p in units) {
				if (p.gridPosition == destTile.gridPosition) {
					target = p;

				}
			}

			if (target != null) {

				var newRotation = Quaternion.LookRotation((target.transform.position - units[currentUnitIndex].transform.position).normalized);
				units[currentUnitIndex].transform.rotation = Quaternion.Slerp(units[currentUnitIndex].transform.rotation, newRotation, 1);

				if (units[currentUnitIndex].gridPosition.x >= target.gridPosition.x - units[currentUnitIndex].attackRange && units[currentUnitIndex].gridPosition.x <= target.gridPosition.x + units[currentUnitIndex].attackRange &&
				    units[currentUnitIndex].gridPosition.y >= target.gridPosition.y - units[currentUnitIndex].attackRange && units[currentUnitIndex].gridPosition.y <= target.gridPosition.y + units[currentUnitIndex].attackRange) {

					removeTileHighlights();
								
					units[currentUnitIndex].animation.Play("Attack");
					StartCoroutine(WaitAndCallback(units[currentUnitIndex].animation["Attack"].length));
					bool hit = Random.Range(0.0f, 1.0f) <= units[currentUnitIndex].attackChance;

					
					if (hit) {
						//damage logic
						int amountOfDamage = (int)Mathf.Floor(units[currentUnitIndex].damageBase + Random.Range(0, units[currentUnitIndex].damageRollSides));
						
						target.HP -= amountOfDamage;

						target.animation.Play("Damage");
						if (target.HP <= 0) {
							target.makeDead();
//							target.animation.CrossFade("Death", 1f);
						} else {
							target.animation.CrossFade("Idle", 1f);
						}
						Debug.Log(units[currentUnitIndex].unitName + " successfuly hit " + target.unitName + " for " + amountOfDamage + " damage!");
					} else {
						Debug.Log(units[currentUnitIndex].unitName + " missed " + target.unitName + "!");
						target.animation.Play("Damage");
						target.animation.CrossFade("Idle", 1f);
					}
				} else {
					Debug.Log ("Target is not adjacent!");
				}

				units[currentUnitIndex].animation.CrossFade("Idle", 1f);

				units[currentUnitIndex].actionPoints--;
				units[currentUnitIndex].checkAP();
			}
		} else {
			Debug.Log ("target invalid");
		}
	}


	public void distanceAttackWithCurrentPlayer(Tile destTile) {
		if ((highlightedTiles.Contains(destTile)) && !destTile.impassible) {
			
			Unit target = null;
			foreach (Unit p in units) {
				if (p.gridPosition == destTile.gridPosition) {
					target = p;
					targetPub = p;
				}
			}
			
			if (target != null && (target.UnitState != unitStates.dead)) {

				var newRotation = Quaternion.LookRotation((target.transform.position - units[currentUnitIndex].transform.position).normalized);

				magic = ((GameObject)Instantiate(MagicPrefab, units[currentUnitIndex].transform.position+0.5f*Vector3.up, Quaternion.identity));

				units[currentUnitIndex].transform.rotation = Quaternion.Slerp(units[currentUnitIndex].transform.rotation, newRotation, 1);

				if (units[currentUnitIndex].gridPosition.x >= target.gridPosition.x - units[currentUnitIndex].attackDistance && units[currentUnitIndex].gridPosition.x <= target.gridPosition.x + units[currentUnitIndex].attackDistance &&
				    units[currentUnitIndex].gridPosition.y >= target.gridPosition.y - units[currentUnitIndex].attackDistance && units[currentUnitIndex].gridPosition.y <= target.gridPosition.y + units[currentUnitIndex].attackDistance) {

					
					removeTileHighlights();
							
					
					units[currentUnitIndex].animation.Play("Attack");
					StartCoroutine(WaitAndCallback(units[currentUnitIndex].animation["Attack"].length));

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

						target.HP -= amountOfDamage;

						Debug.Log(units[currentUnitIndex].unitName + " successfuly hit " + target.unitName + " for " + amountOfDamage + " damage!");
					} else {
						magic.transform.DOMove(target.transform.position+1.0f*Vector3.up, 1f).OnComplete(MoveCompleted);
						Debug.Log(units[currentUnitIndex].unitName + " missed " + target.unitName + "!");

					}
				} else {
					Debug.Log ("Target is not adjacent!");
				}

				units[currentUnitIndex].animation.CrossFade("Idle", 1f);

				units[currentUnitIndex].actionPoints--;
				units[currentUnitIndex].checkAP();
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
				Tile tile = ((GameObject)Instantiate(PrefabHolder.instance.BASE_TILE_PREFAB, new Vector3(i - Mathf.Floor(mapSize/2),0, -j + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();

				tile.transform.parent = mapTransform;
				tile.gridPosition = new Vector2(i, j);
				tile.setType((TileType)container.tiles.Where(x => x.locX == i && x.locY == j).First().id);
				 
				if (tile.type == TileType.Difficult) {
					int height = 1;
					Vector3 temp = new Vector3(0, height,0 );
					tile.transform.position += temp;
				}

				if (tile.type == TileType.VeryDifficult) {
					int height = 2;
					Vector3 temp = new Vector3(0, height,0 );
					tile.transform.position += temp;
				}

				if (tile.type == TileType.Impassible) {

					Vector3 temp = new Vector3(0, -1,0 );
					tile.transform.position += temp;
				}
				row.Add (tile);
			}
			map.Add(row);

		}
	}

	void generatePlayers() {
		UserPlayer player;
		AIPlayer ai;
		for(int i=0; i< unitsCountPlayer;i++)
		{
			Vector2 position = getRandoMapTileXY(true);
			player = ((GameObject)Instantiate(UserPlayerPrefab[i],Vector3.zero,Quaternion.identity)).GetComponent<UserPlayer>();
			player.gridPosition = position;
			player.transform.position = map[(int)position.x][(int)position.y].transform.position + new Vector3(0,0.5f,0);
			player.unitName = "Alice-"+i;				
			units.Add(player);
		}

		for(int i=0; i< unitsCountPlayer;i++)
		{
			Vector2 position = getRandoMapTileXY(true);
			ai = ((GameObject)Instantiate(AIPlayerPrefab,Vector3.zero,Quaternion.identity)).GetComponent<AIPlayer>();
			ai.gridPosition = position;
			ai.transform.position = map[(int)position.x][(int)position.y].transform.position + new Vector3(0,0.5f,0);
			ai.unitName = "Bot-"+i;				
			units.Add(ai);
		}
	}

	public void Explode (Unit target, GameObject magictodestroy) {

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

	IEnumerator WaitAndCallback(float waitTime){
		
		yield return new WaitForSeconds(waitTime);

		
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
}
