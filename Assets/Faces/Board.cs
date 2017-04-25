using System;
using System.Collections;
using System.Collections.Generic;
using GDGeek;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public RawImage _image;
    public GameObject _offset;
    
    internal Task show()
    {
        Task task = new TweenTask(delegate
        {
            return TweenLocalPosition.Begin(_offset, 0.3f, Vector3.zero);
        });
        return task;
    }

    internal Task hide()
    {
        Task task = new TweenTask(delegate
        {

            return TweenLocalPosition.Begin(_offset, 0.3f, new Vector3(0, 2, 0));
        });
        return task;
    }

    internal Task show(Texture2D texture)
    {
        _image.texture = texture;
        return this.show();
    }



    internal Task error()
    {
        throw new NotImplementedException();
    }

    internal void add(Face face, Texture2D result)
    {

    }
    internal Task draw(){
        return new Task();
   }
}
