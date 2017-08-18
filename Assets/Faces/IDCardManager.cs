using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDCardManager : GDGeek.Singleton<IDCardManager>
{


    public DragItem _phototype = null;
    public DragManager _dragManager;
    internal void removeAll() {

    }
    internal void addFaces(Face[] faces, Texture2D shot)
    {
        for (int i = 0; i < faces.Length; ++i) {
            DragItem item = GameObject.Instantiate(_phototype);
            item._id = 0;
            item._index = i;
            item.transform.SetParent(_dragManager.transform);
            item.transform.localScale = Vector3.one;
            item.gameObject.SetActive(true);
            IDCard card = item.GetComponent<IDCard>();
            card.setFace(faces[i], shot);
            _dragManager._list.Add(item);
        }

        _dragManager.reset(faces.Length/2);
    }
}
