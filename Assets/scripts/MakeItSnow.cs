using UnityEngine;
using System.Collections;

public class MakeItSnow : MonoBehaviour {
	private RaycastHit hit;
	public Texture snowTexture;
	public bool snow = false;
	public GameObject snowPrefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		LayerMask mask = 1<<LayerMask.NameToLayer("tiles");
		if ((Physics.Raycast (gameObject.transform.position, Vector3.down, out hit, 1000f, mask)) && (!GUImanager.instance.mouseOverGUI)) {
						if (hit.transform.gameObject.GetComponent<Tile> () != null) {
							Tile t = hit.transform.gameObject.GetComponent<Tile>();
							t.visual.transform.renderer.materials[1].mainTexture = snowTexture;
						}
			if (snow == false) 
				snow = true;
		}
	}
}
