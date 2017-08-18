using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDCard : MonoBehaviour {
    public RawImage _image;
    public Image _smile;
    private Texture2D result_ = null;
    public Sprite[] _smiles;
    public Text _text;
    public String _xingbie = "性别：";
    public String _boy = "男";
    public String _girl = "女";
    public String _old = "年龄：";
    public void setup(Face face, Texture2D picture) {
        _image.texture = picture;
    }
    private void attributes(Face.Attributes att) {
        // att.smile
        if (att.smile < 0.25f)
        {
            _smile.sprite = _smiles[0];
        }
        else if (att.smile < 0.5f)
        {
            _smile.sprite = _smiles[1];
        }
        else if (att.smile < 0.75f)
        {
            _smile.sprite = _smiles[2];
        }
        else
        {
            _smile.sprite = _smiles[3];
        }
        _text.text = _xingbie + (att.gender == "\"male\"" ? _boy:_girl) + "\n" + _old + Mathf.CeilToInt(att.age).ToString();

    }
    internal void setFace(Face face, Texture2D shot)
    {
       
        result_ = new Texture2D(face.faceRectangle.width, face.faceRectangle.height, TextureFormat.RGBA32, false);
        Debug.Log(shot.height);
        result_.SetPixels(shot.GetPixels(face.faceRectangle.left, shot.height - (face.faceRectangle.top + face.faceRectangle.height), face.faceRectangle.width, face.faceRectangle.height));

        result_.Apply();

        Material mater = new Material(Shader.Find("UI/Default"));
        mater.mainTexture = result_;
        _image.material = mater;
        attributes(face.faceAttributes);
    }
}
