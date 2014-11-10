using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPanelController : MonoBehaviour {

	public Text title;
	public Text descr;
	public Button yesButton;
	public Button noButton;

	private guiConfirmFunc funcToConfirm;

	void Awake(){
		UnitEvents.onConfirmRequest += showConfirmationPanel;
	}

	void Start(){
		hideConfirmationPanel();
	}

	public void showConfirmationPanel(guiConfirmFunc func){
		funcToConfirm = func;
		gameObject.SetActive(true);
	}

	public void hideConfirmationPanel(){
		gameObject.SetActive(false);
	}

	public void answerYes(){
		hideConfirmationPanel();
		funcToConfirm(true);
	}

	public void answerNo(){
		hideConfirmationPanel();
	}

	void OnDestroy(){
		UnitEvents.onConfirmRequest -= showConfirmationPanel;
	}
}
