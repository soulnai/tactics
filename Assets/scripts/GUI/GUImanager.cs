using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUImanager : MonoBehaviour {

	public static GUImanager instance;
	public GameManager gm;
	public GameObject controlsPanel;
	public GameObject abilitiesPanel;
	public GameObject statsPanel;
	public Unit unit;
	public bool mouseOverGUI = false;
	public List<Button> abilitiesButtonsList;
	public Text turnsIndicator;
	public Text playerIndicator;
	public Button abilityTest;
	public LogController Log;

	// Use this for initialization
	void Awake()
	{
		instance = this;
	}
	void Start () {
		gm = GameManager.instance;
		foreach(Button b in abilitiesButtonsList)
		{
			b.gameObject.SetActive(false);
		}
		showAbilities();
		abilitiesPanel.SetActive (!abilitiesPanel.activeInHierarchy);
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
		Unit u = gm.currentUnit;
		u.EndTurn();
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
			abilitiesButtonsList[j].GetComponent<buttonTextController>().setText(abilitiesList[j].abilityID);
			abilitiesButtonsList[j].onClick.RemoveAllListeners();
			abilitiesButtonsList[j].onClick.AddListener(delegate{onAbilityClick(abilitiesList[j]);});
		}
		}
	}

	public void updateStatsPanel()
	{

	}
}
