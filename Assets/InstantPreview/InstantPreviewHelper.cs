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

using System.Runtime.InteropServices;
using Gvr.Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class InstantPreviewHelper : MonoBehaviour {
#if UNITY_HAS_GOOGLEVR && UNITY_EDITOR
  [DllImport(InstantPreview.dllName)]
  private static extern bool IsConnected();

  [DllImport(InstantPreview.dllName)]
  private static extern bool IsAdbAvailable();

  void Awake() {
    // Triggers the Instant Preview dll to load.
    IsConnected();
    if (!IsAdbAvailable()) {
      Debug.LogError("Adb Not Detected.  Please add adb to your path and restart the Unity editor.");
    }
  }
#elif UNITY_EDITOR
  void Awake() {
    print("Instant Preview is disabled; set target platform to Android to use it.");
  }

#endif
}

#if !UNITY_HAS_GOOGLEVR && UNITY_EDITOR
[CustomEditor(typeof(InstantPreviewHelper))]
public class InstantPreviewHelperEditor : Editor {
  public override void OnInspectorGUI() {
    EditorGUILayout.LabelField("Instant Preview is disabled; set target platform to Android to use it.");

  }

}
#endif
