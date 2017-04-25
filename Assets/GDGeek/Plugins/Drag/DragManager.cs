using UnityEngine;
using System.Collections;
using GDGeek;
using System.Collections.Generic;

public class DragManager : MonoBehaviour {

	public List<DragItem> _list = new List<DragItem>(); 

	public float _distance = 0.14f;


	class Touch{
		public float x;
		public DragItem item;

	}



	private FSM fsm_ = new FSM ();
	private float pose_ = 0.0f;
	private DragItem curr_ = null;
	private float begin_ = 0;


	public void refreshPose(float pose){

		for (int i = 0; i < _list.Count; ++i) {
			_list [i].refreshPose (this._distance, pose);
		}
	}
	private int offset(float pose){
		int n = 0;
		int r = 0;
		if (pose > 0) {
			n = Mathf.FloorToInt (pose / (_distance/2.0f));
			r = (n + 1) / 2;
		} else {
			n = Mathf.CeilToInt (pose / (_distance/2.0f));
			r =  (n - 1) / 2;
		}

		return r;
	}

	public void touchStart(DragItem item, float x){

		Touch touch = new Touch();
		touch.x = x;
		touch.item = item;
		fsm_.post ("drag", touch);

	}
	public void touchUp(){

		this.fsm_.post ("reset");

	}
	public void reset(){
        for (int i = 0; i < this._list.Count; ++i)
        {
            this._list[i]._index = i;
            this._list[i]._id = i;
        }
		this.refreshPose (pose_);	
	}
	public void touchDown(float x){
		this.fsm_.post ("down", x);

	}



	private State getInput(){
		State state = new State ();

		state.addAction ("drag", delegate(FSMEvent evt) {
			Touch touch = (Touch)(evt.obj);
			curr_ = touch.item;
			//begin_ = touch.x;
            return "drag";
		});

		return state;
	}
	private State getDrag(){
		State state = new State ();
        state.onStart += delegate
        {
            Debug.Log("drag");
        };

        state.addAction ("down", delegate(FSMEvent evt) {
			pose_ = (float)(evt.obj);
			this.refreshPose (pose_);	
		});
		state.addAction ("reset", "reset");
		return state;
	}



	private State getReset(){
		
		State state = TaskState.Create (delegate() {
			Task task = new TweenTask(delegate() {
				Tween tween = TweenValue.Begin(this.gameObject, 0.1f, pose_, 0f, delegate(float v) {
					this.refreshPose(v);
				});
				return tween;
			});
			TaskManager.PushFront(task,delegate() {
				int idx = offset(this.pose_);
				if(idx  + this._list[0]._index > 0){
					idx = -this._list[0]._index;
				}

				if(idx  + this._list[this._list.Count -1]._index < 0){
					idx = -this._list[this._list.Count -1]._index;
				}
				for (int i = 0; i < this._list.Count; ++i) {
					this._list[i]._index +=  idx;

				}
				this.pose_ = this.pose_ - idx * _distance;

			});
			TaskManager.PushBack(task, delegate {
				this.pose_ = 0;

			});
			return task;
		}, this.fsm_, "input");
	
		return state;
	}
	void Start(){
        reset();
		fsm_.addState ("input", getInput ());
		fsm_.addState ("drag", getDrag ());
		fsm_.addState ("reset", getReset ());
		fsm_.init ("input");
	}






}
