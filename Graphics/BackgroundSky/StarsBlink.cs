using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarsBlink : MonoBehaviour {
  public SpriteRenderer[] bsrs;
  public Image[] bis;
  float time = 0;
  bool byImages = false;
  public SpriteRenderer[] Lightnings;
  public AudioClip[] Thunders;
  public Material Normal;
  public Transform Moon;
  bool disable = false;

  private void Awake() {
    byImages = (bis != null && bis.Length == 4 && bis[0] != null);
    if (byImages) return;
    me = this;
    Lightnings[0].enabled = false;
    Lightnings[1].enabled = false;
    Lightnings[2].enabled = false;
    Moon.localPosition = new Vector3(5.9f, 3.35f, 10);
    Moon.localScale = new Vector3(.55f, .55f, 1);
    Normal.SetFloat("_Lightning", 0);
    if (storm != null) {
      StopCoroutine(storm);
      storm = null;
    }
  }

  public static void Disable(bool d) {
    if (me == null) return;
    me.disable = d;

    foreach (SpriteRenderer sr in me.bsrs)
      sr.enabled = !d;
    me.Moon.gameObject.SetActive(!d);
  }

  void Update() {
    if (disable) return;
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
    me.Lightnings[0].enabled = false;
    me.Lightnings[1].enabled = false;
    me.Lightnings[2].enabled = false;
    if (level < 0) { // not in woods. Full luminosity
      me.Normal.SetFloat("_Lightning", 0);

      Debug.Log("level=0");

      me.Moon.localScale = new Vector3(.55f, .55f, 1);
      if (storm != null) {
        me.StopCoroutine(storm);
        storm = null;
        me.Lightnings[0].enabled = false;
        me.Lightnings[1].enabled = false;
        me.Lightnings[2].enabled = false;
      }
    }
    else { // Partial luminosity
      me.Normal.SetFloat("_Lightning", -.15f * level);

      me.Moon.localScale = new Vector3(.25f, .25f, 1);
      stormProb = level - 1;
      if (storm == null) {
        storm = me.StartCoroutine(me.Storm());
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
          // FIXME play a sound
          SpriteRenderer what = Lightnings[Random.Range(0, 3)];
          AudioClip ac = Thunders[Random.Range(0, 3)];
          // Anim
          float llorig = me.Normal.GetFloat("_Lightning");
          float time = 0;
          float len = Random.Range(.1f, .25f);
          while (time < len) {
            what.color = new Color32(255, 255, 255, (byte)(255 * time / len));
            what.enabled = true;
            me.Normal.SetFloat("_Lightning", time * 10);
            time += Time.deltaTime;
            yield return null;
          }
          if (!sound.isPlaying) {
            sound.clip = ac;
            sound.Play(); 
          }
          time = 0;
          len = what == Lightnings[0] ? .1f : .5f;
          while (time < .05f) {
            time += Time.deltaTime;
            yield return null;
          }
          while (time > 0) {
            what.color = new Color32(255, 255, 255, (byte)(200 * time / len));
            time -= Time.deltaTime;
            me.Normal.SetFloat("_Lightning", time * 10);
            yield return null;
          }
          what.enabled = false;
          me.Normal.SetFloat("_Lightning", llorig);
          yield return null;
        }
      }

      yield return null;
    }
  }
}
