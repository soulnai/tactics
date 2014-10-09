using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

public class Tile : MonoBehaviour {

	GameObject PREFAB;

	public GameObject visual;

	public TileType type = TileType.Normal;

	public Vector2 gridPosition = Vector2.zero;
	
	public int movementCost = 1;
	public int height = 0;
	public bool impassible = false;
	
	public List<Tile> neighbors = new List<Tile>();

	public Unit unitInTile;

	private GameManager gm;
	// Use this for initialization
	void Start () {
		if (Application.loadedLevelName == "gameScene") {
			gm = GameManager.instance;
			generateNeighbors();
		}
	}
	
	public void generateNeighbors() {		
		neighbors = new List<Tile>();
		
		//up
		if (gridPosition.y > 0) {
			Vector2 n = new Vector2(gridPosition.x, gridPosition.y - 1);
			if (Mathf.Abs(height - gm.map[(int)n.x][(int)n.y].height) <= gm.maxHeighDiff){
				neighbors.Add(gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
			}
		}
		//down
		if (gridPosition.y < gm.mapSize - 1) {
			Vector2 n = new Vector2(gridPosition.x, gridPosition.y + 1);
			if (Mathf.Abs(height - gm.map[(int)n.x][(int)n.y].height) <= gm.maxHeighDiff){
				neighbors.Add(gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
			}
		}		
		
		//left
		if (gridPosition.x > 0) {
			Vector2 n = new Vector2(gridPosition.x - 1, gridPosition.y);
			if (Mathf.Abs(height - gm.map[(int)n.x][(int)n.y].height) <= gm.maxHeighDiff){
				neighbors.Add(gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
			}
		}
		//right
		if (gridPosition.x < gm.mapSize - 1) {
			Vector2 n = new Vector2(gridPosition.x + 1, gridPosition.y);
			if (Mathf.Abs(height - gm.map[(int)n.x][(int)n.y].height) <= gm.maxHeighDiff){
				neighbors.Add(gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseEnter() {
		if (Application.loadedLevelName == "MapCreatorScene" && Input.GetMouseButton(0)) {
			setType(MapCreatorManager.instance.palletSelection);
		}
	}
	
	void OnMouseExit() {

	}
	
	
	void OnMouseDown() {
		if ((Application.loadedLevelName == "gameScene")&&(!GUImanager.instance.mouseOverGUI)) {
			if (gm.units[gm.currentUnitIndex].UnitAction == unitActions.readyToMove) {
				gm.moveCurrentPlayer(this);
			} else if (gm.units[gm.currentUnitIndex].UnitAction == unitActions.meleeAttack) {
				gm.attackWithCurrentPlayer(this);
			} else if (gm.units[gm.currentUnitIndex].UnitAction == unitActions.rangedAttack) {
				gm.distanceAttackWithCurrentPlayer(this);
			} else if (gm.units[gm.currentUnitIndex].UnitAction == unitActions.magicAttack) {
				gm.distanceAttackWithCurrentPlayer(this);
			} else if (gm.units[gm.currentUnitIndex].UnitAction == unitActions.healAttack) {
				gm.distanceAttackWithCurrentPlayer(this);
			}
		} else if (Application.loadedLevelName == "MapCreatorScene") {
			setType(MapCreatorManager.instance.palletSelection);
		}
	}

	public void setType(TileType t) {
		type = t;
		//definitions of TileType properties
		switch(t) {
			case TileType.Normal:
				movementCost = 1;
				height=0;
				impassible = false;
				PREFAB = PrefabHolder.instance.TILE_NORMAL_PREFAB;
				break;
			
			case TileType.Difficult:
				movementCost = 1;
				height=1;
				impassible = false;
				PREFAB = PrefabHolder.instance.TILE_DIFFICULT_PREFAB;
				break;
				
			case TileType.VeryDifficult:
				movementCost = 1;
				height=2;
				impassible = false;
				PREFAB = PrefabHolder.instance.TILE_VERY_DIFFICULT_PREFAB;
				break;
				
			case TileType.Impassible:
				movementCost = 1;
				height=3;
				impassible = true;
				PREFAB = PrefabHolder.instance.TILE_IMPASSIBLE_PREFAB;
				break;

			default:
				movementCost = 1;
				impassible = false;
				PREFAB = PrefabHolder.instance.TILE_NORMAL_PREFAB;
				break;
		}

		generateVisuals();
	}

	public void generateVisuals() {
		GameObject container = transform.FindChild("Visuals").gameObject;
		//initially remove all children
		for(int i = 0; i < container.transform.childCount; i++) {
			Destroy (container.transform.GetChild(i).gameObject);
		}

		GameObject newVisual = (GameObject)Instantiate(PREFAB, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
		newVisual.transform.parent = container.transform;
		//newVisual.renderer.material.SetTexture("_MainTex", gm.ImpasTex);
		visual = newVisual;
	}
}
