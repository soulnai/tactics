using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public int unitsCountPlayer;
	public int unitsCountAI;
	public RaycastHit hit;
	public GameObject TilePrefab;
	public GameObject UserPlayerPrefab;
	public GameObject AIPlayerPrefab;
	public GameObject MagicPrefab;
	public GameObject MagicExplosionPrefab;
	public GameObject magic;
	public GameObject selectionring;
	public Player targetPub;
	public bool Loose = false;
	public Texture ImpasTex;
	public GameObject unitselection;
	public List<Tile> highlightedTiles;
	public bool magiceffect = false;
	
	public int mapSize = 22;
	public Transform mapTransform;
	Transform tileTransform;
	
	public List <List<Tile>> map = new List<List<Tile>>();
	public List <Player> players = new List<Player>();
	public int currentPlayerIndex = 0;
	
	void Awake() {
		instance = this;

		mapTransform = transform.FindChild("Map");
	}

	// Use this for initialization
	void Start () {		
		generateMap();
		generatePlayers();
		unitselection = (GameObject)Instantiate(selectionring, players[0].transform.position, Quaternion.Euler(0,0,0));
		unitselection.transform.parent = players [0].transform;
		Camera.main.GetComponent<CameraOrbit>().pivot = players[currentPlayerIndex].transform;
		Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
//		Camera.main.GetComponent<CameraOrbit> ().pivot = mapTransform.transform;
//		Camera.main.GetComponent<CameraOrbit> ().pivotOffset.x += 17;
//		Camera.main.GetComponent<CameraOrbit> ().pivotOffset.y += 20;
//		Camera.main.GetComponent<CameraOrbit> ().pivotOffset.z += -10;
//
	//	Camera.main.GetComponent<CameraOrbit>().pivot = players[currentPlayerIndex].transform;
	//	Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnUpdate();
		else nextTurn();

		if (players [0].HP <= 0 && players [1].HP <= 0 && players [2].HP <= 0 && players [3].HP <= 0) {
			Loose = true;
		}

		AtackOnMouseClick ();
		//Camera.main.transform.LookAt (players[currentPlayerIndex].transform);
		//Camera.main.GetComponent<CameraOrbit>().pivot = players[currentPlayerIndex].transform;
		//Camera.main.GetComponent<CameraOrbit> ().pivotOffset = 0.9f * Vector3.up;
	}
	
	void OnGUI () {
		if (players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnOnGUI();
		if (Loose) {
						GUI.Label (new Rect (Screen.width / 2, Screen.height / 2, 200f, 200f), "You Loose!");
				}
	}
	
	public void nextTurn() {
		if (currentPlayerIndex + 1 < players.Count) {

			removeTileHighlights();

			currentPlayerIndex++;

			Camera.main.GetComponent<CameraOrbit> ().pivotOffset = Vector3.zero;

			unitselection.transform.position = players [currentPlayerIndex].transform.position;
			unitselection.transform.parent = players [currentPlayerIndex].transform;

			Camera.main.GetComponent<CameraOrbit>().pivot = players[currentPlayerIndex].transform;
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;

			if (players[currentPlayerIndex].poisonTimer >0) {
				players[currentPlayerIndex].poisonTimer--;
			} else {
				players[currentPlayerIndex].poisoned = false;
			}

			if (players[currentPlayerIndex].stunTimer >0) {
				players[currentPlayerIndex].stunTimer--;
			} else {
				players[currentPlayerIndex].stunned = false;
			}

			if (players[currentPlayerIndex].burnTimer >0) {
				players[currentPlayerIndex].burnTimer--;
			} else {
				players[currentPlayerIndex].burned = false;
			}

			if (players[currentPlayerIndex].freezeTimer >0) {
				players[currentPlayerIndex].freezeTimer--;
			} else {
				players[currentPlayerIndex].freezed = false;
			}

			players[currentPlayerIndex].moving = true;
			players[currentPlayerIndex].attacking = false;
			players[currentPlayerIndex].rangeattacking = false;
			GameManager.instance.highlightTilesAt(players[currentPlayerIndex].gridPosition, Color.blue, players[currentPlayerIndex].movementPerActionPoint, false);
		} else {
		//	Camera.main.GetComponent<CameraOrbit> ().pivotOffset = Vector3.zero;
			currentPlayerIndex = 0;

			removeTileHighlights();

			Camera.main.GetComponent<CameraOrbit>().pivot = players[currentPlayerIndex].transform;
			Camera.main.GetComponent<CameraOrbit> ().pivotOffset += 0.9f * Vector3.up;

			unitselection.transform.position = players [currentPlayerIndex].transform.position;
			unitselection.transform.parent = players [currentPlayerIndex].transform;

			if (players[currentPlayerIndex].poisonTimer >0) {
				players[currentPlayerIndex].poisonTimer--;
			} else {
				players[currentPlayerIndex].poisoned = false;
			}
			
			if (players[currentPlayerIndex].stunTimer >0) {
				players[currentPlayerIndex].stunTimer--;
			} else {
				players[currentPlayerIndex].stunned = false;
			}
			
			if (players[currentPlayerIndex].burnTimer >0) {
				players[currentPlayerIndex].burnTimer--;
			} else {
				players[currentPlayerIndex].burned = false;
			}
			
			if (players[currentPlayerIndex].freezeTimer >0) {
				players[currentPlayerIndex].freezeTimer--;
			} else {
				players[currentPlayerIndex].freezed = false;
			}

			players[currentPlayerIndex].moving = true;
			players[currentPlayerIndex].attacking = false;
			players[currentPlayerIndex].rangeattacking = false;
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

	public void AtackhighlightTiles(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers) {
		
		highlightedTiles = new List<Tile>();
		
		if (ignorePlayers) highlightedTiles = TileHighlightAtack.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance);
		else highlightedTiles = TileHighlightAtack.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance, players.Where(x => x.gridPosition != originLocation).Select(x => x.gridPosition).ToArray());

	/*	for (int index = 0; index<highlightedTiles.Count; index++ ) {
			if (Physics.Raycast(players[currentPlayerIndex].transform.position+0.5f*Vector3.up, highlightedTiles[index].transform.position, out hit, 100.0F))
			{
				if (hit.transform.position != highlightedTiles[index].transform.position) {
					Debug.Log (hit.transform.position);
					highlightedTiles.Remove(highlightedTiles[index]);
				}
			}
		}
		*/
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

		//if (destTile.visual.transform.renderer.materials[0].color != Color.white && !destTile.impassible && players[currentPlayerIndex].positionQueue.Count == 0) {
			if ((highlightedTiles.Contains(destTile)) && !destTile.impassible && players[currentPlayerIndex].positionQueue.Count == 0) {
			removeTileHighlights();
			players[currentPlayerIndex].moving = false;
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
				//newRotation.x = 0.0f;
				//newRotation.y = 0.0f;
				newRotation.x = 0.0f;
				players[currentPlayerIndex].transform.rotation = Quaternion.Slerp(players[currentPlayerIndex].transform.rotation, newRotation, 1);


				//Debug.Log ("p.x: " + players[currentPlayerIndex].gridPosition.x + ", p.y: " + players[currentPlayerIndex].gridPosition.y + " t.x: " + target.gridPosition.x + ", t.y: " + target.gridPosition.y);
				if (players[currentPlayerIndex].gridPosition.x >= target.gridPosition.x - players[currentPlayerIndex].attackRange && players[currentPlayerIndex].gridPosition.x <= target.gridPosition.x + players[currentPlayerIndex].attackRange &&
				    players[currentPlayerIndex].gridPosition.y >= target.gridPosition.y - players[currentPlayerIndex].attackRange && players[currentPlayerIndex].gridPosition.y <= target.gridPosition.y + players[currentPlayerIndex].attackRange) {

					
					removeTileHighlights();
								

					players[currentPlayerIndex].animation.Play("Attack");
					StartCoroutine(WaitAndCallback(players[currentPlayerIndex].animation["Attack"].length));

					/*if (players[currentPlayerIndex].animation.IsPlaying("ComboAttack") == false){
						Debug.Log("Atack animation finished");
					players[currentPlayerIndex].actionPoints=0;
					players[currentPlayerIndex].attacking = false;
					}*/
					//attack logic
					//roll to hit
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
						target.animation.Play("Damage");
						target.animation.CrossFade("Idle", 1f);
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
				magic = ((GameObject)Instantiate(MagicPrefab, players[currentPlayerIndex].transform.position+0.5f*Vector3.up, Quaternion.Euler(0,0,0)));

				//newRotation.x = 0.0f;
				//newRotation.y = 0.0f;
				newRotation.x = 0.0f;
				players[currentPlayerIndex].transform.rotation = Quaternion.Slerp(players[currentPlayerIndex].transform.rotation, newRotation, 1);
				
				
				//Debug.Log ("p.x: " + players[currentPlayerIndex].gridPosition.x + ", p.y: " + players[currentPlayerIndex].gridPosition.y + " t.x: " + target.gridPosition.x + ", t.y: " + target.gridPosition.y);
				if (players[currentPlayerIndex].gridPosition.x >= target.gridPosition.x - players[currentPlayerIndex].attackDistance && players[currentPlayerIndex].gridPosition.x <= target.gridPosition.x + players[currentPlayerIndex].attackDistance &&
				    players[currentPlayerIndex].gridPosition.y >= target.gridPosition.y - players[currentPlayerIndex].attackDistance && players[currentPlayerIndex].gridPosition.y <= target.gridPosition.y + players[currentPlayerIndex].attackDistance) {

					
					removeTileHighlights();
							
					
					players[currentPlayerIndex].animation.Play("Attack");
					StartCoroutine(WaitAndCallback(players[currentPlayerIndex].animation["Attack"].length));
					
					//attack logic
					//roll to hit
					bool hit = Random.Range(0.0f, 1.0f) <= players[currentPlayerIndex].attackChance;

					if (hit) {

						if (players[currentPlayerIndex].stunDamage) {
							target.stunned = true;
							target.stunTimer = players[currentPlayerIndex].stunTimerDuration;
						}
					
						magic.transform.DOMove(target.transform.position, 2f).OnComplete(MoveCompleted);
						magiceffect = true;
						//damage logic
						int amountOfDamage = (int)Mathf.Floor(players[currentPlayerIndex].damageBase + Random.Range(0, players[currentPlayerIndex].damageRollSides));
						


					//	target.animation.CrossFade("Damage");
						target.HP -= amountOfDamage;

						Debug.Log(players[currentPlayerIndex].playerName + " successfuly hit " + target.playerName + " for " + amountOfDamage + " damage!");
					} else {
						magic.transform.DOMove(target.transform.position, 2f).OnComplete(MoveCompleted);
						Debug.Log(players[currentPlayerIndex].playerName + " missed " + target.playerName + "!");
						/*target.animation.Play("Damage");
						target.animation.CrossFade("Idle", 1f);*/
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

//		map = new List<List<Tile>>();
//		for (int i = 0; i < mapSize; i++) {
//			List <Tile> row = new List<Tile>();
//			for (int j = 0; j < mapSize; j++) {
//				Tile tile = ((GameObject)Instantiate(TilePrefab, new Vector3(i - Mathf.Floor(mapSize/2),0, -j + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
//				tile.gridPosition = new Vector2(i, j);
//				row.Add (tile);
//			}
//			map.Add(row);
//		}
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
					/*for (int e = 0; e < tile.transform.position.y; e++) {
						Debug.Log("Check");
						Tile filltile = ((GameObject)Instantiate(PrefabHolder.instance.BASE_TILE_PREFAB, new Vector3(tile.transform.position.x,tile.transform.position.y-e,tile.transform.position.z), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
						filltile.gridPosition = new Vector2(i, j);
						filltile.setType(TileType.Normal);
						filltile.transform.parent = tile.transform;
					}*/
				}

				if (tile.type == TileType.VeryDifficult) {
					int height = 2;
					Vector3 temp = new Vector3(0, height,0 );
					tile.transform.position += temp;
					/*for (int e = 0; e < tile.transform.position.y; e++) {
						Debug.Log("Check");
						Tile filltile = ((GameObject)Instantiate(PrefabHolder.instance.BASE_TILE_PREFAB, new Vector3(tile.transform.position.x,tile.transform.position.y-e,tile.transform.position.z), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
						filltile.gridPosition = new Vector2(i, j);
						filltile.setType(TileType.Normal);
						filltile.transform.parent = tile.transform;
					}*/
				}

				if (tile.type == TileType.Impassible) {

					Vector3 temp = new Vector3(0, -1,0 );
					tile.transform.position += temp;
					//tile.renderer.material.mainTexture =  Resources.Load<Texture>("HPTP_NS_Lava1a.png"); 
					//tile.renderer.material.SetTexture("_MainTex", ImpasTex);
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
			player = ((GameObject)Instantiate(UserPlayerPrefab,Vector3.zero,Quaternion.identity)).GetComponent<UserPlayer>();
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
//		AIPlayer aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(6 - Mathf.Floor(mapSize/2),0f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
//		aiplayer.gridPosition = new Vector2(6,4);
//		aiplayer.transform.position = map[6][4].transform.position + 0.5f * Vector3.up;
//		aiplayer.playerName = "Bot1";
//		
//		players.Add(aiplayer);
//
//		aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(8 - Mathf.Floor(mapSize/2),0.5f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
//		aiplayer.gridPosition = new Vector2(8,4);
//
//		aiplayer.playerName = "Bot2";
//		
//		players.Add(aiplayer);
//
//		aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(12 - Mathf.Floor(mapSize/2),0.5f, -1 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
//		aiplayer.gridPosition = new Vector2(12,1);
//		aiplayer.playerName = "Bot3";
//		
//		players.Add(aiplayer);
//
//		aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(18 - Mathf.Floor(mapSize/2),0.5f, -8 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
//		aiplayer.gridPosition = new Vector2(18,8);
//		aiplayer.playerName = "Bot4";
//
//		players.Add(aiplayer);
	}

	public void Explode (Player target, GameObject magictodestroy) {

		GameObject magicExposion = ((GameObject)Instantiate(MagicExplosionPrefab, target.transform.position+0.5f*Vector3.up, Quaternion.Euler(0,0,0)));
		Debug.Log ("explosion");
		Destroy (magictodestroy);
		magiceffect = false;
		if (target.HP > 0) {
			target.animation.Play("Damage");
			target.animation.CrossFade("Idle", 2f);
		}

	}

	public void MoveCompleted (){
		Explode(targetPub, magic);
	}

	IEnumerator WaitAndCallback(float waitTime){

		yield return new WaitForSeconds(waitTime);
		nextTurn ();

	}

	public Vector2 getRandoMapTileXY(bool passible = false)
	{
		Vector2 tileXY = new Vector2(Random.Range(0,mapSize),Random.Range(0,mapSize));

		if ((passible == true)&&(map[(int)tileXY.x][(int)tileXY.y].impassible == true))
		{
			tileXY = getRandoMapTileXY(passible);
		}
		return tileXY;
	}

	public void AtackOnMouseClick () {
				
		if ((Input.GetMouseButtonDown(0))&&(!GUImanager.instance.mouseOverGUI))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(ray,out hit))
			{
				if ((hit.collider.gameObject.GetComponent<AIPlayer>() != null)&&(!hit.collider.gameObject.GetComponent<AIPlayer> ().dead)){
					if (players[currentPlayerIndex].rangeattacking) {
					GameManager.instance.distanceAttackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.y]);
					}
					if (players[currentPlayerIndex].attacking) {
					GameManager.instance.attackWithCurrentPlayer (map[(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.x][(int)hit.collider.gameObject.GetComponent<AIPlayer> ().gridPosition.y]);
					}
				}
			}
		}

	}
}
