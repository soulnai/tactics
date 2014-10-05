using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUImanager : MonoBehaviour {

	public static GUImanager instance;
	public GameManager gameManager;
	public GameObject controlsPanel;
	public UserPlayer unit;
	public bool mouseOverGUI = false;
	// Use this for initialization
	void Awake()
	{
		instance = this;
	}
	void Start () {
		gameManager = GameManager.instance;
//		unit = gameManager.players[gameManager.currentPlayerIndex] as UserPlayer;
//		Camera.main.WorldToScreenPoint(transform.position)
	}
	
	// Update is called once per frame
	void Update () {
		unit = gameManager.players[gameManager.currentPlayerIndex] as UserPlayer;
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
