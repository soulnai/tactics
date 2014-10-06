using UnityEngine;
using System.Collections;

public class StunSkill : MonoBehaviour {

	public int stunBaseDuration = 3;
	public int stunBaseDamage = 5;
	public int stunBaseRange = 2;
	public bool stunned = true;
	public GameObject visualprefab = MagicPrefabHolder.instance.Lightning;
	public GameObject explosionprefab = MagicPrefabHolder.instance.LightningExplode;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
