using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUImanager : MonoBehaviour {

	public static GUImanager instance;
	public GameManager gameManager;
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
		gameManager = GameManager.instance;
		foreach(Button b in abilitiesButtonsList)
		{
			b.gameObject.SetActive(false);
		}
		showAbilities();
		abilitiesPanel.SetActive (!abilitiesPanel.activeInHierarchy);
	}
	
	// Update is called once per frame
	void Update () {
		turnsIndicator.text = "Turn - "+gameManager.turnsCounter;
		playerIndicator.text = gameManager.currentPlayer.playerName;
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
		Unit u = gameManager.units[gameManager.currentUnitIndex] as Unit;
		u.tryMove();
	}

	public void OnAttackClick()
	{
//		unit.MeleeAttack() ;
	}

	public void OnStunAttackClick()
	{
//		unit.StunAttack ();
	}

	public void OnRangedAttackClick()
	{
//		unit.RangedAttack ();
	}

	public void OnMagicAttackClick()
	{
//		unit.MagicAttack();
	}

	public void OnEndTurnClick()
	{
		abilitiesPanel.SetActive (false);
		Unit u = gameManager.units[gameManager.currentUnitIndex] as Unit;
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
		if(gm.units[gm.currentUnitIndex].GetComponent<UnitSkillsManager>() != null){
		List<BaseAbility> abilitiesList = gm.units[gm.currentUnitIndex].GetComponent<UnitSkillsManager>().abilities;

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
//			Debug.Log(abilitiesList[j].attackID);
		}
		}
	}

	public void updateStatsPanel()
	{

	}
}
