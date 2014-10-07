using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUImanager : MonoBehaviour {

	public static GUImanager instance;
	public GameManager gameManager;
	public GameObject controlsPanel;
	public UserPlayer unit;
	public bool mouseOverGUI = false;

	public Button[] UIButtonsArray;
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
		unit = gameManager.players[gameManager.currentPlayerIndex] as UserPlayer;

		if (!unit.unitSkills.skillsList.Contains ("baseStun")) {
						UIButtonsArray [0].enabled = false;		
				} else {
			UIButtonsArray [0].enabled = true;	
		}

		if (!unit.unitSkills.skillsList.Contains ("baseRanged")) {
			UIButtonsArray [1].enabled = false;		
		} else {
			UIButtonsArray [1].enabled = true;	
		}

		if (!unit.unitSkills.skillsList.Contains ("baseMagic")) {
			UIButtonsArray [2].enabled = false;		
		} else {
			UIButtonsArray [2].enabled = true;	
		}
	}

	public void setMouseOverGUI(bool over)
	{
		mouseOverGUI = over;
	}

	public void OnMoveClick()
	{
		unit.Move();
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
}
