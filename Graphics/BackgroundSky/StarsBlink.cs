using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarsBlink : MonoBehaviour {
  public SpriteRenderer[] bsrs;
  public Image[] bis;
  float time = 0;
  bool byImages = false;
  public SpriteRenderer[] Lightninghs;
  public SpriteRenderer Thunder;
  public Material Normal;
  public Transform Moon;

  private void Awake() {
    byImages = (bis != null && bis.Length == 4 && bis[0] != null);
    if (byImages) return;
    me = this;
    Lightninghs[0].enabled = false;
    Lightninghs[1].enabled = false;
    Thunder.enabled = false;
    Moon.localPosition = new Vector3(5.9f, 3.35f, 10);
    Moon.localScale = new Vector3(.55f, .55f, 1);
    Normal.SetFloat("_Lightning", 0);
    if (storm != null) {
      StopCoroutine(storm);
      storm = null;
    }
  }

  void Update() {
    time += 10 * Time.deltaTime;

    if (byImages) {
      float val = (Mathf.Sin(time * Random.Range(.95f, 1.05f)) + 1.5f) / 2.5f;
      byte c = (byte)(val * 255);
      bis[0].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(time * Random.Range(.95f, 1.05f) * 1.5f) + 2) / 3;
      c = (byte)(val * 255);
      bis[1].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(2.3f + time * Random.Range(.95f, 1.05f) * 2.5f) + 4) / 5;
      c = (byte)(val * 255);
      bis[2].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(3.3f - time * Random.Range(.975f, 1.025f) * 1.35f) + 3) / 4;
      c = (byte)(val * 255);
      bis[3].color = new Color32(c, c, c, 255);
    }
    else {
      float val = (Mathf.Sin(time * Random.Range(.95f, 1.05f)) + 1.5f) / 2.5f;
      byte c = (byte)(val * 255);
      bsrs[0].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(time * Random.Range(.95f, 1.05f) * 1.5f) + 2) / 3;
      c = (byte)(val * 255);
      bsrs[1].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(2.3f + time * Random.Range(.95f, 1.05f) * 2.5f) + 4) / 5;
      c = (byte)(val * 255);
      bsrs[2].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(3.3f - time * Random.Range(.975f, 1.025f) * 1.35f) + 3) / 4;
      c = (byte)(val * 255);
      bsrs[3].color = new Color32(c, c, c, 255);
    }
  }

  static StarsBlink me;
  static Coroutine storm = null;
  static float stormProb = 0;
  public AudioSource sound;

  public static void SetWoods(int level) {
    if (me == null) return;
    me.Lightninghs[0].enabled = false;
    me.Lightninghs[1].enabled = false;
    me.Thunder.enabled = false;
    if (level > 0) { // Partial luminosity
      me.Normal.SetFloat("_Lightning", -1f / level);
      me.Moon.localScale = new Vector3(.25f, .25f, 1);
      stormProb = level - 1;
      if (storm == null) {
        storm = me.StartCoroutine(me.Storm());
      }
    }
    else { // 0=not in woods. Full luminosity
      me.Normal.SetFloat("_Lightning", 0);
      me.Moon.localScale = new Vector3(.55f, .55f, 1);
      if (storm != null) {
        me.StopCoroutine(storm);
        storm = null;
        me.Lightninghs[0].enabled = false;
        me.Lightninghs[1].enabled = false;
        me.Thunder.enabled = false;
      }
    }
  }

  IEnumerator Storm() {
    while (true) {
      if (stormProb == 0) {
        yield return 5;
      }
      else {
        // Wait a random time, longer if the prob is small
        yield return new WaitForSeconds(Random.Range(10, 20) / stormProb);

        // Then with the specified prob decide if doing a thunder and lightning
        if (Random.Range(0, stormProb) > .9f) {
          int what = Random.Range(0, 3);
          // FIXME play a sound
          SpriteRenderer whatsr = null;
          if (what == 0) { // Thunder
            whatsr = Thunder;
          }
          else if (what == 1) { // Lightningh 1
            whatsr = Lightninghs[0];
          }
          else if (what == 2) { // Lightningh 2
            whatsr = Lightninghs[1];
          }

          // Anim
          float llorig = me.Normal.GetFloat("_Lightning");
          float time = 0;
          while (time < .1f) {
            whatsr.color = new Color32(1, 1, 1, (byte)(time * 255));
            whatsr.enabled = true;
            me.Normal.SetFloat("_Lightning", time * 10);
            time += Time.deltaTime;
            yield return null;
          }
          while (time > 0) {
            whatsr.color = new Color32(1, 1, 1, (byte)(time * 255));
            time -= Time.deltaTime;
            me.Normal.SetFloat("_Lightning", time * 10);
            yield return null;
          }
          whatsr.enabled = false;
          me.Normal.SetFloat("_Lightning", llorig);
          yield return null;
          sound.Play();
        }
      }

      yield return null;
    }
  }
}
