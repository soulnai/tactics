using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPanelController : MonoBehaviour {

	public Text title;
	public Text descr;
	public Button yesButton;
	public Button noButton;

	private guiConfirmFunc funcYes;
	private guiConfirmFunc funcNo;
	void Awake(){
		EventManager.onRequestConfirm += showConfirmationPanel;
	}

	void Start(){
		hideConfirmationPanel();
	}

	public void showConfirmationPanel(guiConfirmFunc funcOnConfirm,guiConfirmFunc funcOnDecline){
		funcYes = funcOnConfirm;
		funcNo = funcOnDecline;
		gameObject.SetActive(true);
	}

	public void hideConfirmationPanel(){
		gameObject.SetActive(false);
	}

	public void answerYes(){
		hideConfirmationPanel();
		if(funcYes != null)
			funcYes();
	}

	public void answerNo(){
		hideConfirmationPanel();
		if(funcNo != null)
			funcNo();
	}

	void OnDestroy(){
		EventManager.onRequestConfirm -= showConfirmationPanel;
	}
}
