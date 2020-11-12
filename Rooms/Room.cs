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
  public LightMode lights = LightMode.GlobalLightsOn;
  List<SpriteRenderer> srs;
  List<PathNode> paths;

  private void Start() {
    srs = new List<SpriteRenderer>();
    CollectAllRenderers(transform);
    CollectAllPaths();
  }

  void CollectAllRenderers(Transform tran) {
    SpriteRenderer sr = tran.GetComponent<SpriteRenderer>();
    if (sr != null) {
      if (sr.material.name.IndexOf("SceneSelectionPoint") == -1)
        srs.Add(sr);
      else
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


  /*
  If the room is external then there is no need to change the lights
  If the power is off all rooms should go dark
  If the current room light is switchable, the power is on of there is global power
  If we are in a room with windows and the light is off, then there is a little bit more visibility
  If the lights are off and we have a flash light we should enable the flashlight shader


  External
  GlobalLightsOn
  GlobalLightsFlashlight
  GlobalLightsOff
  LocalLightsOn
  LocalLightsFlashlight
  LocalLightsOff

   
   */

  internal bool HasLights() {
    switch (lights) {
      case LightMode.External: return true;
      case LightMode.GlobalLightsOn: return true;
      case LightMode.GlobalLightsFlashlight: return false;
      case LightMode.GlobalLightsOff: return false;
      case LightMode.LocalLightsOn: return true;
      case LightMode.LocalLightsFlashlight: return false;
      case LightMode.LocalLightsOff: return false;
    }
    return true;
  }

  internal void SetLights(bool on, bool flashlightUsed) {
    switch (lights) {
      case LightMode.External: return;

      case LightMode.GlobalLightsOn:
        if (on) lights = LightMode.GlobalLightsOn;
        if (!on && flashlightUsed) lights = LightMode.GlobalLightsFlashlight;
        if (!on && !flashlightUsed) lights = LightMode.GlobalLightsOff;
        break;

      case LightMode.GlobalLightsFlashlight:
        if (on) lights = LightMode.GlobalLightsOn;
        if (!on && flashlightUsed) lights = LightMode.GlobalLightsFlashlight;
        if (!on && !flashlightUsed) lights = LightMode.GlobalLightsOff;
        break;

      case LightMode.GlobalLightsOff:
        if (on) lights = LightMode.GlobalLightsOn;
        if (!on && flashlightUsed) lights = LightMode.GlobalLightsFlashlight;
        if (!on && !flashlightUsed) lights = LightMode.GlobalLightsOff;
        break;

      case LightMode.LocalLightsOn:
        if (on) lights = LightMode.LocalLightsOn;
        if (!on && flashlightUsed) lights = LightMode.LocalLightsFlashlight;
        if (!on && !flashlightUsed) lights = LightMode.LocalLightsOff;
        break;

      case LightMode.LocalLightsFlashlight:
        if (on) lights = LightMode.LocalLightsOn;
        if (!on && flashlightUsed) lights = LightMode.LocalLightsFlashlight;
        if (!on && !flashlightUsed) lights = LightMode.LocalLightsOff;
        break;

      case LightMode.LocalLightsOff:
        if (on) lights = LightMode.LocalLightsOn;
        if (!on && flashlightUsed) lights = LightMode.LocalLightsFlashlight;
        if (!on && !flashlightUsed) lights = LightMode.LocalLightsOff;
        break;
    }
    UpdateLights();
  }

  internal void RemoveFlashLight() {
    switch (lights) {
      case LightMode.External:
      case LightMode.GlobalLightsOn:
      case LightMode.GlobalLightsOff:
      case LightMode.LocalLightsOn:
      case LightMode.LocalLightsOff:
        return;

      case LightMode.GlobalLightsFlashlight:
        lights = LightMode.GlobalLightsOff;
        break;

      case LightMode.LocalLightsFlashlight:
        lights = LightMode.LocalLightsOff;
        break;
    }
    UpdateLights();
  }



  internal void UpdateLights() {
    Material m = null;
    switch (lights) {
      case LightMode.External: m = GD.Normal(); break;
      case LightMode.GlobalLightsOn: m = GD.Normal(); break;
      case LightMode.GlobalLightsFlashlight: m = GD.FlashLight(); break;
      case LightMode.GlobalLightsOff: m = GD.LightOffRoom(); break;
      case LightMode.LocalLightsOn: m = GD.Normal(); break;
      case LightMode.LocalLightsFlashlight: m = GD.FlashLight(); break;
      case LightMode.LocalLightsOff: m = GD.LightOffRoom(); break;
    }
    foreach (SpriteRenderer sr in srs)
      sr.material = m;

    // All actors in the room should get the material
    foreach (Actor a in GD.c.allActors)
      if (a != null && a.currentRoom == this) a.SetLight(lights);
  }
}

public enum LightMode {
  External,
  GlobalLightsOn,
  GlobalLightsFlashlight,
  GlobalLightsOff,
  LocalLightsOn,
  LocalLightsFlashlight,
  LocalLightsOff,
}