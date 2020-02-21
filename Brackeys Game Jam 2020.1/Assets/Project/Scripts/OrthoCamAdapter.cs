using UnityEngine;

[ExecuteInEditMode]
public class OrthoCamAdapter : MonoBehaviour {
    //private const float Ratio_4To3 = 1.333333f;

    private const float Ratio_16To9 = 1.777778f;
    //private const float Ratio_18To9 = 2f;

    //private const float Ratio_21To9 = 2.333334f;

    private const float defaultOrthoSize = 4.8f;
    //private const float maxOrthoSize = 5.4f;

    private Camera cam;


    private void Awake() {
        if (cam == null)
            cam = GetComponent<Camera>();
        CheckCameraOrthoSize();
        //Destroy(this);
    }

    private void Update() {
        if (Application.isEditor) {
            CheckCameraOrthoSize();
        }
    }

    public float CheckCameraOrthoSize() {
        var aspect = cam.aspect > 1 ? cam.aspect : 1 / cam.aspect;
        if (aspect > Ratio_16To9) {
            ChangeCameraOrthoSize(aspect);
        }
        else {
            cam.orthographicSize = defaultOrthoSize;
        }

        return cam.orthographicSize;
    }

    private void ChangeCameraOrthoSize(float aspect) {
        var scale = aspect / Ratio_16To9;
        cam.orthographicSize = scale * defaultOrthoSize;
    }
}