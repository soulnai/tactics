using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

public class GUImanager : MonoBehaviour {

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

	private GameManager gm;
	// Use this for initialization
	void Awake()
	{
		instance = this;
		foreach(Button b in abilitiesButtonsList)
		{
			b.gameObject.SetActive(false);
		}
	}

	void Start () {
		gm = GameManager.instance;
		showAbilities();
		abilitiesPanel.SetActive (!abilitiesPanel.activeInHierarchy);

		UnitEvents.onLockUI += LockUI;
		UnitEvents.onUnlockUI += UnlockUI;
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

	public void OnMoveClick()
	{
		abilitiesPanel.SetActive (false);
		Unit u = gm.currentUnit;
		u.tryMove();
	}

	public void OnEndTurnClick()
	{
		abilitiesPanel.SetActive (false);
		GameManager.instance.PlayerEndTurn();
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
}
