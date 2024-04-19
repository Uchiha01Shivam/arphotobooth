using UnityEngine;
using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class KinectWebcamTexture : MonoBehaviour
{
    public Material targetMaterial; // Assign the material you want to show the Kinect data on

    private KinectSensor kinectSensor;
    private Texture2D kinectColorTexture;
    private BodyFrameReader bodyFrameReader;
    private ColorFrameReader colorFrameReader;
     private DepthFrameReader depthFrameReader; // Added DepthFrameReader variable
    private ushort[] depthData;
    public GameObject video;
     private bool screenshotTaken = false;
     public string screenshotDirectory = "Screenshots";
     public TMP_Text timer;
     public GameObject Tmer;
    public Image displayimage;
    bool appear;

    void Start()
    {
        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {
            kinectColorTexture = new Texture2D(kinectSensor.ColorFrameSource.FrameDescription.Width, kinectSensor.ColorFrameSource.FrameDescription.Height, TextureFormat.RGBA32, false);
            targetMaterial.mainTexture = kinectColorTexture;

            bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
            colorFrameReader = kinectSensor.ColorFrameSource.OpenReader();
             depthFrameReader = kinectSensor.DepthFrameSource.OpenReader(); // Open the DepthFrameReader
            depthData = new ushort[kinectSensor.DepthFrameSource.FrameDescription.LengthInPixels];

            kinectSensor.Open();
            Debug.Log(PlayerPrefs.GetString("number"));
            Debug.Log(PlayerPrefs.GetString("username"));
        }
        else
        {
            Debug.LogError("No Kinect sensor found. Make sure the Kinect is connected and the appropriate SDK is installed.");
           
        }
        StartCoroutine(scrreny());
    
    }

   IEnumerator scrreny(){
    while(true){
        if(appear){
            Tmer.SetActive(true);
            timer.text = "3";
            yield return new WaitForSeconds(1f);
             timer.text = "2";
            yield return new WaitForSeconds(1f);
             timer.text = "1";
            yield return new WaitForSeconds(1f);
             Tmer.SetActive(false);
            TakeScreenshot();
                SceneManager.LoadScene(1);
            yield return new WaitForSeconds(5f);
        }
        else{yield return null;}
    }
   }

    void Update()
    {
        if (kinectSensor != null)
        {
            if (kinectSensor.IsOpen)
            {
                using (var frame = bodyFrameReader.AcquireLatestFrame())
                using (var colorFrame = colorFrameReader.AcquireLatestFrame())
                 using (var depthFrame = depthFrameReader.AcquireLatestFrame()) 
                {
                    if (frame != null && colorFrame != null && depthFrame != null)
                    {
                        Body[] bodies = new Body[kinectSensor.BodyFrameSource.BodyCount];
                        frame.GetAndRefreshBodyData(bodies);

                        // Get the color data
                        byte[] colorData = new byte[colorFrame.FrameDescription.LengthInPixels * 4];
                        colorFrame.CopyConvertedFrameDataToArray(colorData, ColorImageFormat.Rgba);

                        // Update the color texture
                        kinectColorTexture.LoadRawTextureData(colorData);
                        kinectColorTexture.Apply();

                    

                          foreach (var body in bodies)
                        {
                            if (body.IsTracked)
                            {
                                // Get the right hand position in depth space
                                Windows.Kinect.Joint head = body.Joints[Windows.Kinect.JointType.Head];
                                DepthSpacePoint handPositionDepthSpace = kinectSensor.CoordinateMapper.MapCameraPointToDepthSpace(head.Position);

                                // Get the depth value at the hand's depth space position
                                ushort handDepthValue = GetDepthValueAtPosition(depthFrame, handPositionDepthSpace);

                                // Convert depth value to real-world depth (z position)
                                float handZPosition = handDepthValue * kinectSensor.DepthFrameSource.DepthMaxReliableDistance / ushort.MaxValue;

                                // Print the hand position in the debug console
                                Debug.Log("Right Hand Position (Depth Space): X=" + handPositionDepthSpace.X + ", Y=" + handPositionDepthSpace.Y + ", Z=" + handZPosition);
                                if(handZPosition> 100 ){
                                    
                                    video.GetComponent<Transform>().position = new Vector3(-0.17f,0.8f,-8.9f);
                                    appear = true;
                                  
                                  
                                }
                                else {
                                    video.GetComponent<Transform>().position = new Vector3(-0.17f, 0.8f, -1000f);
                                    appear = false;
                                     screenshotTaken = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }


     // Helper function to get the depth value at a specific depth space position
    private ushort GetDepthValueAtPosition(DepthFrame depthFrame, DepthSpacePoint depthSpacePosition)
    {
        int depthX = (int)depthSpacePosition.X;
        int depthY = (int)depthSpacePosition.Y;

        if (depthX >= 0 && depthX < depthFrame.FrameDescription.Width && depthY >= 0 && depthY < depthFrame.FrameDescription.Height)
        {
            ushort[] depthData = new ushort[depthFrame.FrameDescription.LengthInPixels];
            depthFrame.CopyFrameDataToArray(depthData);

            int depthIndex = depthY * depthFrame.FrameDescription.Width + depthX;
            return depthData[depthIndex];
        }

        return 0;
    }


    void OnDestroy()
    {
        if (kinectSensor != null)
        {
            if (kinectSensor.IsOpen)
            {
                kinectSensor.Close();
            }
        }
    }

   void TakeScreenshot()
    {
        // Create the screenshot directory if it doesn't exist
        string screenshotFolderPath = Path.Combine(Application.persistentDataPath, screenshotDirectory);
        if (!Directory.Exists(screenshotFolderPath))
        {
            Directory.CreateDirectory(screenshotFolderPath);
        }

        // Generate a unique file name for the screenshot using the current date and time
        string screenshotFileName = System.DateTime.Now.ToString("yyyyMMddHHmmss")+PlayerPrefs.GetString("username") + ".png";

        // Set the target texture for rendering the screenshot
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        // Create a Texture2D to read the pixels from the render texture
        Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Encode the Texture2D to a PNG image and save it to the specified directory with the generated file name
        byte[] screenshotBytes = screenshotTexture.EncodeToPNG();
        string screenshotFilePath = Path.Combine(screenshotFolderPath, screenshotFileName);
        File.WriteAllBytes(screenshotFilePath, screenshotBytes);

        // Destroy the temporary Texture2D
        Destroy(screenshotTexture);

        Debug.Log("Screenshot saved: " + screenshotFilePath);
    }

    public void SendToWHatsapp()
    {
        Process.Start("https://web.whatsapp.com/send?phone=" + 8102142526);
    }
}
