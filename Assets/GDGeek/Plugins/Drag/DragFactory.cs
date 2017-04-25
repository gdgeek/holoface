using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragFactory : MonoBehaviour {

	public DragItem _phototype = null;
	private DragManager _dManager = null;
	void Start () {
		_dManager = this.gameObject.AddComponent<DragManager> ();	
		this.gameObject.AddComponent<EasyInput> ();
		DragItem item = GameObject.Instantiate (_phototype);
		item._id = 0;
		item._index = 0;
		item.transform.SetParent (this.transform);
		item.gameObject.SetActive (true);
		_dManager._list.Add (item);

		DragItem item2 = GameObject.Instantiate (_phototype);
		item2._id = 1;
		item2._index = 1;
		item2.transform.SetParent (this.transform);
		item2.gameObject.SetActive (true);
		_dManager._list.Add (item2);



		DragItem item3 = GameObject.Instantiate (_phototype);
		item3._id = 2;
		item3._index = 2;
		item3.transform.SetParent (this.transform);
		item3.gameObject.SetActive (true);
		_dManager._list.Add (item3);
		_dManager.reset ();
	}


	// Update is called once per frame
	void Update () {
		
	}
}
