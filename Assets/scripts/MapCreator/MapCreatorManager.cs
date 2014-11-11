using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnumSpace;

public class MapCreatorManager : MonoBehaviour {
	public static MapCreatorManager instance;

	public int mapSize;
	public List <List<Tile>> map = new List<List<Tile>>();

	public TileType palletSelection = TileType.Normal;
	public EnumSpace.editorStates editorState;
	public bool up;
	Transform mapTransform;

	// Use this for initialization
	void Awake () {
		instance = this;

		mapTransform = transform.FindChild("Map");

		generateBlankMap(mapSize);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void generateBlankMap(int mSize) {


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
				tile.setType(TileType.Normal);
				row.Add (tile);
			}
			map.Add(row);
		}
	}

	void loadMapFromXml() {
		MapXmlContainer container = MapSaveLoad.Load("map.xml");

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

	void saveMapToXml() {
		MapSaveLoad.Save(MapSaveLoad.CreateMapContainer(map), "map-test.xml");
	}

	void OnGUI() {

		//pallet
		Rect rect = new Rect(10, Screen.height - 80, 100, 60);

		if (GUI.Button(rect, "Normal")) {
			editorState = editorStates.setType;
			palletSelection = TileType.Normal;
		}

		rect = new Rect(10 + (100 + 10) * 1, Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "Difficult")) {
			editorState = editorStates.setType;
			palletSelection = TileType.Difficult;
		}

		rect = new Rect(10 + (100 + 10) * 2, Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "VeryDifficult")) {
			editorState = editorStates.setType;
			palletSelection = TileType.VeryDifficult;
		}

		rect = new Rect(10 + (100 + 10) * 3, Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "Impassible")) {
			editorState = editorStates.setType;
			palletSelection = TileType.Impassible;
		}
		//

		//IO 
		rect = new Rect(Screen.width - (10 + (100 + 10) * 3), Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "Clear Map")) {
			generateBlankMap(mapSize);
		}

		rect = new Rect(Screen.width - (10 + (100 + 10) * 2), Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "Load Map")) {
			loadMapFromXml();
		}

		rect = new Rect(Screen.width - (10 + (100 + 10) * 1), Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "Save Map")) {
			saveMapToXml();
		}
		//

	}

	public void setUP(bool u)
	{
		editorState = editorStates.setHeight;
		up = u;
	}
}
