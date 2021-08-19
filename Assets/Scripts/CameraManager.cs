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
        background.color = Color.white;
        camAvailable = true;
    }
    private void Update()
    {
        if (!camAvailable) return;
        float ratio = (float)backCamera.width / (float)backCamera.height;
        fitter.aspectRatio = ratio;

        float scaleY = backCamera.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -backCamera.videoRotationAngle;
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

            //Encode to a PNG
            byte[] bytes = photo.EncodeToPNG();
            //Write out the PNG. Of course you have to substitute your_path for something sensible
            File.WriteAllBytes(Application.persistentDataPath + "/Photo/photo_" + DateTime.Now + ".png", bytes);
            Debug.Log("Image Captured");
        }
    }
}
