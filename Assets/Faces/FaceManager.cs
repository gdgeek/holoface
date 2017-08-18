using Academy.HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using GDGeek;
using System;

public class FaceManager : Academy.HoloToolkit.Unity.Singleton<FaceManager>
{
    public bool _debug= false;

    string FaceAPIKey = "b61f17bdb28b4e32bdd2918de26b1b55";
    Resolution cameraResolution_;
    PhotoCapture photoCaptureObject_ = null;
    Texture2D targetTexture_ = null;
    Renderer quadRenderer_ = null;
    bool isRunning_ = false;
    private void Awake()
    {
       // PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {

        if(photoCaptureObject_ == null) { 
            photoCaptureObject_ = captureObject;

            cameraResolution_ = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            targetTexture_ = new Texture2D(cameraResolution_.width, cameraResolution_.height, TextureFormat.RGBA32, false);
            CameraParameters c = new CameraParameters();
            c.hologramOpacity = 0.0f;

            c.cameraResolutionWidth = targetTexture_.width;
            c.cameraResolutionHeight = targetTexture_.height;
            //c.pixelFormat = CapturePixelFormat.PNG;

            c.pixelFormat = CapturePixelFormat.BGRA32;
            captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
        }
    }
    private Task photoTask(Action<Texture2D> p) {
        Task task = new Task();
      
        task.init = delegate {
            isRunning_ = true;
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        };
        task.isOver = delegate
        {
            return !isRunning_;
        };
        task.shutdown = delegate
        {
            p(targetTexture_);
        };
        return task;
    }
    internal Task photo(Action<Texture2D> p)
    {

        if (isRunning_)
        {
            return null;
        }
        else {
            return photoTask(p);
        }
        
    }

    internal Task scanning(Texture2D texture, Action<Face[]> doFace)
    {
        Face[] pface = null;
        Texture2D pShot = null;
        bool isOver = false;
        Task task = new Task();
        task.init = delegate
        {
            StartCoroutine(PostToFaceAPI(texture, delegate (Face[] faces) {
                pface = faces;
               
               // IDCardManager.Instance.addFaces(faces, texture);
                isOver = true;
            }));
        };
        task.isOver = delegate {
            return isOver;
        };

        task.shutdown = delegate
        {
            doFace(pface);
        };
        return task;
    }

    public void doFaceList(Face[] faces, Texture2D texture)
    {
        for (int i = 0; i < faces.Length; ++i)
        {
            var face = faces[i];

            var result = new Texture2D(face.faceRectangle.width, face.faceRectangle.height, TextureFormat.RGBA32, false);
           // Debug.Log(shot.height);
            result.SetPixels(texture.GetPixels(face.faceRectangle.left, texture.height - (face.faceRectangle.top + face.faceRectangle.height), face.faceRectangle.width, face.faceRectangle.height));

            result.Apply();
            //_idCard.setup(face, result);
            //theResult.material.mainTexture = result;
        }

    }
    IEnumerator<object> PostToFaceAPI(Texture2D texture, Action<Face[]> doFaces)
    {
        byte[] imageData = texture.EncodeToPNG();
        var url = "https://api.projectoxford.ai/face/v1.0/detect?returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses";
        var headers = new Dictionary<string, string>() {
            { "Ocp-Apim-Subscription-Key", FaceAPIKey },
            { "Content-Type", "application/octet-stream" }
        };

        WWW www = new WWW(url, imageData, headers);
        yield return www;
        string responseString = www.text;
        Debug.Log(2);
        JSONObject j = new JSONObject(responseString);


        //DebugManager.Instance.log(responseString);
        List<Face> facelist = new List<Face>();

        foreach (var result in j.list)
        {
            Face face = new Face();
            var a = result.GetField("faceAttributes");
            var f = a.GetField("facialHair");
            var p = result.GetField("faceRectangle");
            face.faceRectangle = new Face.Rectangle();
            face.faceRectangle.top = Mathf.CeilToInt(p.GetField("top").f);
            face.faceRectangle.left = Mathf.CeilToInt(p.GetField("left").f);
            face.faceRectangle.width = Mathf.CeilToInt(p.GetField("width").f);
            face.faceRectangle.height = Mathf.CeilToInt(p.GetField("height").f);
            face.faceAttributes = new Face.Attributes();
            face.faceAttributes.age = a.GetField("age").f;
            face.faceAttributes.gender = a.GetField("gender").ToString();
            face.faceAttributes.smile = a.GetField("smile").f;

            facelist.Add(face);
            //Debug.Log(string.Format("Gender: {0}\nAge: {1}\nMoustache: {2}\nBeard: {3}\nSideburns: {4}\nGlasses: {5}\nSmile: {6}", a.GetField("gender").str, a.GetField("age"), f.GetField("moustache"), f.GetField("beard"), f.GetField("sideburns"), a.GetField("glasses").str, a.GetField("smile")));
        }

        //DebugManager.Instance.log(":" + facelist.Count);

        // Face[] faces = JsonHelper.getJsonArray<Face>(responseString);
        // DebugManager.Instance.log(":"+faces.Length.ToString());
        doFaces(facelist.ToArray());
       // doFaceList(faces, texture);



    }


    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Camera ready");
            photoCaptureObject_.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
            isRunning_ = false;
        }
    }

 
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success) { 
            List<byte> imageBufferList = new List<byte>();
            // Copy the raw IMFMediaBuffer data into our empty byte list.
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

            // In this example, we captured the image using the BGRA32 format.
            // So our stride will be 4 since we have a byte for each rgba channel.
            // The raw image data will also be flipped so we access our pixel data
            // in the reverse order.
            int stride = 4;
            float denominator = 1.0f / 255.0f;
            List<Color> colorArray = new List<Color>();
            Debug.Log(imageBufferList.Count);
            for (int i = imageBufferList.Count - 1; i-3 >= 0; i -= stride)
            {
                float a = (int)(imageBufferList[i - 0]) * denominator;
                float r = (int)(imageBufferList[i - 1]) * denominator;
                float g = (int)(imageBufferList[i - 2]) * denominator;
                float b = (int)(imageBufferList[i - 3]) * denominator;

                colorArray.Add(new Color(r, g, b, a));
            }

            targetTexture_.SetPixels(colorArray.ToArray());
            targetTexture_.Apply();
            if (_debug) { 
                if (quadRenderer_ == null)
                {
                    GameObject p = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quadRenderer_ = p.GetComponent<Renderer>() as Renderer;
                    Debug.Log(quadRenderer_);
                    Debug.Log(Shader.Find("Unlit/Texture"));
                    quadRenderer_.material = new Material(Shader.Find("Unlit/Texture"));

                    p.transform.parent = this.transform;
                    p.transform.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
                }

                quadRenderer_.material.SetTexture("_MainTex", targetTexture_);

            }
        }
        photoCaptureObject_.StopPhotoModeAsync(OnStoppedPhotoMode);
        // Take another photo
    }

    //  StartCoroutine(PostToFaceAPI(targetTexture.EncodeToPNG()));
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject_.Dispose();
        photoCaptureObject_ = null;
        isRunning_ = false;
    }
}
