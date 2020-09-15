using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour {
  public float minY;
  public float minSize;
  public float maxY;
  public float maxSize;
  public bool isStair;

  void Start() {
    GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);

  }

}


[CustomEditor(typeof(Path))]
public class PathEditor : Editor {
  public override void OnInspectorGUI() {
    GUILayout.Label("Path", EditorStyles.boldLabel);

    Path p = (Path)target;

    GUILayout.BeginHorizontal();
    GUILayout.Label(" ", GUILayout.Width(60));
    GUI.enabled = false;
    GUILayout.TextField("Y");
    GUILayout.TextField("Size");
    GUI.enabled = true;
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Label("Min", GUILayout.Width(60));
    if (float.TryParse(GUILayout.TextField(p.minY.ToString()), out float tmp)) p.minY = tmp;
    if (float.TryParse(GUILayout.TextField(p.minSize.ToString()), out tmp)) p.minSize = tmp;
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Label("Max", GUILayout.Width(60));
    if (float.TryParse(GUILayout.TextField(p.maxY.ToString()), out tmp)) p.maxY = tmp;
    if (float.TryParse(GUILayout.TextField(p.maxSize.ToString()), out tmp)) p.maxSize = tmp;
    GUILayout.EndHorizontal();

    GUILayout.Space(8);

    GUILayout.BeginHorizontal();
    GUILayout.Label("Stair?", GUILayout.Width(60));
    p.isStair = GUILayout.Toggle(p.isStair, "");
    GUILayout.EndHorizontal();
  }
}