/*
 *  HM Added 5/8/2021 - Press A to take screenshot 
 *  Just attach this to an empty game object in the scene.
 *  To find where the screenshots are stored, find Assets folder and go one level up.
 *********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UnityTipsTricks
{
    public class CaptureScreenShot : MonoBehaviour
    {
        public int superSize = 1;
        private int _shotIndex = 0;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                print("Took screen shot" + $"Screenshot{_shotIndex}.png");
                ScreenCapture.CaptureScreenshot($"Screenshot{_shotIndex}.png", superSize);
                _shotIndex++;
            }
        }
    }
}
