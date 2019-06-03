using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Source by jashan: https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html
*/
public class HiResScreenShots : MonoBehaviour {
     public int resWidth = 2550; 
     public int resHeight = 3300;
 
     private bool takeHiResShot = false;

	 private Camera _camera;

	 void Awake()
	 {
		 _camera = GetComponent<Camera>();
         // Lat, Long -37.82177, 144.96470
         // Center: Lat Long -37.81846, 144.97009
	 }
 
     public static string ScreenShotName(int width, int height) {
         return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", 
                              Application.dataPath, 
                              width, height, 
                              System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
     }
 
     public void TakeHiResShot() {
         takeHiResShot = true;
     }
 
     void LateUpdate() {
         takeHiResShot |= Input.GetKeyDown("k");
         if (takeHiResShot) {
             RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
             _camera.targetTexture = rt;
             Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
             _camera.Render();
             RenderTexture.active = rt;
             screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
             _camera.targetTexture = null;
             RenderTexture.active = null; // JC: added to avoid errors
             Destroy(rt);
             byte[] bytes = screenShot.EncodeToPNG();
             string filename = ScreenShotName(resWidth, resHeight);
             System.IO.File.WriteAllBytes(filename, bytes);
             Debug.Log(string.Format("Took screenshot to: {0}", filename));
             takeHiResShot = false;
         }
     }
 }