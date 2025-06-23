using System;
using UnityEditor;
using UnityEngine;
using System.IO;

public static class Capture_Test
{
    [MenuItem("Tools/Capture Scene View with Transparency")]
    static void Capture()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("❌ 메인 카메라가 없습니다. Camera.main 태그가 설정되어 있는지 확인하세요.");
            return;
        }

        int width = Screen.width;
        int height = Screen.height;

        // 기존 카메라 설정 백업
        CameraClearFlags originalFlags = cam.clearFlags;
        Color originalBGColor = cam.backgroundColor;

        // 투명한 배경 설정
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0f, 0f, 0f, 0f); // 알파 0

        // 알파 채널을 지원하는 RenderTexture 설정
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        RenderTexture.active = rt;

        cam.Render();

        // 픽셀 읽기
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // PNG 저장
        string folder = Path.Combine(Application.dataPath, "Captures");
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, $"GameViewCapture_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());

        // 복원
        cam.targetTexture = null;
        RenderTexture.active = null;
        cam.clearFlags = originalFlags;
        cam.backgroundColor = originalBGColor;
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(tex);

        AssetDatabase.Refresh();
        Debug.Log($"✅ GameView 캡처 저장됨: {path}");
    }
}
