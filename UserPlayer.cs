using UnityEngine;
using System.Collections;

public class UserPlayer : Player {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
		if (GameManager.instance.players[GameManager.instance.currentPlayerIndex] == this) {
			transform.renderer.material.color = Color.green;
		} else {
			transform.renderer.material.color = Color.white;
		}

		base.Update();
	}
	
	public override void TurnUpdate ()
	{
		//highlight
		//
		//
		
		if (positionQueue.Count > 0) {
			//transform.rotation = Quaternion.LookRotation((positionQueue[0] - transform.position).normalized);

			var newRotation = Quaternion.LookRotation((positionQueue[0] - transform.position).normalized);
			//newRotation.x = 0.0f;
			//newRotation.y = 0.0f;
			newRotation.x = 0.0f;
			transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 1);


			transform.position += (positionQueue[0] - transform.position).normalized * moveSpeed * Time.deltaTime;
			if (!animation.IsPlaying("Walk")) {animation.CrossFade("Walk", 0.2F);}
			if (Vector3.Distance(positionQueue[0], transform.position) <= 0.1f) {
				transform.position = positionQueue[0];
				positionQueue.RemoveAt(0);
				if (positionQueue.Count == 0) {
					animation.Stop();
					animation.CrossFade("Idle", 0.2F);
					newRotation.x = 0.0f;
					//newRotation.y = 0.0f;
					newRotation.z = 0.0f;
					transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 1);
					actionPoints--;
				}
			}
			
		}
		
		base.TurnUpdate ();
	}
	
	public override void TurnOnGUI () {
		float buttonHeight = 50;
		float buttonWidth = 150;
		
		Rect buttonRect = new Rect(0, Screen.height - buttonHeight * 3, buttonWidth, buttonHeight);
		
		
		//move button
		if (GUI.Button(buttonRect, "Move")) {
			if (!moving) {
				GameManager.instance.removeTileHighlights();
				moving = true;
				attacking = false;
				rangeattacking = false;
				GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);
			} else {
				moving = false;
				attacking = false;
				rangeattacking = false;
				GameManager.instance.removeTileHighlights();
			}
		}

		buttonRect = new Rect(0, Screen.height - buttonHeight * 4, buttonWidth, buttonHeight);
		
		
		//move button
		if (GUI.Button(buttonRect, "Attack Distance")) {
			if (!attacking && !rangeattacking) {
				GameManager.instance.removeTileHighlights();
				moving = false;
				attacking = false;
				rangeattacking = true;
				GameManager.instance.AtackhighlightTiles(gridPosition, Color.red, attackDistance, true);
			} else {
				moving = false;
				attacking = false;
				rangeattacking = false;
				GameManager.instance.removeTileHighlights();
			}
		}
		
		//attack button
		buttonRect = new Rect(0, Screen.height - buttonHeight * 2, buttonWidth, buttonHeight);
		
		if (GUI.Button(buttonRect, "Attack")) {
			if (!attacking && !rangeattacking) {
				GameManager.instance.removeTileHighlights();
				moving = false;
				attacking = true;
				rangeattacking = false;
				GameManager.instance.AtackhighlightTiles(gridPosition, Color.red, attackRange, true);
			} else {
				moving = false;
				attacking = false;
				rangeattacking = false;
				GameManager.instance.removeTileHighlights();
			}
		}
		
		//end turn button
		buttonRect = new Rect(0, Screen.height - buttonHeight * 1, buttonWidth, buttonHeight);		
		
		if (GUI.Button(buttonRect, "End Turn")) {
			GameManager.instance.removeTileHighlights();
			actionPoints = 2;
			moving = false;
			attacking = false;
			rangeattacking = false;
			GameManager.instance.nextTurn();
		}
		
		base.TurnOnGUI ();
	}
}
