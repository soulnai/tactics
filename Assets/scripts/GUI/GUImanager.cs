using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUImanager : MonoBehaviour {

	public static GUImanager instance;
	public GameManager gameManager;
	public GameObject controlsPanel;
	public GameObject statsPanel;
	public UserUnit unit;
	public bool mouseOverGUI = false;
	public List<Button> skillsButtonsList;

	// Use this for initialization
	void Awake()
	{
		instance = this;
	}
	void Start () {
		gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		//убрать потом из апдейта
		unit = gameManager.units[gameManager.currentUnitIndex] as UserUnit;
	}

	public void setMouseOverGUI(bool over)
	{
		mouseOverGUI = over;
	}

	public void OnMoveClick()
	{
		unit.tryMove();
	}

	public void OnAttackClick()
	{
		unit.MeleeAttack();
	}

	public void OnStunAttackClick()
	{
		unit.StunAttack ();
	}

	public void OnRangedAttackClick()
	{
		unit.RangedAttack ();
	}

	public void OnMagicAttackClick()
	{
		unit.MagicAttack();
	}

	public void OnEndTurnClick()
	{
		unit.EndTurn();
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

	public void updateSkills()
	{
		GameManager gm = GameManager.instance;
		int skillsCount = gm.units[gm.currentUnitIndex].GetComponent<UnitSkillsManager>().skillsList.Count;
		for(int i = 0; i<skillsCount; i++)
		{
			skillsButtonsList[i].gameObject.SetActive(true);
//			skillsButtonsList[i].onClick();
		}
	}

	public void updateStatsPanel()
	{

	}
}
