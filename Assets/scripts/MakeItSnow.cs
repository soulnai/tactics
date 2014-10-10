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
		Ray ray;// = gameObject.transform.position;
		LayerMask mask = 1<<LayerMask.NameToLayer("tiles");
		if ((Physics.Raycast (gameObject.transform.position, Vector3.down, out hit, 1000f, mask)) && (!GUImanager.instance.mouseOverGUI)) {
						if (hit.transform.gameObject.GetComponent<Tile> () != null) {

				Tile t = hit.transform.gameObject.GetComponent<Tile>();
				t.visual.transform.renderer.materials[1].mainTexture = snowTexture;

//							Debug.Log("Tile hitted");
						}
			if (snow == false) {
				GameObject blizzard = (GameObject)Instantiate(snowPrefab,transform.position+ new Vector3(0,5f,0),Quaternion.identity);
				snow = true;
			}

				}
	}
}
