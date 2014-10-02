using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	public Vector2 gridPosition = Vector2.zero;
	
	public Vector3 moveDestination;
	public float moveSpeed = 10.0f;
	
	public int movementPerActionPoint = 5;
	public int attackRange = 1;
	public int attackDistance = 5;
	
	public bool moving = false;
	public bool attacking = false;
	public bool rangeattacking = false;
	public bool dead = false;
	
	
	public string playerName = "George";
	public int HP = 25;
	
	public float attackChance = 0.75f;
	public float defenseReduction = 0.15f;
	public int damageBase = 5;
	public float damageRollSides = 6; //d6
	
	public int actionPoints = 2;
	
	//movement animation
	public List<Vector3> positionQueue = new List<Vector3>();	
	//
	
	void Awake () {
		moveDestination = transform.position;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {		
		if (HP <= 0 && dead == false && GameManager.instance.magiceffect == false) {
			//transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
			transform.renderer.material.color = Color.red;
			dead = true;
			animation.CrossFade("Death");
			StartCoroutine(WaitAndCallback(animation["Death"].length));
		}
	}
	
	public virtual void TurnUpdate () {
		if (actionPoints <= 0) {
			actionPoints = 2;
			moving = false;
			attacking = false;	
			rangeattacking = false;
			GameManager.instance.nextTurn();
		}
	}
	
	public virtual void TurnOnGUI () {
		
	}

	public void OnGUI() {
		//display HP
		Vector3 location = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 35;
		GUI.TextArea(new Rect(location.x, Screen.height - location.y, 30, 20), HP.ToString());
	}

	IEnumerator WaitAndCallback(float waitTime){
		
		yield return new WaitForSeconds(waitTime);

		
	}
}
