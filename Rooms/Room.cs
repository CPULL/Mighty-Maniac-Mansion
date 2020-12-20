using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
  public string ID;
  public string RoomName;

  public float minL;
  public float maxR;
  public float minY;
  public float maxY;
  public float scalePerc = -.05f;
  public float CameraGround;
  public LightMode lights = LightMode.On;
  List<SpriteRenderer> srs;
  List<PathNode> paths;
  public float MoonSize = 1;
  public List<ActionAndCondition> actions;

  private void Start() {
    if (srs == null) {
      srs = new List<SpriteRenderer>();
      CollectAllRenderers(transform);
    }
    CollectAllPaths();
    UpdateLights();
  }

  void CollectAllRenderers(Transform tran) {
    SpriteRenderer sr = tran.GetComponent<SpriteRenderer>();
    if (sr != null) {
      bool snap = sr.material.name.IndexOf("SnapPoint") != -1;
      bool water = sr.material.name.IndexOf("Water") != -1;
      if (!snap && !water)
        srs.Add(sr);
      else if (snap)
        sr.enabled = false;
    }
    foreach (Transform t in tran)
      CollectAllRenderers(t);
  }

  void CollectAllPaths() {
    NavPath path = transform.GetComponentInChildren<NavPath>();
    if (path == null) {
      Debug.LogError("Missing path for room: " + ID);
      return;
    }
    paths = path.nodes;
  }


  internal PathNode GetPathNode(Vector3 position) {
    if (paths == null) return null;
    float minDist = float.MaxValue;
    PathNode closest = null;
    foreach(PathNode pn in paths) {
      float dist = Vector2.Distance(position, pn.Center());
      if (dist < minDist) {
        minDist = dist;
        closest = pn;
      }
    }
    return closest;
  }

  internal bool HasLights() {
    return lights == LightMode.External || GD.flashLight || (lights == LightMode.On && GD.globalLights);
  }

  internal void SetLights(LightMode light) {
    if (lights == LightMode.External) return;
    lights = light;
    UpdateLights();
  }


  internal void UpdateLights() {
    if (srs == null) {
      srs = new List<SpriteRenderer>();
      CollectAllRenderers(transform);
    }
    Material m;
    if (lights == LightMode.External) m = GD.Normal();
    else if (lights == LightMode.On && GD.globalLights) m = GD.Normal();
    else if (GD.flashLight) m = GD.FlashLight();
    else m = GD.LightOff();
    foreach (SpriteRenderer sr in srs)
      sr.material = m;

    // All actors in the room should get the material
    foreach (Actor a in GD.c.allActors)
      if (a != null && a.currentRoom == this) a.SetLight(lights);
  }
}

public enum LightMode {
  External,
  On,
  Off
}