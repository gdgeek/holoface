using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyInput : MonoBehaviour {
	DragManager dManager_ = null;
	void Start(){

		dManager_ = this.gameObject.GetComponent<DragManager> ();
	}
	void OnEnable(){
		EasyTouch.On_TouchStart += On_TouchStart;
		EasyTouch.On_TouchUp += On_TouchUp;
		EasyTouch.On_TouchDown += On_TouchDown;

	}

	void OnDisable(){
		UnsubscribeEvent();
	}

	void OnDestroy(){
		UnsubscribeEvent();
	}

	void UnsubscribeEvent(){

		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchUp -= On_TouchUp;
		EasyTouch.On_TouchDown -= On_TouchDown;


	}	
	void On_TouchStart(Gesture gesture)
    {
        Debug.Log(gesture);
        Debug.Log(gesture.pickObject);
        if (gesture.pickObject != null) {
			DragItem item = gesture.pickObject.GetComponentInParent<DragItem> ();
			if (item != null) {
				dManager_.touchStart (item, gesture.GetTouchToWordlPoint (5).x);
			}
		}
	}


	void On_TouchDown( Gesture gesture){

		dManager_.touchDown(gesture.GetTouchToWordlPoint (5).x);

	}
	void On_TouchUp( Gesture gesture){
		dManager_.touchUp ();
	}
}
