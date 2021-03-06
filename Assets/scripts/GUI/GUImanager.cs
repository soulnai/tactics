using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;
using Vectrosity;

public delegate void guiConfirmFunc();

public class GUImanager : MonoBehaviour {

	guiConfirmFunc funcNext;
	guiConfirmFunc funcCurrent;

	public static GUImanager instance;
    public UnitPanelGUI selectedUnit;
    public UnitPanelGUI selectedTargetUnit;
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
	public bool needConfirm = true;

	private GameManager gm;
	private List<Vector3> selectionRegionHighlight = new List<Vector3>();
	private VectorLine highlightSpline;
	private VectorLine pathSpline;
	// Use this for initialization
	void Awake()
	{
		abilitiesPanel.SetActive(false);
		instance = this;
		foreach(Button b in abilitiesButtonsList)
		{
			b.gameObject.SetActive(false);
		}
		EventManager.onLockUI += LockUI;
		EventManager.onUnlockUI += UnlockUI;
        EventManager.onUnitSelectionChanged += updateUnitPanels;
	    EventManager.onTileCursorOverChanged += updateTargetUnitPanel;
        EventManager.OnCursorEnterUnit += updateTargetUnitPanel;
	    EventManager.OnCursorExitUnit += hideTargetUnitPanel;
	}

    private void updateUnitPanels(Unit unit)
    {
        selectedUnit.Init(unit);
    }

    private void updateTargetUnitPanel(Tile tile)
    {
        if (tile.unitInTile != null)
        {
            updateTargetUnitPanel(tile.unitInTile);
        }
        else
        {
            hideTargetUnitPanel(null);
        }
    }
    private void updateTargetUnitPanel(Unit unit)
    {
        if(unit.playerOwner.type == playerType.ai)
            selectedTargetUnit.Init(unit);
    }

    private void hideTargetUnitPanel(Unit unit)
    {
        selectedTargetUnit.Hide();
    }
    void Start () {
		VectorLine.SetCamera3D(Camera.main);
		gm = GameManager.instance;
		initAbilities();
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
		abilitiesPanel.SetActive(false);
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

	public void OnOffAbilitiesList()
	{
		abilitiesPanel.SetActive (!abilitiesPanel.activeInHierarchy);
	}

	public void setMouseOverGUI(bool over)
	{
		mouseOverGUI = over;
	}

	public void OnMoveClick(){
		abilitiesPanel.SetActive (false);
		Unit u = gm.currentUnit;
		u.tryMove();
	}

	public void OnEndTurnClick(){
		if(needConfirm){
			funcCurrent = GameManager.instance.PlayerEndTurn;
			EventManager.RequestConfirm(funcCurrent,null);
		}
		else{
			abilitiesPanel.SetActive (false);
			GameManager.instance.PlayerEndTurn();
		}
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

	public void initAbilities()
	{
		GameManager gm = GameManager.instance;
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

	void OnDestroy(){
		EventManager.onLockUI -= LockUI;
		EventManager.onUnlockUI -= UnlockUI;
	}
}