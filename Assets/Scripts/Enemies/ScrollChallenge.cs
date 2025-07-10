// Assets/Scripts/Entities/ScrollChallenge.cs
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class ScrollChallenge : MonoBehaviour
{
    [Header("Windowed Mode Resolution")]
    [Tooltip("창 모드로 전환할 가로 해상도")]
    public int windowWidth = 800;
    [Tooltip("창 모드로 전환할 세로 해상도")]
    public int windowHeight = 600;

    [Header("Jitter Settings")]
    [Tooltip("창 위치 이동 주기 (초) — 작게 할수록 더 빠르게 흔들립니다")]
    [Range(0.01f, 1f)]
    public float jitterInterval = 0.1f;
    [Tooltip("상단 마진 (픽셀) — 너무 위로 가지 않도록")]
    public int topMargin = 50;
    [Tooltip("하단 마진 (픽셀) — 너무 아래로 가지 않도록")]
    public int bottomMargin = 0;
    [Tooltip("좌측 마진 (픽셀)")]
    public int leftMargin = 0;
    [Tooltip("우측 마진 (픽셀)")]
    public int rightMargin = 0;

    private bool challengeActive;
    private System.IntPtr windowHandle;
    private int screenW, screenH;

    #region Win32 API & Structures

    [DllImport("user32.dll")]
    private static extern bool MoveWindow(System.IntPtr hWnd,
        int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(System.IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool GetWindowPlacement(System.IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X, Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT { public int left, top, right, bottom; }

    [StructLayout(LayoutKind.Sequential)]
    private struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
    }

    private const int SW_SHOWMAXIMIZED = 3;
    private const int SW_RESTORE = 9;

    #endregion

    void Start()
    {
        // 1) 윈도우 핸들
        windowHandle = GetActiveWindow();

        // 2) 전체화면 해제 → 창 모드 + 지정 해상도
        Screen.fullScreen = false;
        Screen.SetResolution(windowWidth, windowHeight, false);

        // 3) OS 최대화 상태에서 벗어나기
        ShowWindow(windowHandle, SW_RESTORE);

        // 4) 모니터 해상도 파악
        screenW = Display.main.systemWidth;
        screenH = Display.main.systemHeight;

        // 5) 흔들기 시작
        challengeActive = true;
        StartCoroutine(JitterWindow());
    }

    IEnumerator JitterWindow()
    {
        int minX = leftMargin;
        int maxX = Mathf.Max(0, screenW - windowWidth - rightMargin);
        int minY = topMargin;
        int maxY = Mathf.Max(0, screenH - windowHeight - bottomMargin);

        while (challengeActive)
        {
            int x = Random.Range(minX, maxX + 1);
            int y = Random.Range(minY, maxY + 1);
            MoveWindow(windowHandle, x, y, windowWidth, windowHeight, true);
            yield return new WaitForSeconds(jitterInterval);
        }
    }

    void Update()
    {
        if (!challengeActive) return;

        // Unity 전체화면 전환 감지
        if (Screen.fullScreen)
        {
            EndChallenge();
            return;
        }

        // OS 창 최대화 감지
        var wp = new WINDOWPLACEMENT();
        wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
        if (GetWindowPlacement(windowHandle, ref wp) &&
            wp.showCmd == SW_SHOWMAXIMIZED)
        {
            EndChallenge();
        }
    }

    private void EndChallenge()
    {
        challengeActive = false;
        StopAllCoroutines();

        // 전체화면 복원 (원해상도로)
        var res = Screen.currentResolution;
        Screen.SetResolution(res.width, res.height, true);
        Destroy(gameObject);
    }
}
