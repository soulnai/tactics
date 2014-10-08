using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUImanager : MonoBehaviour {

	public static GUImanager instance;
	public GameManager gameManager;
	public GameObject controlsPanel;
	public GameObject statsPanel;
	public Unit unit;
	public bool mouseOverGUI = false;
	public List<Button> abilitiesButtonsList;
	public Text turnsIndicator;
	public Button abilityTest;
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
//		abilityTest.onClick.RemoveAllListeners();
//		abilityTest.onClick.AddListener(delegate{onAbilityClick();});
	}
	
	// Update is called once per frame
	void Update () {
		//убрать потом из апдейта
		unit = gameManager.units[gameManager.currentUnitIndex];
		turnsIndicator.text = "Turn - "+gameManager.turnsCounter;
	}

	public void onAbilityClick(BaseAbility a) {
		Debug.Log(a.attackID);
		GameManager.instance.currentUnit.onAbility(a);
	}

	public void setMouseOverGUI(bool over)
	{
		mouseOverGUI = over;
	}

	public void OnMoveClick()
	{
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
		List<BaseAbility> abilitiesList = gm.units[gm.currentUnitIndex].GetComponent<UnitSkillsManager>().abilities;

		foreach(Button b in abilitiesButtonsList)
		{
			b.gameObject.SetActive(false);
		}

		for(int i = 0; i < abilitiesList.Count; i++)
		{
			int j = i;
			abilitiesButtonsList[j].gameObject.SetActive(true);
			abilitiesButtonsList[j].GetComponent<buttonTextController>().setText(abilitiesList[j].attackID);
			abilitiesButtonsList[j].onClick.RemoveAllListeners();
			abilitiesButtonsList[j].onClick.AddListener(delegate{onAbilityClick(abilitiesList[j]);});
			Debug.Log(abilitiesList[j].attackID);
		}
	}

	public void updateStatsPanel()
	{

	}
}
