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
    if (GUILayout.Button("Create node")) {
      if (t.nodes == null) t.nodes = new List<PathNode>();
      t.nodes.Clear();
      foreach (Transform tr in t.transform) {
        PathNode n = tr.GetComponent<PathNode>();
        if (n != null) t.nodes.Add(n);
      }

      // Actual creation
      PathNode pn = Instantiate(t.EmptyPathNode, t.transform).GetComponent<PathNode>();
      pn.name = "Node";
      pn.parent = t;
      t.nodes.Add(pn);
      pn.UpdateEdgesFromPoly(t.transform.parent.position);
    }


    if (GUILayout.Button("From colliders")) {
      foreach (PathNode n in t.nodes) {
        if (n == null) continue;
        n.tl.y = 1;
        n.tr.y = 1;
        n.bl.y = 0;
        n.br.y = 0;
        n.tl.x = -1;
        n.tr.x = 1;
        n.bl.x = -1;
        n.br.x = 1;
        n.UpdateEdgesFromPoly(t.transform.parent.position);
      }
    }


    GUILayout.EndHorizontal();

    // Toggle to show/hide path and path handlers
    GUILayout.BeginHorizontal();
    bool newval = GUILayout.Toggle(t.ShowSubNodes, "Show paths");
    if (newval != t.ShowSubNodes) {
      foreach (PathNode p in t.nodes)
        p.showMeshLocal = newval;
      t.ShowSubNodes = newval;
    }
    EditorGUILayout.PropertyField(serializedObject.FindProperty("EmptyPathNode"));
    GUILayout.EndHorizontal();

    // A*
    GUILayout.BeginHorizontal();
    newval = GUILayout.Toggle(t.DoAStar, "A*");
    if (newval != t.DoAStar) {
      t.DoAStar = newval;
    }

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