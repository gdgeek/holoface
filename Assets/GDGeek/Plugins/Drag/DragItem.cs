using UnityEngine;
using System.Collections;
using GDGeek;


[ExecuteInEditMode]
public class DragItem : MonoBehaviour {
	public int _id = 0;
	public int _index = 0;

	public Transform _left;
	public Transform _right;
	public Transform _mid;



	public void refreshPose(float distance , float pose){
		float val = pose + distance * _index;
		if (val <= 0) {
			if (val < -1.0f) {

				this.transform.localPosition = _left.localPosition;
				this.transform.localEulerAngles = new Vector3(0, 90, 0);
			} else if (val == 0) {
				this.transform.localPosition = _mid.localPosition;
				this.transform.localEulerAngles = _mid.localEulerAngles;
			} else {
				var r = Tween.easeOutQuart(0,1, Mathf.Abs (val));
				this.transform.localPosition = _mid.localPosition * (1 - r) + _left.localPosition * r;
				this.transform.localEulerAngles = _mid.localEulerAngles * (1 - r) + new Vector3(0, 90, 0)  * r;
			}
		} else {

			if (val > 1.0f) {
				this.transform.localPosition = _right.localPosition;
				this.transform.localEulerAngles = new Vector3(0, -90, 0);
			} else {
				var r = Tween.easeOutQuart(0,1, Mathf.Abs (val));
				this.transform.localPosition = _mid.localPosition * (1 - r) + _right.localPosition * r;
				this.transform.localEulerAngles = _mid.localEulerAngles * (1 - r) + new Vector3(0, -90, 0) * r;
               
			}
		}

	}

}
