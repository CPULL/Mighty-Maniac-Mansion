using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Item))]
class ItemEditor : Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
    Item item = target as Item;
    if (item == null || item.transform.childCount == 0) return;
    if (GUILayout.Button("Set interaction point")) {
      Transform spawn = item.transform.GetChild(0);
      Debug.Log(spawn.name + " is at " + spawn.transform.position);
      item.HotSpot = spawn.transform.position;
    }
  }
}

[CustomEditor(typeof(Door))]
class DoorEditor : Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
    Item item = target as Item;
    if (item == null || item.transform.childCount == 0) return;
    if (GUILayout.Button("Set interaction point")) {
      Transform spawn = item.transform.GetChild(0);
      Debug.Log(spawn.name + " is at " + spawn.transform.position);
      item.HotSpot = spawn.transform.position;
    }
  }
}
