using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDCard : MonoBehaviour {
    public RawImage _image;
    Texture2D result = null;
    public void setup(Face face, Texture2D picture) {
        _image.texture = picture;
    }
}
