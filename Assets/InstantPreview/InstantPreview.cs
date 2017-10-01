// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;

namespace Gvr.Internal {
  public class InstantPreview : MonoBehaviour {
    internal static InstantPreview Instance { get; set; }

    internal const string dllName = "ip_unity_plugin";

    public enum Resolutions : int {
      Big,
      Regular,
      WindowSized,
    }
    struct ResolutionSize {
      public int width;
      public int height;
    }
    static ResolutionSize[] resolutionSizes = new ResolutionSize[] {
      new ResolutionSize() { width = 2560, height = 1440, },
      new ResolutionSize() { width = 1920, height = 1080, },
      new ResolutionSize() ,
    };

    [Tooltip("Resolution of video stream. Higher = more expensive / better visual quality.")]
    public Resolutions OutputResolution = Resolutions.Big;

    public enum MultisampleCounts {
      One,
      Two,
      Four,
      Eight,
    }

    [Tooltip("Anti-aliasing for video preview. Higher = more expensive / better visual quality.")]
    public MultisampleCounts MultisampleCount = MultisampleCounts.One;
    static int[] multisampleCounts = new int[] {
      1,
      2,
      4,
      8,
    };

    public enum BitRates {
      _2000,
      _4000,
      _8000,
      _16000,
      _24000,
      _32000,
    }

    [Tooltip("Video codec streaming bit rate. Higher = more expensive / better visual quality.")]
    public BitRates BitRate = BitRates._16000;
    static int[] bitRates = new int[] {
      2000,
      4000,
      8000,
      16000,
      24000,
      32000,
    };

    struct UnityRect {
      public float right;
      public float left;
      public float top;
      public float bottom;
    }

    struct UnityEyeViews {
      public Matrix4x4 leftEyePose;
      public Matrix4x4 rightEyePose;
      public UnityRect leftEyeViewSize;
      public UnityRect rightEyeViewSize;
    }

#if UNITY_HAS_GOOGLEVR && UNITY_EDITOR
    [DllImport(dllName)]
    private static extern bool IsConnected();

    [DllImport(dllName)]
    private static extern bool GetHeadPose(out Matrix4x4 pose, out double timestamp);

    [DllImport(dllName)]
    private static extern bool GetEyeViews(out UnityEyeViews outputEyeViews);

    [DllImport(dllName)]
    private static extern IntPtr GetRenderEventFunc();

    [DllImport(dllName)]
    private static extern void SendFrame(IntPtr renderTexture, ref Matrix4x4 pose, double timestamp, int bitRate);

    [DllImport(dllName)]
    private static extern void GetVersionString(StringBuilder dest, uint n);

    public bool IsCurrentlyConnected { get { return connected; } }

    private IntPtr renderEventFunc;
    private RenderTexture renderTexture;
    private Matrix4x4 headPose = Matrix4x4.identity;
    private double timestamp;
    private Camera leftEyeCamera;
    private Camera rightEyeCamera;
    private bool connected;

    void Awake() {
      renderEventFunc = GetRenderEventFunc();

      if (Instance != null) {
        Destroy(gameObject);
        gameObject.SetActive(false);
        return;
      }

      Instance = this;
    }

    void Start() {
      PrintInstantPreviewVersion();
      DontDestroyOnLoad(gameObject);
    }

    void Update() {
      if (!EnsureCameras()) {
        return;
      }

      var mainCamera = Camera.main;
      var mainCameraObject = mainCamera.gameObject;
      var mainCameraTransform = mainCameraObject.transform;

      var newConnectionState = IsConnected();
      if (connected && !newConnectionState) {
        Debug.Log("Disconnected from Instant Preview.");
      } else if (!connected && newConnectionState) {
        Debug.Log("Connected to Instant Preview.");
      }
      connected = newConnectionState;

      if (connected) {
        if (GetHeadPose(out headPose, out timestamp)) {
          SetEditorEmulatorsEnabled(false);
          mainCameraTransform.localRotation = Quaternion.LookRotation(headPose.GetColumn(2), headPose.GetColumn(1));
        } else {
          SetEditorEmulatorsEnabled(true);
        }

        var eyeViews = new UnityEyeViews();
        if (GetEyeViews(out eyeViews)) {
          SetTransformFromMatrix(leftEyeCamera.gameObject.transform, eyeViews.leftEyePose);
          SetTransformFromMatrix(rightEyeCamera.gameObject.transform, eyeViews.rightEyePose);

          var near = Camera.main.nearClipPlane;
          var far = Camera.main.farClipPlane;
          leftEyeCamera.projectionMatrix =
            PerspectiveMatrixFromUnityRect(eyeViews.leftEyeViewSize, near, far);
          rightEyeCamera.projectionMatrix =
            PerspectiveMatrixFromUnityRect(eyeViews.rightEyeViewSize, near, far);

          bool multisampleChanged = multisampleCounts[(int)MultisampleCount] != renderTexture.antiAliasing;

          // Adjusts render texture size.
          if (OutputResolution != Resolutions.WindowSized) {
            var selectedResolutionSize = resolutionSizes[(int)OutputResolution];
            if (selectedResolutionSize.width != renderTexture.width ||
                selectedResolutionSize.height != renderTexture.height ||
                multisampleChanged) {
              ResizeRenderTexture(selectedResolutionSize.width, selectedResolutionSize.height);
            }
          } else { // OutputResolution == Resolutions.WindowSized
            var screenAspectRatio = (float)Screen.width / Screen.height;

            var eyeViewsWidth =
              -eyeViews.leftEyeViewSize.left +
              eyeViews.leftEyeViewSize.right +
              -eyeViews.rightEyeViewSize.left +
              eyeViews.rightEyeViewSize.right;
            var eyeViewsHeight =
              eyeViews.leftEyeViewSize.top +
              -eyeViews.leftEyeViewSize.bottom;
            if (eyeViewsHeight > 0f) {
              int renderTextureHeight;
              int renderTextureWidth;
              var eyeViewsAspectRatio = eyeViewsWidth / eyeViewsHeight;
              if (screenAspectRatio > eyeViewsAspectRatio) {
                renderTextureHeight = Screen.height;
                renderTextureWidth = (int)(Screen.height * eyeViewsAspectRatio);
              } else {
                renderTextureWidth = Screen.width;
                renderTextureHeight = (int)(Screen.width / eyeViewsAspectRatio);
              }
              renderTextureWidth = renderTextureWidth & ~0x3;
              renderTextureHeight = renderTextureHeight & ~0x3;

              if (multisampleChanged ||
                  renderTexture.width != renderTextureWidth ||
                  renderTexture.height != renderTextureHeight) {
                ResizeRenderTexture(renderTextureWidth, renderTextureHeight);
              }
            }
          }
        }
      } else { // !connected
        SetEditorEmulatorsEnabled(true);

        if (renderTexture.width != Screen.width || renderTexture.height != Screen.height) {
          ResizeRenderTexture(Screen.width, Screen.height);
        }
      }
    }

    void OnPostRender() {
      if (connected && renderTexture != null) {
        var nativeTexturePtr = renderTexture.GetNativeTexturePtr();
        SendFrame(nativeTexturePtr, ref headPose, timestamp, bitRates[(int)BitRate]);
        GL.IssuePluginEvent(renderEventFunc, 69);
      }
    }

    bool EnsureCameras() {
      var mainCamera = Camera.main;
      if (!mainCamera) {
        // If the main camera doesn't exist, destroys a remaining render texture and exits.
        if (renderTexture != null) {
          Destroy(renderTexture);
          renderTexture = null;
        }
        return false;
      }

      var mainCameraObject = mainCamera.gameObject;

      // renderTexture might still be null so this creates and assigns it.
      if (renderTexture == null) {
        if (OutputResolution != Resolutions.WindowSized) {
          var selectedResolutionSize = resolutionSizes[(int)OutputResolution];
          ResizeRenderTexture(selectedResolutionSize.width, selectedResolutionSize.height);
        } else {
          ResizeRenderTexture(Screen.width, Screen.height);
        }
      }

      EnsureEyeCamera(mainCamera, ":Instant Preview Left", new Rect(0.0f, 0.0f, 0.5f, 1.0f), ref leftEyeCamera);
      EnsureEyeCamera(mainCamera, ":Instant Preview Right", new Rect(0.5f, 0.0f, 0.5f, 1.0f), ref rightEyeCamera);

      return true;
    }

    void EnsureEyeCamera(Camera mainCamera, String eyeCameraName, Rect rect, ref Camera eyeCamera) {
      // Destroys eye camera object if it's not parented to the main cameara.
      if (eyeCamera != null && eyeCamera.gameObject.transform.parent != mainCamera.gameObject.transform) {
        Destroy(eyeCamera.gameObject);
        eyeCamera = null;
      }

      // Creates eye camera object if it doesn't exist.
      if (eyeCamera == null) {
        var eyeCameraObject = new GameObject(mainCamera.gameObject.name + eyeCameraName);
        eyeCamera = eyeCameraObject.AddComponent<Camera>();
        eyeCameraObject.transform.SetParent(mainCamera.gameObject.transform, false);
      }

      eyeCamera.CopyFrom(mainCamera);
      eyeCamera.rect = rect;
      eyeCamera.targetTexture = renderTexture;

      // Match child camera's skyboxes to main camera.
      Skybox monoCameraSkybox = mainCamera.gameObject.GetComponent<Skybox>();
      Skybox customSkybox = eyeCamera.GetComponent<Skybox>();
      if (monoCameraSkybox != null) {
        if (customSkybox == null) {
          customSkybox = gameObject.AddComponent<Skybox>();
        }
        customSkybox.material = monoCameraSkybox.material;
      } else if (customSkybox != null) {
        Destroy(customSkybox);
      }
    }

    void ResizeRenderTexture(int width, int height) {
      var newRenderTexture = new RenderTexture(width, height, 16);
      newRenderTexture.antiAliasing = multisampleCounts[(int)MultisampleCount];
      if (renderTexture != null) {
        if (leftEyeCamera != null) {
          leftEyeCamera.targetTexture = null;
        }
        if (rightEyeCamera != null) {
          rightEyeCamera.targetTexture = null;
        }
        Destroy(renderTexture);
      }
      renderTexture = newRenderTexture;
    }

    private static void SetEditorEmulatorsEnabled(bool enabled) {
      foreach (var editorEmulator in FindObjectsOfType<GvrEditorEmulator>()) {
        editorEmulator.enabled = enabled;
      }
    }

    private static void PrintInstantPreviewVersion() {
      StringBuilder sb = new StringBuilder(256);
      GetVersionString(sb, (uint)sb.Capacity);
      Debug.Log("Instant Preview Version: " + sb.ToString());
    }

    private static Matrix4x4 PerspectiveMatrixFromUnityRect(UnityRect rect, float near, float far) {
      if (rect.left == rect.right || rect.bottom == rect.top || near == far ||
          near <= 0f || far <= 0f) {
        return Matrix4x4.identity;
      }
      rect.left *= near;
      rect.right *= near;
      rect.top *= near;
      rect.bottom *= near;
      var X = (2 * near) / (rect.right - rect.left);
      var Y = (2 * near) / (rect.top - rect.bottom);
      var A = (rect.right + rect.left) / (rect.right - rect.left);
      var B = (rect.top + rect.bottom) / (rect.top - rect.bottom);
      var C = (near + far) / (near - far);
      var D = (2 * near * far) / (near - far);

      var perspectiveMatrix = new Matrix4x4();
      perspectiveMatrix[0, 0] = X;
      perspectiveMatrix[0, 2] = A;
      perspectiveMatrix[1, 1] = Y;
      perspectiveMatrix[1, 2] = B;
      perspectiveMatrix[2, 2] = C;
      perspectiveMatrix[2, 3] = D;
      perspectiveMatrix[3, 2] = -1f;
      return perspectiveMatrix;
    }

    private static void SetTransformFromMatrix(Transform transform, Matrix4x4 matrix) {
      var position = matrix.GetRow(3);
      position.x *= -1;
      transform.localPosition = position;
      transform.localRotation = Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
    }
#endif
  }
}
