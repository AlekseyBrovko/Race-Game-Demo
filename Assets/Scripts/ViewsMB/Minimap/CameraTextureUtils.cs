using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CameraTextureUtils
{
    private const float FAR_PLANE_TRESHOLD = 50f;

    public static Texture2D CreateScreen(Color backgroundColor, float cameraHeight, Rect rect)
    {
        if (rect == default)
            return null;

        Camera camera = new GameObject("Screenshot camera").AddComponent<Camera>();


        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = backgroundColor;

        camera.transform.position = new Vector3(rect.center.x, cameraHeight, rect.center.y);

        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = cameraHeight + FAR_PLANE_TRESHOLD;
        camera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        camera.orthographicSize = Mathf.Max(rect.size.x, rect.size.y) / 2;

        // need to get xyz -> xz vector 
        Vector3 bottomLeft = rect.center.ToXZVector() - rect.size.ToXZVector() / 2;
        Vector3 topRight = rect.center.ToXZVector() + rect.size.ToXZVector() / 2;

        Vector3 screenBottomLeft = camera.WorldToScreenPoint(bottomLeft);
        Vector3 screenTopRight = camera.WorldToScreenPoint(topRight);

        float width = screenTopRight.x - screenBottomLeft.x;
        float height = screenTopRight.y - screenBottomLeft.y;

        return CreateTextureFromCamera(camera, new Rect(screenBottomLeft, new Vector2(width, height)));
    }

    private static Texture2D CreateTextureFromCamera(Camera camera, Rect pixelRect)
    {
        int width = Mathf.CeilToInt(pixelRect.width / 4f);
        int height = Mathf.CeilToInt(pixelRect.height / 4f);

        RenderTexture rt = new RenderTexture(
            width,
            height,
            GraphicsFormat.R8G8B8A8_SRGB,
            GraphicsFormat.D24_UNorm,
            1
        );

        camera.ResetAspect();

        Texture2D cameraTexture = new Texture2D(width, height, TextureFormat.RGBA32, 1, false);
        camera.targetTexture = rt;
        RenderTexture.active = rt;

        camera.Render();

        cameraTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
        cameraTexture.Apply();

        rt.Release();

        Object.DestroyImmediate(camera.gameObject);
        return cameraTexture;
    }
}