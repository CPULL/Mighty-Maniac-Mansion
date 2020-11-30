using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour {
  static Fader f;
  public Canvas canvas;
  public RawImage[] blocks;
  Color32 Transparent;
  Color32 Semi;
  Color32 Black;
  bool fading = false;

  private void Awake() {
    f = this;
    Transparent = new Color32(0, 0, 0, 0);
    Semi = new Color32(0, 0, 0, 128);
    Black = new Color32(0, 0, 0, 255);
    foreach (RawImage b in blocks)
      b.color = Transparent;
  }

  public static void FadeIn() {
    f.fading = true;
    f.StartCoroutine(f.FadeInCR());
  }
  IEnumerator FadeInCR() {
    canvas.enabled = true;
    for (int i = 0; i <= blocks.Length; i++) {
      int a = i;
      int b = i - 1;
      if (a < blocks.Length) blocks[a].color = f.Semi;
      yield return null;
      if (b >= 0) blocks[b].color = f.Black;
      yield return null;
    }
    f.fading = false;
  }

  public static void FadeOut() {
    f.fading = true;
    f.StartCoroutine(f.FadeOutCR());
  }
  IEnumerator FadeOutCR() {
    for (int i = blocks.Length; i >= 0 ; i--) {
      int a = i;
      int b = i - 1;
      if (a < blocks.Length) blocks[a].color = f.Semi;
      yield return null;
      if (b >= 0) blocks[b].color = f.Transparent;
      yield return null;
    }
    canvas.enabled = false;
    f.fading = false;
  }

  public static void RemoveFade() {
    for (int i = 0; i < f.blocks.Length; i++) {
      f.blocks[i].color = f.Transparent;
    }
    f.canvas.enabled = false;
    f.fading = false;
  }

  public static bool IsFading() {
    return f.fading;
  }

}
