using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;
using Vectrosity;

public delegate void guiConfirmFunc(bool answer);

public class GUImanager : MonoBehaviour {

	guiConfirmFunc func;

	public static GUImanager instance;
	public GameObject controlsPanel;
	public GameObject abilitiesPanel;
	public GameObject statsPanel;
	public Unit unit;
	public bool mouseOverGUI = false;
	public List<Button> abilitiesButtonsList;
	public Text turnsIndicator;
	public Text playerIndicator;
	public LogController Log;
	public List<UnitPanelGUI> unitPanels;
	public TooltipGUI tooltip;
	public VictoryPanelControllerGUI victoryPanel;
	public Button endTurnButton;
	public Button endPlacementButton;

	private GameManager gm;
	private List<Vector3> selectionRegionHighlight = new List<Vector3>();
	private VectorLine highlightSpline;
	private VectorLine pathSpline;
	// Use this for initialization
	void Awake()
	{
		instance = this;
		foreach(Button b in abilitiesButtonsList)
		{
			b.gameObject.SetActive(false);
		}
		UnitEvents.onLockUI += LockUI;
		UnitEvents.onUnlockUI += UnlockUI;
	}

	void Start () {
		gm = GameManager.instance;
		showAbilities();
		abilitiesPanel.SetActive (!abilitiesPanel.activeInHierarchy);
	}

	void LockUI ()
	{
		controlsPanel.SetActive(false);
		endTurnButton.gameObject.SetActive(false);
	}

	void UnlockUI ()
	{
		controlsPanel.SetActive(true);
		endTurnButton.gameObject.SetActive(true);
	}

	// Update is called once per frame
	void Update () {

		turnsIndicator.text = "Turn - "+gm.turnsCounter;
		playerIndicator.text = gm.currentPlayer.playerName;
	}

	public void onAbilityClick(BaseAbility a) {
		Debug.Log(a.abilityID);
		GameManager.instance.currentUnit.onAbility(a);
		abilitiesPanel.SetActive (false);
	}

	public void OnOff()
	{
		abilitiesPanel.SetActive (!abilitiesPanel.activeInHierarchy);
	}

	public void setMouseOverGUI(bool over)
	{
		mouseOverGUI = over;
	}

	public void OnMoveClick(bool noConfirm = false)
	{
		if(noConfirm == true){
			abilitiesPanel.SetActive (false);
			Unit u = gm.currentUnit;
			u.tryMove();
		}
		else{
			func = OnMoveClick;
			UnitEvents.ConfirmRequest(func);
		}
	}

	public void OnEndTurnClick()
	{
		abilitiesPanel.SetActive (false);
		GameManager.instance.PlayerEndTurn();
	}

	public void OnEndPlacementClick(){
		endPlacementButton.gameObject.SetActive(false);
		GameManager.instance.startBattlePhase();
	}

	public void ShowHideGUI()
	{
		controlsPanel.SetActive(!controlsPanel.activeSelf);
	}

	public void ReloadScene()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void SlowMo()
	{
		Time.timeScale = 0.5f;
	}

	public void showAbilities()
	{
		GameManager gm = GameManager.instance;
		if(gm.currentUnit.GetComponent<AbilitiesController>() != null){
		List<BaseAbility> abilitiesList = gm.currentUnit.GetComponent<AbilitiesController>().abilities;

		foreach(Button b in abilitiesButtonsList)
		{
			b.gameObject.SetActive(false);
		}

		for(int i = 0; i < abilitiesList.Count; i++)
		{
			int j = i;
			abilitiesButtonsList[j].gameObject.SetActive(true);
			abilitiesButtonsList[j].GetComponent<buttonController>().assignedElement = abilitiesList[j];
			abilitiesButtonsList[j].GetComponent<buttonController>().setText(abilitiesList[j].abilityID);
			abilitiesButtonsList[j].onClick.RemoveAllListeners();
			abilitiesButtonsList[j].onClick.AddListener(delegate{onAbilityClick(abilitiesList[j]);});
		}
		}
	}

	public void updateStatsPanel()
	{

	}

	public void showTooltip (TooltipHelperGUI t)
	{
		tooltip.showTooltipDelayed(t);
	}

	public void hideTooltip ()
	{
		tooltip.hideTooltip();
	}

	void OnDestroy(){
		UnitEvents.onLockUI -= LockUI;
		UnitEvents.onUnlockUI -= UnlockUI;
	}



	public class Vector3Sorter : IComparer<Vector3>{

		public int Compare(Vector3 A, Vector3 B)
		{
			//  Variables to Store the atans
			double aTanA, aTanB;
			
			//  Reference Point
			Vector3 reference = GameManager.instance.currentUnit.currentTile.transform.position;
			
			//  Fetch the atans
			aTanA = Mathf.Atan2(A.z - reference.z, A.x - reference.x);
			aTanB = Mathf.Atan2(B.z - reference.z, B.x - reference.x);
			
			//  Determine next point in Clockwise rotation
			if (aTanA < aTanB) return -1;
			else if (aTanA > aTanB) return 1;
			return 0;
		}
	}

	public class TileSorter : IComparer<Tile>{
		
		public int Compare(Tile A, Tile B)
		{
			//  Variables to Store the atans
			double aTanA, aTanB;
			
			//  Reference Point
			Unit reference = GameManager.instance.currentUnit;
			
			//  Fetch the atans
			aTanA = Mathf.Atan2(A.gridPosition.y - reference.gridPosition.y, A.gridPosition.x - reference.gridPosition.x);
			aTanB = Mathf.Atan2(B.gridPosition.y - reference.gridPosition.y, B.gridPosition.x - reference.gridPosition.x);
			
			//  Determine next point in Clockwise rotation
			if (aTanA < aTanB)return -1;
			else if (aTanA > aTanB) return 1;
			return 0;
		}
	}

	public void showHighlightRegion(List<Tile> tiles){
		int segments = 1000;
		selectionRegionHighlight.Clear();
		tiles.Add(GameManager.instance.currentUnit.currentTile);
		tiles.Sort(new GUImanager.TileSorter());
		foreach(Tile t in tiles){
			if(t.highlightController.getContour(tiles)!=null)
				selectionRegionHighlight.AddRange(t.highlightController.getContour(tiles));
		}
		selectionRegionHighlight.Sort(new Vector3Sorter());
		if(highlightSpline == null)
			highlightSpline = new VectorLine("highlightSpline", selectionRegionHighlight, null, 12.0f, LineType.Continuous, Joins.Fill);
		else
		{}
//		highlightSpline = new VectorLine("highlightSpline", new Vector3[segments+1], null, 4.0f, LineType.Continuous);
//		highlightSpline.MakeSpline (selectionRegionHighlight.ToArray(), segments, true);
		highlightSpline.Draw3DAuto();
	}

	public void hideHighlightRegion ()
	{
		if(highlightSpline != null)
			highlightSpline.Resize(0);
	}

	public void showPath(List<Tile> path)
	{
		if(pathSpline != null)
			pathSpline.Resize(0);
		List<Vector3> pathPoints = new List<Vector3>();
		pathPoints.Add(gm.currentUnit.currentTile.transform.position+Vector3.up*0.8f);
		foreach(Tile t in path)
			pathPoints.Add(t.transform.position+Vector3.up*0.8f);
		if(pathSpline == null)
			pathSpline = new VectorLine("path",pathPoints,ColorHolder.instance.pathMat,8f,LineType.Continuous,Joins.Fill);
		else{
			pathSpline.Resize(0);
			pathSpline.points3.AddRange(pathPoints);
		}
		pathSpline.Draw3DAuto();
	}

	public void hidePath(){
		if(pathSpline != null)
			pathSpline.Resize(0);
	}

}