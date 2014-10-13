using UnityEngine;
using System.Collections;

public class FXController : MonoBehaviour {
	public BaseAbility ability;
	public Unit unitOwner;
	public Unit unitTarget;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(BaseAbility a = null,Unit owner = null,Unit target = null){
		ability = a;
		unitOwner = owner;
		unitTarget = target;
	}

	public void Kill(){
		FXmanager.instance.OnFXMoveEnd(this.transform.position,ability,unitOwner,unitTarget);
		Destroy(gameObject);
	}
}
