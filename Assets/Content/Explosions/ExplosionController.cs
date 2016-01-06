using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour
{
	public float loopduration = 1f;
	public float duration = 1f;
	public float speed = 2f;
	
	public Texture2D ramp;

	Material[] m;
	public AudioClip[] a;
	
	bool init = false;
	
	public void Init()
	{
		float audio_len = 0f;

		init = true;

		if (a != null && a.Length > 0 && !Settings.mute_music)
		{
			AudioSource ad = gameObject.AddComponent<AudioSource>();
			ad.clip = a[Random.Range(0, a.Length)];
			ad.loop = false;
			ad.maxDistance = 1000;
			ad.rolloffMode = AudioRolloffMode.Linear;
			ad.Play();
			audio_len = ad.clip.length + 0.1f;
		}
		
		transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
		
		Renderer[] rends = GetComponentsInChildren<Renderer>();
		m = new Material[rends.Length];
		for (int i = 0; i < m.Length; i++)
			m[i] = rends[i].material;
		
		foreach (Material mat in m)
		{
			mat.SetTexture("_RampTex", ramp);
			mat.SetFloat("_Radius", transform.localScale.x * 0.5f);
		}
		
		float r = Mathf.Sin((Time.time / loopduration) * (2 * Mathf.PI)) * 0.5f + 0.25f;
		float g = Mathf.Sin((Time.time / loopduration + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		float b = Mathf.Sin((Time.time / loopduration + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		float correction = 1f / (r + g + b);
		r *= correction;
		g *= correction;
		b *= correction;
		
		r += 1f - duration;
		g += 1f - duration;
		b += 1f - duration;
		
		foreach (Material mat in m)
			mat.SetVector("_ChannelFactor", new Vector4(r, g, b, 0));

		Destroy(gameObject, Mathf.Max (duration, Mathf.Max(duration / speed, audio_len)));
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!init)
		{
			Init ();
		}
		
		duration -= Time.deltaTime * speed;
		
		if (duration < 0)
		{
			Destroy(gameObject);
			return;
		}
		
		float r = Mathf.Sin((Time.time / loopduration) * (2 * Mathf.PI)) * 0.5f + 0.25f;
		float g = Mathf.Sin((Time.time / loopduration + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		float b = Mathf.Sin((Time.time / loopduration + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		float correction = 1f / (r + g + b);
		r *= correction;
		g *= correction;
		b *= correction;
		
		r += 1f - duration;
		g += 1f - duration;
		b += 1f - duration;
		
		foreach (Material mat in m)
			mat.SetVector("_ChannelFactor", new Vector4(r, g, b, 0));
	}
}
