using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

[System.Serializable]
public class Unit : MonoBehaviour {

	//public AnimationClip[] animationsArray;


	public Vector2 gridPosition = Vector2.zero;
	
	public Vector3 moveDestination;
	public float moveSpeed = 10.0f;
	
	public int movementPerActionPoint = 5;
	public int attackRange = 1;
	public int attackDistance = 5;
	
	public string unitName = "George";
	public int HP = 25;
	public int MP = 25;
	public int Strength = 2;
	public int Magic = 2;
	public int PhysicalDefense = 2;
	public int MagicDefense = 2;
	
	public float attackChance = 0.75f;
	public float defenseReduction = 0.15f;
	public int damageBase = 5;
	public float damageRollSides = 6; //d6
	
	public int maxActionPoints = 2;
	public int actionPoints;

	public EnumSpace.unitStates UnitState;
	public EnumSpace.unitActions UnitAction;

	//movement animation
	public List<Vector3> positionQueue = new List<Vector3>();	
	//


	void Awake () {
		moveDestination = transform.position;
	}
	
	// Use this for initialization
	void Start () {
		actionPoints = maxActionPoints;
	}
	
	// Update is called once per frame
	public virtual void Update () {		
		if (HP <= 0 && UnitState!=unitStates.dead && GameManager.instance.magiceffect == false &&(GameManager.instance.units[GameManager.instance.currentUnitIndex] == this)) {
			GameManager.instance.nextTurn();
		}
	}

	public void makeDead()
	{
		HP = 0;
		actionPoints = 0;
		UnitState = unitStates.dead;
		animation.CrossFade("Death");
		StartCoroutine(WaitAndCallback(animation["Death"].length));
	}

	public virtual void EndTurn () {
		GameManager.instance.removeTileHighlights ();
		actionPoints = 0;
		UnitAction = unitActions.idle;
		GameManager.instance.nextTurn ();
	}
	
	public virtual void TurnOnGUI () {
		
	}

	public void OnGUI() {
		//display HP
		Vector3 location = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 35;
		GUI.Label(new Rect(location.x, Screen.height - location.y, 30, 20), HP.ToString());
	}

	public void checkAP()
	{
		if(actionPoints <= 0)
			EndTurn();
		Debug.Log(actionPoints);
	}

	IEnumerator WaitAndCallback(float waitTime){	
		yield return new WaitForSeconds(waitTime);
	}
}
