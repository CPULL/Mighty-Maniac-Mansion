using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor {
  PathNode t = null;
  float size = 1;
  Vector3 snap = Vector3.one * 0.5f;


  void OnEnable() {
    t = target as PathNode;
    size = HandleUtility.GetHandleSize(Vector2.zero) * 0.25f;
    snap = Vector3.one * 0.5f;
  }

  public override void OnInspectorGUI() {
    EditorGUIUtility.labelWidth = 70;
    EditorGUILayout.BeginHorizontal();
    t.isStair = EditorGUILayout.Toggle("Stairs", t.isStair);
    EditorGUIUtility.labelWidth = 90;
    t.showMeshLocal = EditorGUILayout.Toggle("Show mesh", t.showMeshLocal);
    EditorGUILayout.EndHorizontal();

    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.BeginHorizontal();
    t.tl = EditorGUILayout.Vector2Field("TL", t.tl);
    t.top = (PathNode)EditorGUILayout.ObjectField("Top", t.top, typeof(PathNode), true);
    t.tr = EditorGUILayout.Vector2Field("TR", t.tr);
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    t.left = (PathNode)EditorGUILayout.ObjectField("Left", t.left, typeof(PathNode), true);
    t.parent = (NavPath)EditorGUILayout.ObjectField("Parent", t.parent, typeof(NavPath), true);
    t.right = (PathNode)EditorGUILayout.ObjectField("Right", t.right, typeof(PathNode), true);
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    t.bl = EditorGUILayout.Vector2Field("BL", t.bl);
    t.down = (PathNode)EditorGUILayout.ObjectField("Down", t.down, typeof(PathNode), true);
    t.br = EditorGUILayout.Vector2Field("BR", t.br);
    EditorGUILayout.EndHorizontal();
  }

  void OnSceneGUI() {
    Handles.color = Color.red;
    size = HandleUtility.GetHandleSize(Vector2.zero) * 0.25f;
    snap = Vector3.one * 0.5f;

    if (t.left == null && Handles.Button((t.tl + t.bl) / 2, Quaternion.identity, size * .5f, size * .5f, Handles.CircleHandleCap)) {
      t.left = Instantiate(t.gameObject, t.transform.parent).GetComponent<PathNode>();
      t.left.Init(t.name + "L");
      t.left.tr = t.tl;
      t.left.br = t.bl;
      t.left.tl = t.tl + Vector2.left * 3;
      t.left.bl = t.bl + Vector2.left * 3;
      t.left.right = t;
      if (!t.parent.nodes.Contains(t.left)) t.parent.nodes.Add(t.left);
    }

    if (t.right == null && Handles.Button((t.tr + t.br) / 2, Quaternion.identity, size * .5f, size * .5f, Handles.CircleHandleCap)) {
      t.right = Instantiate(t.gameObject, t.transform.parent).GetComponent<PathNode>();
      t.right.Init(t.name + "R");
      t.right.tl = t.tr;
      t.right.bl = t.br;
      t.right.tr = t.tr + Vector2.right * 3;
      t.right.br = t.br + Vector2.right * 3;
      t.right.left = t;
      if (!t.parent.nodes.Contains(t.right)) t.parent.nodes.Add(t.right);
    }

    if (t.top == null && Handles.Button((t.tr + t.tl) / 2, Quaternion.identity, size * .5f, size * .5f, Handles.CircleHandleCap)) {
      t.top = Instantiate(t.gameObject, t.transform.parent).GetComponent<PathNode>();
      t.top.Init(t.name + "T");
      t.top.bl = t.tl;
      t.top.br = t.tr;
      t.top.tl = t.tl + Vector2.up * 2;
      t.top.tr = t.tr + Vector2.up * 2;
      t.top.down = t;
      if (!t.parent.nodes.Contains(t.top)) t.parent.nodes.Add(t.top);
    }

    if (t.down == null && Handles.Button((t.br + t.bl) / 2, Quaternion.identity, size * .5f, size * .5f, Handles.CircleHandleCap)) {
      t.down = Instantiate(t.gameObject, t.transform.parent).GetComponent<PathNode>();
      t.down.Init(t.name + "D");
      t.down.tl = t.bl;
      t.down.tr = t.br;
      t.down.bl = t.bl + Vector2.down * 2;
      t.down.br = t.br + Vector2.down * 2;
      t.down.top = t;
      if (!t.parent.nodes.Contains(t.down)) t.parent.nodes.Add(t.down);
    }


    EditorGUI.BeginChangeCheck();
    Vector2 wptl = Handles.FreeMoveHandle(t.tl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    Vector2 wptr = Handles.FreeMoveHandle(t.tr, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    Vector2 wpbr = Handles.FreeMoveHandle(t.br, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    Vector2 wpbl = Handles.FreeMoveHandle(t.bl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);

    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "Bounds");
      t.tl = wptl;
      t.tr = wptr;
      t.br = wpbr;
      t.bl = wpbl;

      // Move siblings
      if (t.left != null) {
        t.left.UpdateEdges(Vector2.zero, t.tl, t.bl, Vector2.zero);
        t.left.tr = t.tl;
        t.left.br = t.bl;
      }
      if (t.right != null) {
        t.right.UpdateEdges(t.tr, Vector2.zero, Vector2.zero, t.br);
        t.right.tl = t.tr;
        t.right.bl = t.br;
      }
      if (t.top != null) {
        t.top.UpdateEdges(Vector2.zero, Vector2.zero, t.tl, t.tr);
        t.top.bl = t.tl;
        t.top.br = t.tr;
      }
      if (t.down != null) {
        t.down.UpdateEdges(t.bl, t.br, Vector2.zero, Vector2.zero);
        t.down.tl = t.bl;
        t.down.tr = t.br;
      }

      // At angles
      if (t.left != null && t.left.top != null) t.left.top.UpdateEdges(Vector2.zero, Vector2.zero, t.tl, Vector2.zero);
      if (t.top != null && t.top.left != null) t.top.left.UpdateEdges(Vector2.zero, Vector2.zero, t.tl, Vector2.zero);

      if (t.left != null && t.left.down != null) t.left.down.UpdateEdges(Vector2.zero, t.bl, Vector2.zero, Vector2.zero);
      if (t.down != null && t.down.left != null) t.down.left.UpdateEdges(Vector2.zero, t.bl, Vector2.zero, Vector2.zero);

      if (t.right != null && t.right.top != null) t.right.top.UpdateEdges(Vector2.zero, Vector2.zero, Vector2.zero, t.tr);
      if (t.top != null && t.top.right != null) t.top.right.UpdateEdges(Vector2.zero, Vector2.zero, Vector2.zero, t.tr);

      if (t.right != null && t.right.down != null) t.right.down.UpdateEdges(t.br, Vector2.zero, Vector2.zero, Vector2.zero);
      if (t.down != null && t.down.right != null) t.down.right.UpdateEdges(t.br, Vector2.zero, Vector2.zero, Vector2.zero);

      t.UpdatePoly();
    }

  }
}