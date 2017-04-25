using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.WebCam;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.InteropServices;



public class WebCamManager : MonoBehaviour
{

   // public IDCard _idCard;
    string FaceAPIKey = "b61f17bdb28b4e32bdd2918de26b1b55";
    string EmotionAPIKey = "fb7bedf6699449e2b8664aa68881ce0f";
    Renderer quadRenderer_ = null;
    WebCamTexture webcamTexture = null;
    public Renderer screenshot = null;
    public Renderer theResult = null;
    Texture2D shot_ = null;
    Texture2D result_ = null;
    Texture2D targetTexture_ = null;
    // Use this for initialization
    void Start()
    {
 
        webcamTexture = new WebCamTexture();

        //如果有后置摄像头，调用后置摄像头
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            if (!WebCamTexture.devices[i].isFrontFacing)
            {
                webcamTexture.deviceName = WebCamTexture.devices[i].name;
                break;
            }
        }

        quadRenderer_ = GetComponent<Renderer>();
       
        webcamTexture.Play();
        targetTexture_ = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);
        shot_ = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);
    }
    private void Update()
    {
        Color32[] data = new Color32[webcamTexture.width * webcamTexture.height];
        webcamTexture.GetPixels32(data);
      
        byte[] bytes = Color32ArrayToByteArray(data);
     
        float denominator = 1.0f / 255.0f;
        int stride = 4;

        List < Color > colorArray = new List<Color>();
        for (int i = bytes.Length - 1; i >= 0; i -= stride)
        {
            float a = (int)(bytes[i - 0]) * denominator;
            float r = (int)(bytes[i - 1]) * denominator;
            float g = (int)(bytes[i - 2]) * denominator;
            float b = (int)(bytes[i - 3]) * denominator;

            colorArray.Add(new Color(r, g, b, a));
        }
   
        // Debug.Log(colorArray.ToArray().Length);

        targetTexture_.SetPixels(colorArray.ToArray());
      
        targetTexture_.Apply();

        quadRenderer_.material.mainTexture = targetTexture_;

        if (Input.GetKeyDown(KeyCode.Space)) {


            shot_.SetPixels32(targetTexture_.GetPixels32());
            shot_.Apply();
            screenshot.material.mainTexture = shot_;          
            StartCoroutine(PostToFaceAPI(targetTexture_.EncodeToPNG()));
        }
        //quadRenderer.material.SetTexture("_MainTex", targetTexture);
    }

    private static byte[] Color32ArrayToByteArray(Color32[] colors)
    {
        
        int length = 4 * colors.Length;
        byte[] bytes = new byte[length];

        for (int i = 0; i < colors.Length; ++i)
        {
            bytes[(colors.Length -1 -i) * 4 + 3] = colors[i].a;
            bytes[(colors.Length - 1 - i) * 4 + 2] = colors[i].r;
            bytes[(colors.Length - 1 - i) * 4 + 1] = colors[i].g;
            bytes[(colors.Length - 1 - i) * 4 + 0] = colors[i].b;
        }
       
        return bytes;
    }
    /*
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {

      
    }*/

    public GameObject status;

    string OpenFaceUrl = "http://ml.cer.auckland.ac.nz:8000";

    public GameObject framePrefab;
    Resolution cameraResolution;
    public GameObject textPrefab;

    Quaternion cameraRotation;
    public void doFaceList(Face[] faces) {
        for (int i = 0; i < faces.Length; ++i) {
            var face = faces[i];

            result_ = new Texture2D(face.faceRectangle.width, face.faceRectangle.height, TextureFormat.RGBA32, false);
            Debug.Log(shot_.height);
            result_.SetPixels(shot_.GetPixels(face.faceRectangle.left, shot_.height - (face.faceRectangle.top + face.faceRectangle.height), face.faceRectangle.width, face.faceRectangle.height));
            
            result_.Apply();
            IDCardManager.Instance.addFace(face);
            //_idCard.setup(face, result_);
            theResult.material.mainTexture = result_;
        }
   
    }
    IEnumerator<object> PostToFaceAPI(byte[] imageData)
    {
        Debug.Log(1);
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
        Debug.Log(j);
        Debug.Log(responseString);

        Face[] faces = GDGeek.JsonHelper.getJsonArray <Face>(responseString);
        doFaceList(faces);
    
        /*
        var existing = GameObject.FindGameObjectsWithTag("faceText");

        foreach (var go in existing)
        {
            Destroy(go);
        }

        existing = GameObject.FindGameObjectsWithTag("faceBounds");

        foreach (var go in existing)
        {
            Destroy(go);
        }

        if (j.list.Count == 0)
        {
            status.GetComponent<TextMesh>().text = "no faces found";
            yield break;
        }
        else
        {
            status.SetActive(false);
        }

        var faceRectangles = "";
        Dictionary<string, TextMesh> textmeshes = new Dictionary<string, TextMesh>();
        Dictionary<string, WWW> recognitionJobs = new Dictionary<string, WWW>();

        foreach (var result in j.list)
        {
            GameObject txtObject = (GameObject)Instantiate(textPrefab);
            TextMesh txtMesh = txtObject.GetComponent<TextMesh>();
            var a = result.GetField("faceAttributes");
            var f = a.GetField("facialHair");
            var p = result.GetField("faceRectangle");
            float top = -(p.GetField("top").f / cameraResolution.height - .5f);
            float left = p.GetField("left").f / cameraResolution.width - .5f;
            float width = p.GetField("width").f / cameraResolution.width;
            float height = p.GetField("height").f / cameraResolution.height;

            string id = string.Format("{0},{1},{2},{3}", p.GetField("left"), p.GetField("top"), p.GetField("width"), p.GetField("height"));
            textmeshes[id] = txtMesh;

            try
            {
                var source = new Texture2D(0, 0);
                source.LoadImage(imageData);
                var dest = new Texture2D((int)p["width"].i, (int)p["height"].i);
                dest.SetPixels(source.GetPixels((int)p["left"].i, cameraResolution.height - (int)p["top"].i - (int)p["height"].i, (int)p["width"].i, (int)p["height"].i));
                byte[] justThisFace = dest.EncodeToPNG();
                string filepath = Path.Combine(Application.persistentDataPath, "cropped.png");
                File.WriteAllBytes(filepath, justThisFace);
                Debug.Log("saved " + filepath);
                recognitionJobs[id] = new WWW(OpenFaceUrl, justThisFace);
                Debug.Log(recognitionJobs.Count + " recog jobs running");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            if (faceRectangles == "")
            {
                faceRectangles = id;
            }
            else
            {
                faceRectangles += ";" + id;
            }

            GameObject faceBounds = (GameObject)Instantiate(framePrefab);
            faceBounds.transform.position = cameraToWorldMatrix.MultiplyPoint3x4(pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(left + width / 2, top, 0)));
            faceBounds.transform.rotation = cameraRotation;
            Vector3 scale = pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(width, height, 0));
            scale.z = .1f;
            faceBounds.transform.localScale = scale;
            faceBounds.tag = "faceBounds";

            Vector3 origin = cameraToWorldMatrix.MultiplyPoint3x4(pixelToCameraMatrix.MultiplyPoint3x4(new Vector3(left + width + .1f, top, 0)));
            txtObject.transform.position = origin;
            txtObject.transform.rotation = cameraRotation;
            txtObject.tag = "faceText";
            if (j.list.Count > 1)
            {
                txtObject.transform.localScale /= 2;
            }

            txtMesh.text = string.Format("Gender: {0}\nAge: {1}\nMoustache: {2}\nBeard: {3}\nSideburns: {4}\nGlasses: {5}\nSmile: {6}", a.GetField("gender").str, a.GetField("age"), f.GetField("moustache"), f.GetField("beard"), f.GetField("sideburns"), a.GetField("glasses").str, a.GetField("smile"));
        }
        /*
           // Emotion API

           url = "https://api.projectoxford.ai/emotion/v1.0/recognize?faceRectangles=" + faceRectangles;

           headers["Ocp-Apim-Subscription-Key"] = EmotionAPIKey;

           www = new WWW(url, imageData, headers);
           yield return www;
           responseString = www.text;

           j = new JSONObject(responseString);
           Debug.Log(j);
           existing = GameObject.FindGameObjectsWithTag("emoteText");

           foreach (var go in existing)
           {
               Destroy(go);
           }

           foreach (var result in j.list) {
               var p = result.GetField("faceRectangle");
               string id = string.Format("{0},{1},{2},{3}", p.GetField("left"), p.GetField("top"), p.GetField("width"), p.GetField("height"));
               var txtMesh = textmeshes[id];
               var obj = result.GetField("scores");
               string highestEmote = "Unknown";
               float highestC = 0;
               for (int i = 0; i < obj.list.Count; i++)
               {
                   string key = obj.keys[i];
                   float c = obj.list[i].f;
                   if (c > highestC) {
                       highestEmote = key;
                       highestC = c;
                   }
               }
               txtMesh.text += "\nEmotion: " + highestEmote;
           }

           // OpenFace API

           foreach (var kv in recognitionJobs) {
               var id = kv.Key;
               www = kv.Value;
               yield return www;
               responseString = www.text;
               j = new JSONObject(responseString);
               Debug.Log(j);
               var txtMesh = textmeshes[id];
               if (j.HasField("error"))
               {
                   txtMesh.text += "\n" + j["error"].str;
               }
               else
               {
                   var d = j["data"];
                   var recogString = string.Format("\nRecognition confidence: {0}\nUPI: {1}", j["confidence"], j["uid"].str);

                   if (d.HasField("fullName"))
                   {
                       recogString += string.Format("\nName: {0}", d["fullName"].str);
                       if (d.HasField("positions") && d["positions"].Count > 0)
                       {
                           var p = d["positions"][0];
                           recogString += string.Format("\nPosition: {0}\nDepartment: {1}\nReports to: {2}", p["position"].str, p["department"]["name"].str, p["reportsTo"]["name"].str);
                       }
                   } else {
                       recogString += d;
                   }
                   txtMesh.text += recogString;
               }
           }
           */
    }


}