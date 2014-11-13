using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

public class Tile : MonoBehaviour {

	GameObject PREFAB;

	public GameObject visual;
	public GameObject highlight;

	public TileType type = TileType.Normal;

	public Vector2 gridPosition = Vector2.zero;
	
	public int movementCost = 1;
	public float height = 0;
	public bool impassible = false;
	
	public List<Tile> neighbors = new List<Tile>();

	public Unit unitInTile;
	public highlightContourController highlightController;

	public Tile upNeighbour;
	public Tile rightNeighbour;
	public Tile downNeighbour;
	public Tile leftNeighbour;

	private GameManager gm;
	// Use this for initialization
	void Awake(){
//		highlight.SetActive(false);
		gm = GameManager.instance;
	}

	void Start () {
		if (Application.loadedLevelName == "gameScene") {
			generateNeighbors();
		}
	}
	
	public void generateNeighbors() {		
		neighbors = new List<Tile>();
		
		//up
		if (gridPosition.y > 0) {
			Vector2 n = new Vector2(gridPosition.x, gridPosition.y - 1);
			neighbors.Add(gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
			upNeighbour = gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)];
		}
		//down
		if (gridPosition.y < gm.mapSize - 1) {
			Vector2 n = new Vector2(gridPosition.x, gridPosition.y + 1);
			neighbors.Add(gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
			downNeighbour = gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)];
		}		
		
		//left
		if (gridPosition.x > 0) {
			Vector2 n = new Vector2(gridPosition.x - 1, gridPosition.y);
			neighbors.Add(gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
			leftNeighbour = gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)];
		}
		//right
		if (gridPosition.x < gm.mapSize - 1) {
			Vector2 n = new Vector2(gridPosition.x + 1, gridPosition.y);
			neighbors.Add(gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
			rightNeighbour = gm.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)];
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseEnter() {
		if(Application.loadedLevelName == "gameScene"){
			UnitEvents.TileCursorOverChanged(this);
		}
		if (Application.loadedLevelName == "MapCreatorScene" && Input.GetMouseButton(0)) {
			if(MapCreatorManager.instance.editorState == editorStates.setType)
			setType(MapCreatorManager.instance.palletSelection);
			else if(MapCreatorManager.instance.editorState == editorStates.setHeight)
				changeHeight(MapCreatorManager.instance.up);
		}
	}
	
	void OnMouseExit() {

	}
	
	
	void OnMouseDown() {
		if ((Application.loadedLevelName == "gameScene")&&(!GUImanager.instance.mouseOverGUI)) {
			if (gm.currentUnit.UnitAction == unitActions.readyToMove) {
				gm.moveUnitTo(this);
			}
			UnitEvents.TileClick(this);
//			else if (gm.currentUnit.UnitAction == unitActions.meleeAttack) {
//				gm.attackWithCurrentPlayer(this);
//			} else if (gm.currentUnit.UnitAction == unitActions.rangedAttack) {
//				gm.distanceAttackWithCurrentPlayer(this);
//			} else if (gm.currentUnit.UnitAction == unitActions.magicAttack) {
//				gm.distanceAttackWithCurrentPlayer(this);
//			} else if (gm.currentUnit.UnitAction == unitActions.healAttack) {
//				gm.distanceAttackWithCurrentPlayer(this);
//			}
		} else if (Application.loadedLevelName == "MapCreatorScene") {
			if(MapCreatorManager.instance.editorState == editorStates.setType)
			setType(MapCreatorManager.instance.palletSelection);
			else if(MapCreatorManager.instance.editorState == editorStates.setHeight)
				changeHeight(MapCreatorManager.instance.up);
		}
	}

	public void changeHeight(bool up)
	{
		Vector3 tempHeight = new Vector3(0,0.2f,0 );
		if(up == true)
		{
			transform.position += tempHeight;
		}
		else
		{
			transform.position -= tempHeight;
		}
		height = transform.position.y;
	}

	public void setType(TileType t) {
		type = t;
		//definitions of TileType properties
		switch(t) {
			case TileType.Normal:
				movementCost = 1;
				impassible = false;
				PREFAB = PrefabHolder.instance.TILE_NORMAL_PREFAB;
				break;
			
			case TileType.Difficult:
				movementCost = 2;
				impassible = false;
				PREFAB = PrefabHolder.instance.TILE_DIFFICULT_PREFAB;
				break;
				
			case TileType.VeryDifficult:
				movementCost = 3;
				impassible = false;
				PREFAB = PrefabHolder.instance.TILE_VERY_DIFFICULT_PREFAB;
				break;
				
			case TileType.Impassible:
				movementCost = 999;
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

		GameObject newVisual = (GameObject)Instantiate(PREFAB, transform.position, Quaternion.identity);
		newVisual.transform.parent = container.transform;
		visual = newVisual;
	}

	public void showHighlight(Color color){
		highlight.SetActive(true);
		highlight.renderer.material.color = color;
	}

	public void hideHighlight(){
		highlight.SetActive(false);
	}
}
