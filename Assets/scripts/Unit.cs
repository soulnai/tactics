using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

//Events
public delegate void UnitAnimationEnd(Unit unit);
//

[System.Serializable]
public class Unit : MonoBehaviour {

	//public AnimationClip[] animationsArray;
	public event UnitAnimationEnd OnUnitAnimationEnd;

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
	public Tile currentTile;

	//movement animation
	public List<Vector3> positionQueue = new List<Vector3>();	
	//

	private Vector3 lookDirection = Vector3.zero;

	void Awake () {
		moveDestination = transform.position;
	}
	
	// Use this for initialization
	void Start () {
		actionPoints = maxActionPoints;
	}
	
	// Update is called once per frame
	public virtual void Update () {		
		if (UnitState == unitStates.dead&&(GameManager.instance.units[GameManager.instance.currentUnitIndex] == this)) {
			EndTurn();
		}
		if(UnitAction == unitActions.moving)
		{
			MoveUnit();
		}
	}

	public void Attack(Unit target)
	{
		animation.Play("Attack");
		StartCoroutine(WaitAndCallback(animation["Attack"].length));
		actionPoints=0;
//		checkAP ();
	}

	public void tryMove ()
	{
		GameManager.instance.removeTileHighlights ();
		if(actionPoints > 0){
			UnitAction = unitActions.readyToMove;
			GameManager.instance.highlightTilesAt (gridPosition, Color.blue, movementPerActionPoint, false);
		}
	}

	public void MoveUnit()
	{
		if (positionQueue.Count > 0) {
			lookDirection = (positionQueue[0] - transform.position).normalized;
			lookDirection.y = 0;
			
			transform.rotation = Quaternion.Lerp(transform.rotation,(Quaternion.LookRotation((lookDirection).normalized)),0.1f);
			transform.position += (positionQueue[0] - transform.position).normalized * moveSpeed * Time.deltaTime;
			if (!animation.IsPlaying("Run")) {animation.CrossFade("Run", 0.2F);}
			if (Vector3.Distance(positionQueue[0], transform.position) <= 0.1f) {
				positionQueue.RemoveAt(0);
				if (positionQueue.Count == 0) {
					animation.Stop();
					animation.CrossFade("Idle", 0.2F);
					actionPoints--;
					UnitAction = unitActions.idle;
					checkAP();
				}
			}	
		}
	}

	public void makeDead()
	{
		HP = 0;
		actionPoints = 0;
		UnitState = unitStates.dead;
		GameManager.instance.checkVictory();
		animation.CrossFade("Death");
		StartCoroutine(WaitAndCallback(animation["Death"].length));
	}

	public virtual void EndTurn () {
		GameManager.instance.removeTileHighlights ();
		actionPoints = 0;
		UnitAction = unitActions.idle;
		GameManager.instance.nextTurn ();
	}

	public void OnGUI() {
		//display HP
		Vector3 location = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 55;
		GUI.Label(new Rect(location.x, Screen.height - location.y, 30, 20), HP.ToString());
	}

	public void checkAP()
	{
		if(actionPoints <= 0)
			EndTurn();
		Debug.Log(actionPoints);
	}

	public void takeDamage(int damage)
	{
		HP -= damage;
		if (HP<=0)
			makeDead();
		else
		{
			animation.CrossFade("Damage");
			StartCoroutine(WaitAndCallback(animation["Damage"].length));
		}
	}

	public void placeUnit(Vector2 position)
	{
		gridPosition = position;
		currentTile = GameManager.instance.map[(int)position.x][(int)position.y];
		currentTile.unitInTile = this;
		transform.position = currentTile.transform.position + new Vector3(0,0.5f,0);
	}
	IEnumerator WaitAndCallback(float waitTime){
		Debug.Log("Started");
		yield return new WaitForSeconds(waitTime);
//		Debug.Log("Animation Ended");
		if(OnUnitAnimationEnd != null)
		{
			OnUnitAnimationEnd(this);
		}
	}
}
