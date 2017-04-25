using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WebCamera : MonoBehaviour
{

    private WebCamTexture _webcamTexFront;
    private WebCamTexture _webcamTexBack;

    //前后摄像头  
    public int m_devID = 0;
    //宽高比  
    public float aspect = 9f / 16f;

    private string m_deviceName;
    public string m_photoName;
    public string m_photoPath;
    // Use this for initialization  

    public delegate void onComplete(Sprite sprite);

    public WebCamTexture webCamera
    {
        get
        {
            m_deviceName = WebCamTexture.devices[m_devID].name;

            if (m_devID == 0)
            {
                if (_webcamTexBack == null)
                {
                    // Checks how many and which cameras are available on the device  
                    foreach (WebCamDevice device in WebCamTexture.devices)
                    {
                        if (!device.isFrontFacing)
                        {
                            m_deviceName = device.name;
                            _webcamTexBack = new WebCamTexture(m_deviceName, Screen.width, (int)(Screen.width * aspect));
                        }
                    }
                }
                return _webcamTexBack;
            }
            else
            {
                if (_webcamTexFront == null)
                {
                    // Checks how many and which cameras are available on the device  
                    foreach (WebCamDevice device in WebCamTexture.devices)
                    {
                        if (device.isFrontFacing)
                        {
                            m_deviceName = device.name;
                            _webcamTexFront = new WebCamTexture(m_deviceName, Screen.width, (int)(Screen.width * aspect));
                        }
                    }
                }
                return _webcamTexFront;
            }
        }
    }

    public void cameraSwitch()
    {
        // Checks how many and which cameras are available on the device  
        foreach (WebCamDevice device in WebCamTexture.devices)
        {
            if (m_deviceName != device.name)
            {
                webCamera.Stop();
                m_devID++;
                if (m_devID >= 2) m_devID = 0;
                m_deviceName = device.name;

                Debug.Log("m_devID" + m_devID);
                Debug.Log("m_deviceName" + m_deviceName);

                webCamera.Play();
                break;
            }
        }
    }

    public void takePicture(onComplete callback)
    {

        StartCoroutine(GetTexture(callback));
    }

    //捕获照片  
    //获取截图  
    public IEnumerator GetTexture(onComplete callback)
    {
        webCamera.Pause();
        yield return new WaitForEndOfFrame();

        //save  
        Texture2D t = new Texture2D(Screen.width, (int)(Screen.width * aspect));
        t.ReadPixels(new Rect(0, 0, Screen.width, (int)(Screen.width * aspect)), 0, 0, false);
        t.Apply();
        byte[] byt = t.EncodeToPNG();
        m_photoName = Time.time + ".png";
        m_photoPath = Application.persistentDataPath + "/" + m_photoName;
        System.IO.File.WriteAllBytes(m_photoPath, byt);

        //load image  
        WWW www = new WWW("file://" + m_photoPath);
        yield return www;

        Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, Screen.width, (int)(Screen.width * aspect)), new Vector2(0.5f, 0.5f));
        //回调  
        callback(sprite);
    }

    // 录像  
    // 连续捕获照片  
    public IEnumerator SeriousPhotoes()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Texture2D t = new Texture2D(400, 300, TextureFormat.RGB24, true);
            t.ReadPixels(new Rect(0, 0, Screen.width, (int)(Screen.width * aspect)), 0, 0, false);
            t.Apply();
            print(t);
            byte[] byt = t.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/MulPhotoes/" + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png", byt);
            //          using System.Threading;  
            //          Thread.Sleep(300);  
        }
    }



}