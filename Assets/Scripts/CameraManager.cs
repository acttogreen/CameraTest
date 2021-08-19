using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCamera;
    private Texture defaultBackground;
    private Texture2D photo;

    public RawImage background;
    public AspectRatioFitter fitter;

    private void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No Camera Detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing) backCamera = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
        }

        if (!backCamera) return;
        backCamera.Play();
        background.texture = backCamera;
        camAvailable = true;
    }
    private float scaleY;
    private int orient;
    private void Update()
    {
        if (!camAvailable) return;
        float ratio = (float)backCamera.width / (float)backCamera.height;
        fitter.aspectRatio = ratio;

        scaleY = backCamera.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        orient = -backCamera.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

    public void TakePicture()
    {
        if (!camAvailable) return;
        StartCoroutine(SequenceTakePicture());
        IEnumerator SequenceTakePicture()
        {
            yield return new WaitForEndOfFrame();
            photo = new Texture2D(backCamera.width, backCamera.height);
            photo.SetPixels(backCamera.GetPixels());
            photo.Apply();

            string path = Application.persistentDataPath + " /Image/";
            byte[] bytes = photo.EncodeToPNG();   //Encode to a PNG
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            yield return new WaitForSeconds(0.5f);

            File.WriteAllBytes(path + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png", bytes);
            Debug.Log("Image Captured : " + DateTime.Now + ".png");
        }
    }
}
