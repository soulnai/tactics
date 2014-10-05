using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AttacksManager : MonoBehaviour {

	public static AttacksManager instance;
	public List<BaseAttack> attacks;

	public void Awake()
	{
		instance = this;
	}

	public BaseAttack getAttack(string ID)
	{
		return attacks.Find(BaseAttack => BaseAttack.attackID == ID); 
	}
}
