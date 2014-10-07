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
	public GameObject MagicPrefab;
	public GameObject MagicExplosionPrefab;
	public GameObject magic;
	public GameObject selectionRing;
	public Player targetPub;
	public bool Loose = false;
	public Texture ImpasTex;

	public List<Tile> highlightedTiles;
	public bool magiceffect = false;
	
	public int mapSize = 22;
	public Transform mapTransform;
	Transform tileTransform;
	
	public List <List<Tile>> map = new List<List<Tile>>();
	public List <Player> players = new List<Player>();
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
		unitSelection = (GameObject)Instantiate(selectionRing, players[0].transform.position, Quaternion.Euler(0,0,0));
		unitSelection.transform.parent = players [0].transform;
		Camera.main.GetComponent<CameraOrbit>().pivot = players[currentPlayerIndex].transform;
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
		GUImanager.instance.comboButton.AddItems(players[currentPlayerIndex].GetComponent<UnitSkillsManager>().skillsList[0], players[currentPlayerIndex].GetComponent<UnitSkillsManager>().skillsList[1]);
	}
	
	// Update is called once per frame
	void Update () {
		drawPointer();
		if (players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnUpdate();
		else nextTurn();

		AttackOnMouseClick ();
	}
	
	public void nextTurn() {
		if (currentPlayerIndex + 1 < players.Count) {

			removeTileHighlights();

			currentPlayerIndex++;

			Camera.main.GetComponent<CameraOrbit> ().pivotOffset = Vector3.zero;

			unitSelection.transform.position = players [currentPlayerIndex].transform.position;
			unitSelection.transform.parent = players [currentPlayerIndex].transform;

			Camera.main.GetComponent<CameraOrbit>().pivot = players[currentPlayerIndex].transform;
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;

			if (players[currentPlayerIndex].currentUnitState!=unitStates.dead) {
				players[currentPlayerIndex].currentUnitState = unitStates.normal;
				players[currentPlayerIndex].currentUnitAction = unitActions.moving; 
				//GUImanager.instance.comboButton.ClearItems();
				GUImanager.instance.comboButton.AddItems(players[currentPlayerIndex].GetComponent<UnitSkillsManager>().skillsList[0], players[currentPlayerIndex].GetComponent<UnitSkillsManager>().skillsList[1]);
			}

			GameManager.instance.highlightTilesAt(players[currentPlayerIndex].gridPosition, Color.blue, players[currentPlayerIndex].movementPerActionPoint, false);
		} else {
			currentPlayerIndex = 0;

			removeTileHighlights();

			Camera.main.GetComponent<CameraOrbit>().pivot = players[currentPlayerIndex].transform;
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;

			unitSelection.transform.position = players [currentPlayerIndex].transform.position;
			unitSelection.transform.parent = players [currentPlayerIndex].transform;

			if (players[currentPlayerIndex].currentUnitState!=unitStates.dead) {
			players[currentPlayerIndex].currentUnitState = unitStates.normal;
			players[currentPlayerIndex].currentUnitAction = unitActions.moving;
				//GUImanager.instance.comboButton.ClearItems();
				GUImanager.instance.comboButton.AddItems(players[currentPlayerIndex].GetComponent<UnitSkillsManager>().skillsList[0], players[currentPlayerIndex].GetComponent<UnitSkillsManager>().skillsList[1]);
			}
			GameManager.instance.highlightTilesAt(players[currentPlayerIndex].gridPosition, Color.blue, players[currentPlayerIndex].movementPerActionPoint, false);
		}
	}

	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance) {
		highlightTilesAt(originLocation, highlightColor, distance, true);
	}

	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {

		highlightedTiles = new List<Tile>();

		if (ignorePlayers) highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance);
		else highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance, players.Where(x => x.gridPosition != originLocation).Select(x => x.gridPosition).ToArray());
		
		foreach (Tile t in highlightedTiles) {
			t.visual.transform.renderer.materials[1].color = highlightColor;
		}
	}

	public void AttackhighlightTiles(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {
		
		highlightedTiles = new List<Tile>();
		
		if (ignorePlayers) highlightedTiles = TileHighlightAtack.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance);
		else highlightedTiles = TileHighlightAtack.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance, players.Where(x => x.gridPosition != originLocation).Select(x => x.gridPosition).ToArray());

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
			if ((highlightedTiles.Contains(destTile)) && !destTile.impassible && players[currentPlayerIndex].positionQueue.Count == 0) {
			removeTileHighlights();
			players[currentPlayerIndex].currentUnitAction = unitActions.moving;
			foreach(Tile t in TilePathFinder.FindPath(map[(int)players[currentPlayerIndex].gridPosition.x][(int)players[currentPlayerIndex].gridPosition.y],destTile, players.Where(x => x.gridPosition != destTile.gridPosition && x.gridPosition != players[currentPlayerIndex].gridPosition).Select(x => x.gridPosition).ToArray())) {
				players[currentPlayerIndex].positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position + 0.5f * Vector3.up);
				Debug.Log("(" + players[currentPlayerIndex].positionQueue[players[currentPlayerIndex].positionQueue.Count - 1].x + "," + players[currentPlayerIndex].positionQueue[players[currentPlayerIndex].positionQueue.Count - 1].y + ")");
			}			
			players[currentPlayerIndex].gridPosition = destTile.gridPosition;

		} else {
			Debug.Log ("destination invalid");
		}
	}
	
	public void attackWithCurrentPlayer(Tile destTile) {
		if ((highlightedTiles.Contains(destTile)) && !destTile.impassible) {
			
			Player target = null;
			foreach (Player p in players) {
				if (p.gridPosition == destTile.gridPosition) {
					target = p;

				}
			}

			if (target != null) {

				var newRotation = Quaternion.LookRotation((target.transform.position - players[currentPlayerIndex].transform.position).normalized);
				players[currentPlayerIndex].transform.rotation = Quaternion.Slerp(players[currentPlayerIndex].transform.rotation, newRotation, 1);

				if (players[currentPlayerIndex].gridPosition.x >= target.gridPosition.x - players[currentPlayerIndex].attackRange && players[currentPlayerIndex].gridPosition.x <= target.gridPosition.x + players[currentPlayerIndex].attackRange &&
				    players[currentPlayerIndex].gridPosition.y >= target.gridPosition.y - players[currentPlayerIndex].attackRange && players[currentPlayerIndex].gridPosition.y <= target.gridPosition.y + players[currentPlayerIndex].attackRange) {

					
					removeTileHighlights();
								

					players[currentPlayerIndex].animation.Play("Attack");
					StartCoroutine(WaitAndCallback(players[currentPlayerIndex].animation["Attack"].length));
					bool hit = Random.Range(0.0f, 1.0f) <= players[currentPlayerIndex].attackChance;

					
					if (hit) {
						//damage logic
						int amountOfDamage = (int)Mathf.Floor(players[currentPlayerIndex].damageBase + Random.Range(0, players[currentPlayerIndex].damageRollSides));
						
						target.HP -= amountOfDamage;

						target.animation.Play("Damage");
						if (target.HP <= 0) {
							target.animation.CrossFade("Death", 1f);
						} else {
							target.animation.CrossFade("Idle", 1f);
						}
						Debug.Log(players[currentPlayerIndex].playerName + " successfuly hit " + target.playerName + " for " + amountOfDamage + " damage!");
					} else {
						Debug.Log(players[currentPlayerIndex].playerName + " missed " + target.playerName + "!");
						target.animation.Play("Avoid");
						target.animation.CrossFade("Idle", 1f);
					}
				} else {
					Debug.Log ("Target is not adjacent!");
				}

				players[currentPlayerIndex].animation.CrossFade("Idle", 1f);
			}
		} else {
			Debug.Log ("target invalid");
		}
	}


	public void distanceAttackWithCurrentPlayer(Tile destTile) {
		if ((highlightedTiles.Contains(destTile)) && !destTile.impassible) {
			
			Player target = null;
			foreach (Player p in players) {
				if (p.gridPosition == destTile.gridPosition) {
					target = p;
					targetPub = p;
				}
			}
			
			if (target != null) {

				var newRotation = Quaternion.LookRotation((target.transform.position - players[currentPlayerIndex].transform.position).normalized);
				//if (players[currentPlayerIndex].currentUnitAction == unitActions.magicAttack)
//					Debug.Log("/MagicPrefabs/"+MagicPrefab);

					//magic = ((GameObject)Instantiate(Resources.Load<GameObject>("MagicPrefabs/"+MagicPrefab), players[currentPlayerIndex].transform.position+0.5f*Vector3.up, Quaternion.Euler(0,0,0)));
				magic = ((GameObject)Instantiate(MagicPrefab, players[currentPlayerIndex].transform.position+0.5f*Vector3.up, Quaternion.identity));

				players[currentPlayerIndex].transform.rotation = Quaternion.Slerp(players[currentPlayerIndex].transform.rotation, newRotation, 1);

				if (players[currentPlayerIndex].gridPosition.x >= target.gridPosition.x - players[currentPlayerIndex].attackDistance && players[currentPlayerIndex].gridPosition.x <= target.gridPosition.x + players[currentPlayerIndex].attackDistance &&
				    players[currentPlayerIndex].gridPosition.y >= target.gridPosition.y - players[currentPlayerIndex].attackDistance && players[currentPlayerIndex].gridPosition.y <= target.gridPosition.y + players[currentPlayerIndex].attackDistance) {

					
					removeTileHighlights();
							
					
					players[currentPlayerIndex].animation.Play("Attack");
					StartCoroutine(WaitAndCallback(players[currentPlayerIndex].animation["Attack"].length));

					players[currentPlayerIndex].MP -= AbilitiesManager.instance.getAbility("baseMagic").MPCost;
					//attack logic
					//roll to hit
					bool hit = Random.Range(0.0f, 1.0f) <= players[currentPlayerIndex].attackChance;

					if (hit) {

						//damage types goes here
					
						magic.transform.DOMove(target.transform.position+1.0f*Vector3.up, 1f).OnComplete(MoveCompleted);
						magiceffect = true;
						//damage logic
						int amountOfDamage = (int)Mathf.Floor(players[currentPlayerIndex].damageBase + Random.Range(0, players[currentPlayerIndex].damageRollSides));

						target.HP -= amountOfDamage;

						Debug.Log(players[currentPlayerIndex].playerName + " successfuly hit " + target.playerName + " for " + amountOfDamage + " damage!");
					} else {
						magic.transform.DOMove(target.transform.position+1.0f*Vector3.up, 1f).OnComplete(MoveCompleted);
						Debug.Log(players[currentPlayerIndex].playerName + " missed " + target.playerName + "!");

					}
				} else {
					Debug.Log ("Target is not adjacent!");
				}

				players[currentPlayerIndex].animation.CrossFade("Idle", 1f);

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
			player.playerName = "Alice-"+i;				
			players.Add(player);
		}

		for(int i=0; i< unitsCountPlayer;i++)
		{
			Vector2 position = getRandoMapTileXY(true);
			ai = ((GameObject)Instantiate(AIPlayerPrefab,Vector3.zero,Quaternion.identity)).GetComponent<AIPlayer>();
			ai.gridPosition = position;
			ai.transform.position = map[(int)position.x][(int)position.y].transform.position + new Vector3(0,0.5f,0);
			ai.playerName = "Bot-"+i;				
			players.Add(ai);
		}
	}

	public void Explode (Player target, GameObject magictodestroy) {

		GameObject magicExposion = ((GameObject)Instantiate(MagicExplosionPrefab, target.transform.position+0.5f*Vector3.up, Quaternion.Euler(0,0,0)));
		Debug.Log ("explosion");
		Destroy (magictodestroy);
		magiceffect = false;
		if (target.HP > 0) {
						target.animation.Play ("Damage");
						StartCoroutine (WaitAndNextTurn (target.animation ["Damage"].length));
						target.animation.CrossFade ("Idle", 2f);
		}
				

	}

	public void MoveCompleted (){
		Explode(targetPub, magic);
	}

	IEnumerator WaitAndNextTurn(float waitTime){

		yield return new WaitForSeconds(waitTime);
		nextTurn ();

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

		for (int i =0; i<players.Count; i++) {
			if (map[(int)tileXY.x][(int)tileXY.y].gridPosition == players[i].gridPosition){
				tileXY = getRandoMapTileXY();
			}		
		}
		return tileXY;
	}

	public void drawPointer()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask mask = 1<<LayerMask.NameToLayer("tiles");

		if(Physics.Raycast(ray,out hit,1000f,mask))
		{
			if(hit.transform.gameObject.GetComponent<Tile>() != null)
			{
				Tile t = hit.transform.gameObject.GetComponent<Tile>();
				pointer.transform.position = Vector3.Lerp(pointer.transform.position,(t.transform.position+new Vector3(0,0.5f,0)),0.5f);
			}
		}
	}

	public void AttackOnMouseClick () {
				
		if ((Input.GetMouseButtonDown(0))&&(!GUImanager.instance.mouseOverGUI))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(ray,out hit))
			{
				if ((hit.collider.gameObject.GetComponent<AIPlayer>() != null)&&(hit.collider.gameObject.GetComponent<AIPlayer> ().currentUnitState != unitStates.dead)){
					ckeckLineofSign(hit.collider.gameObject.GetComponent<AIPlayer>());
					if (players[currentPlayerIndex].currentUnitAction == unitActions.rangedAttack) {
					GameManager.instance.distanceAttackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.y]);
					}
					if (players[currentPlayerIndex].currentUnitAction == unitActions.meleeAttack) {
					GameManager.instance.attackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.y]);
					}
					if (players[currentPlayerIndex].currentUnitAction == unitActions.magicAttack) {
						GameManager.instance.distanceAttackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.y]);
					}

				}
			}
		}

	}

	public void ckeckLineofSign(AIPlayer ai)
	{

		if(Physics.Raycast(players[currentPlayerIndex].transform.position+new Vector3(0,0.5f,0),players[currentPlayerIndex].transform.position+new Vector3(0,0.5f,0)-ai.transform.position,out target,100f))
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
