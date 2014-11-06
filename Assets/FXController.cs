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

	public void Init(Unit target){
		Init(default(Vector3),default(Vector3),null,null,target);
		transform.SetParent(target.transform);
	}

	public void Init(Vector3 startPos = default(Vector3), Vector3 endPos = default(Vector3),BaseAbility a = null,Unit owner = null,Unit target = null){
		ability = a;
		unitOwner = owner;
		unitTarget = target;
		if(GetComponent<CFX_AutoDestructShuriken>() == null)
			StartCoroutine(delayedDestroy(2f));
	}

	IEnumerator delayedDestroy (float time)
	{
		yield return new WaitForSeconds(time);
		if(this != null)
			Destroy(gameObject);
	}

	public void Kill(){
		FXmanager.instance.OnFXMoveEnd(this.transform.position,ability,unitOwner,unitTarget);
		if(unitTarget!=null)
			UnitEvents.FXEnd(unitOwner,unitTarget);
		Destroy(gameObject);
	}
}
