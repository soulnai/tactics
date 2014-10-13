using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FXmanager : MonoBehaviour {
	public static FXmanager instance;
	public float fxSpeed = 5f;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void createFX(GameObject FXprefab,Vector3 startPos = default(Vector3), Vector3 endPos = default(Vector3),BaseAbility a = null,Unit owner = null,Unit target = null)
	{
		GameObject fx;
		fx = ((GameObject)Instantiate(FXprefab,startPos+0.5f*Vector3.up, Quaternion.identity));
		FXController fxC = fx.AddComponent("FXController") as FXController;
		fxC.Init(a,owner,target);
		if(startPos != endPos){
			float distance = Vector3.Distance(startPos,endPos);
			float duration = distance/fxSpeed;
			fx.transform.DOMove(endPos+1.0f*Vector3.up, duration).OnComplete(fxC.Kill);
		}
	}

	public void OnFXMoveEnd (Vector3 position, BaseAbility ability, Unit unitOwner, Unit unitTarget)
	{
		if(ability.hitFXprefab != null)
		{
			createFX(ability.hitFXprefab,position,position,ability);
		}
	}
}
