using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FXmanager : MonoBehaviour {
	public static FXmanager instance;
	public float fxSpeed = 5f;

	public List<GameObject> FXlist;
	
	public void Awake()
	{
		instance = this;
	}
	
	public GameObject getFX(string ID)
	{
		return FXlist.Find(GameObject => gameObject.name == ID) as GameObject; 
	}
	
	public void addFX(GameObject fx)
	{
		if(getFX(fx.name) == null)
			FXlist.Add(fx);
		else
			Debug.Log("FX already in list");
	}
	
	public void removeFX(GameObject fx)
	{
		if(getFX(fx.name) != null)
			FXlist.Remove(fx);
		else
			Debug.Log("FX not in list");
	}

	public void createAbilityFX(GameObject FXprefab,Vector3 startPos = default(Vector3), Vector3 endPos = default(Vector3),BaseAbility a = null,Unit owner = null,Unit target = null)
	{
		if(a.attackType != EnumSpace.attackTypes.melee)
		{
			createFX(a.rangedFXprefab,startPos,endPos,a,owner,target);
		}
		else
		{
			createFX(a.hitFXprefab,startPos,startPos,a,owner,target);
		}
	}

	public void createEffectFX(GameObject FXprefab,Unit target)
	{
		GameObject fx;
		fx = ((GameObject)Instantiate(FXprefab,target.transform.position+0.5f*Vector3.up, Quaternion.identity));
		fx.transform.SetParent(target.transform);
		FXController fxC = fx.AddComponent("FXController") as FXController;
		fxC.Init(target);

	}

	public void createFX(GameObject FXprefab,Vector3 startPos = default(Vector3), Vector3 endPos = default(Vector3),BaseAbility a = null,Unit owner = null,Unit target = null)
	{
		GameObject fx;
		fx = ((GameObject)Instantiate(FXprefab,startPos+0.5f*Vector3.up, Quaternion.identity));
		FXController fxC = fx.AddComponent("FXController") as FXController;
		fxC.Init(startPos,endPos,a,owner,target);
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
