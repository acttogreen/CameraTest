using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class CheckPermission : MonoBehaviour
{
    private void Start()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) Permission.RequestUserPermission(Permission.Camera);
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead)) Permission.RequestUserPermission(Permission.ExternalStorageRead);
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)) Permission.RequestUserPermission(Permission.ExternalStorageWrite);
#endif
    }
}

