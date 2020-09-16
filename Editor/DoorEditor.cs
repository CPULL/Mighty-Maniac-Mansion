using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Interactable))]
class ItemEditor : Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
    Interactable item = target as Interactable;
    if (item == null || item.transform.childCount == 0) return;
    if (GUILayout.Button("Set interaction point")) {
      Transform spawn = item.transform.GetChild(0);
      Debug.Log(spawn.name + " is at " + spawn.transform.position);
      item.InteractionPosition = spawn.transform.position;
      item.InteractionPosition.z = 0;
    }
  }
}

[CustomEditor(typeof(Door))]
class DoorEditor : Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
    Interactable item = target as Interactable;
    if (item == null || item.transform.childCount == 0) return;
    if (GUILayout.Button("Set interaction point")) {
      Transform spawn = item.transform.GetChild(0);
      Debug.Log(spawn.name + " is at " + spawn.transform.position);
      item.InteractionPosition = spawn.transform.position;
      item.InteractionPosition.z = 0;
    }
  }
}
