using GDGeek;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FaceLogic : Singleton<FaceLogic> {
    private FSM fsm_ = new FSM();
    public Board _board = null;

    State getNormal() {
        State state = new State();
        state.addAction("start", "start");
        return state;
    }

    Texture2D texture_ = null;

    State getStart()
    {
        State state = TaskState.Create(delegate {
            TaskList tl = new TaskList();
            tl.push(FaceManager.Instance.photo(delegate (Texture2D t)
            {
                texture_ = t;
            }));
            tl.push(new TaskPack(delegate { return _board.show(texture_); }));

            return tl;

        }, this.fsm_, delegate {

            if (texture_ != null){
                return "scanning";
            }
            else {
                return "error";
            }

        });
       
        return state;
    }


    State getShow()
    {
        State state = new State();
        state.addAction("over", "over");
        return state;
    }

    State getOver()
    {
        State state = TaskState.Create(delegate { return _board.hide(); }, this.fsm_, "normal");
        return state;
    }
    // Use this for initialization
    void Start ()
    {
        fsm_.addState("normal", getNormal());
        fsm_.addState("start", getStart());
        fsm_.addState("show", getShow());
        fsm_.addState("over", getOver());
        fsm_.addState("scanning", getScanning());
        fsm_.addState("noface", getNoface());
        fsm_.addState("error", getError());
        fsm_.init("normal");
    }
    private State getNoface()
    {
        State state = TaskState.Create(delegate { return new Task(); }, this.fsm_, "over");
        return state;
    }
    private State getError()
    {
        State state = TaskState.Create(delegate { return _board.error(); }, this.fsm_, "normal");
        return state;
    }

    private State getScanning()
    {

        bool hasFace = false;
        State state = TaskState.Create(delegate {
            TaskList tl = new TaskList();
            Task task = FaceManager.Instance.scanning(texture_, delegate(Face[] faces) {

                if (faces == null || faces.Length == 0) {
                    hasFace = false;
                    return;
                }
                hasFace = true;

                for (int i = 0; i < faces.Length; ++i)
                {
                    var face = faces[i];

                    var result = new Texture2D(face.faceRectangle.width, face.faceRectangle.height, TextureFormat.RGBA32, false);
                    
                    result.SetPixels(texture_.GetPixels(face.faceRectangle.left, texture_.height - (face.faceRectangle.top + face.faceRectangle.height), face.faceRectangle.width, face.faceRectangle.height));

                    result.Apply();
                    _board.add(face, result);
                }

            });
            tl.push(task);
            return task;
        }, this.fsm_, delegate {
            if (hasFace)
            {
                return "draw";
            }
            else {
                return "noface";
            }
        });
        return state;
    }

    internal void startFace()
    {
        fsm_.post("start");
    }
    internal void overFace()
    {
        fsm_.post("over");
    }
}
