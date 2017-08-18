using UnityEngine;
using System.Collections;
using GDGeek;


//[ExecuteInEditMode]
public class DragItem : MonoBehaviour {
	public int _id = 0;
	public int _index = 0;
    public DragOffset offset_ = null;
   
    public void refreshPose(float distance , float pose){
        if(offset_ == null) {

            offset_ = this.gameObject.GetComponentInParent<DragOffset>();
        }
        
        float val = pose + distance * _index;
		if (val <= 0) {
			if (val < -1.0f) {

				this.transform.localPosition = offset_._left.localPosition;
				this.transform.localEulerAngles = new Vector3(0, 90, 0);
			} else if (val == 0) {
				this.transform.localPosition = offset_._mid.localPosition;
				this.transform.localEulerAngles = offset_._mid.localEulerAngles;
			} else {
				var r = Tween.easeOutQuart(0,1, Mathf.Abs (val));
				this.transform.localPosition = offset_._mid.localPosition * (1 - r) + offset_._left.localPosition * r;
				this.transform.localEulerAngles = offset_._mid.localEulerAngles * (1 - r) + new Vector3(0, 90, 0)  * r;
			}
		} else {

			if (val > 1.0f) {
				this.transform.localPosition = offset_._right.localPosition;
				this.transform.localEulerAngles = new Vector3(0, -90, 0);
			} else {
				var r = Tween.easeOutQuart(0,1, Mathf.Abs (val));
				this.transform.localPosition = offset_._mid.localPosition * (1 - r) + offset_._right.localPosition * r;
				this.transform.localEulerAngles = offset_._mid.localEulerAngles * (1 - r) + new Vector3(0, -90, 0) * r;
               
			}
		}

	}

}
