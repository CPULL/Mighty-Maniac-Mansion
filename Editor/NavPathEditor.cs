using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavPath))]
public class NavPathEditor : Editor {
  NavPath t = null;
  float size;
  Vector3 snap;

  void OnEnable() {
    t = target as NavPath;
    size = HandleUtility.GetHandleSize(t.transform.position) * 0.02f;
    snap = Vector3.one * 0.75f;
  }


  public override void OnInspectorGUI() {

    // Nodes as list
    serializedObject.Update();
    EditorGUILayout.PropertyField(serializedObject.FindProperty("nodes"));


    GUILayout.BeginHorizontal();
    // Button to fix nodes
    if (GUILayout.Button("Fix all nodes")) {
      if (t.nodes == null) t.nodes = new List<PathNode>();
      t.nodes.Clear();
      foreach (Transform tr in t.transform) {
        PathNode n = tr.GetComponent<PathNode>();
        if (n != null) t.nodes.Add(n);
      }
    }
    if (GUILayout.Button("Translate all nodes")) {
      float maxT = -10000;
      float minL = -10000;
      Vector2 min = new Vector2(minL, maxT);
      foreach (PathNode n in t.nodes) {
        if (maxT < n.tl.y) maxT = n.tl.y;
        if (maxT < n.tr.y) maxT = n.tr.y;
        if (maxT < n.bl.y) maxT = n.bl.y;
        if (maxT < n.br.y) maxT = n.br.y;
        if (minL < n.tl.x) minL = n.tl.x;
        if (minL < n.tr.x) minL = n.tr.x;
        if (minL < n.bl.x) minL = n.bl.x;
        if (minL < n.br.x) minL = n.br.x;
      }

      Vector2 pos = t.transform.parent.position;
      foreach (PathNode n in t.nodes) {
        n.tl += pos - min;
        n.tr += pos - min;
        n.bl += pos - min;
        n.br += pos - min;
      }
    }


    if (GUILayout.Button("From colliders")) {
      foreach (PathNode n in t.nodes) {
        n.UpdateEdgesFromPoly();
      }
    }


    GUILayout.EndHorizontal();

    // Toggle to show/hide nodes
    bool newval = GUILayout.Toggle(t.ShowSubNodes, "Show paths");
    if (newval != t.ShowSubNodes) {
      foreach (PathNode p in t.nodes)
        p.showMeshLocal = newval;
      t.ShowSubNodes = newval;
    }

    // Toggle to show/hide path and path handlers
    newval = GUILayout.Toggle(t.DoAStar, "A*");
    if (newval != t.DoAStar) {
      t.DoAStar = newval;
    }

    GUILayout.BeginHorizontal();
    // A*
    if (GUILayout.Button("Calculate A*")) {
      t.DoAStar = true;
      t.EditorCalculatePath();
    }
    if (GUILayout.Button("Clean A*")) {
      t.DoAStar = false;
      t.start = t.transform.parent.transform.position + Vector3.right;
      t.end = t.transform.parent.transform.position + Vector3.left;
      if (t.gizmoLines != null)
        t.gizmoLines.Clear();
      else
        t.gizmoLines = new List<Parcour>();
    }
    GUILayout.EndHorizontal();

  }

  void OnSceneGUI() {
    Handles.color = Color.cyan;

    if (!t.DoAStar) return;
    EditorGUI.BeginChangeCheck();
    Vector2 nps = Handles.FreeMoveHandle(t.start, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    Vector2 npe = Handles.FreeMoveHandle(t.end, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "AStar");
      t.start = nps;
      t.end = npe;
    }
  }

}