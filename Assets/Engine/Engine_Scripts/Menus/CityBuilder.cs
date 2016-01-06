using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CityBuilder : MonoBehaviour
{
	public GameObject[] blocks;
	public GameObject tower;
	public Camera cull_0, cull_1;
	public int width = 100, height = 100;
	public bool generate;
	float progress;
	public float Progress { get { return progress; } }
	
	public int[] polys;
	public int[] low_polys;
	public int tower_polys;
	public int total_polys;
	
	public int tx, ty;
	
	// Use this for initialization
	void Start ()
	{	
		progress = 1f;
	}
	
	void OnValidate()
	{
		if (generate)
		{	
			generate = false;
			total_polys = 0;
			GameObject render = new GameObject("render");
			render.transform.parent = transform;
			
			progress = 0f;
			
			GameObject t = (GameObject)Instantiate(tower);
			t.transform.parent = render.transform;
			t.transform.localPosition = new Vector3(tx, 0f, ty);
			t.transform.Rotate(Vector3.up, 90f * Random.Range(0, 4));
			
			total_polys += tower_polys;
			
			Generate(render.transform);
		}
	}
	
	void Generate(Transform p)
	{
		Plane[] frustrums_0 = GeometryUtility.CalculateFrustumPlanes(cull_0);
		Plane[] frustrums_1 = GeometryUtility.CalculateFrustumPlanes(cull_1);
		
		float one_over_area = 1f/((height / 2 + 10) * width);
		int w_half = width / 2;
		int h_half = height / 2;
		float scale_mod = 1f / h_half;
		
		for (int x = -w_half; x < w_half; x++)
		{
			for (int y = -10; y < h_half; y++)
			{
				if (Mathf.Abs(x - tx) <= 1 && Mathf.Abs(y - ty) <= 1)
					continue;
				
				int i = Random.Range(0, blocks.Length);
				int rot = Random.Range(0, 4);
				
				GameObject block = (GameObject)Instantiate(blocks[i]);
				block.transform.parent = p;
				block.transform.localPosition = new Vector3(x, 0f, y);
				block.transform.localScale = new Vector3(1f, 1f - (y * scale_mod), 1f);
				block.transform.Rotate(Vector3.up, 90f * rot);
				
				bool visible = false;
				if (Vector3.Distance(cull_0.transform.position, block.transform.position) < cull_0.farClipPlane)
					foreach (BoxCollider r in block.GetComponentsInChildren<BoxCollider>())
						if (GeometryUtility.TestPlanesAABB(frustrums_0, r.bounds) || 
							GeometryUtility.TestPlanesAABB(frustrums_1, r.bounds))
							visible = true;
				if (!visible)
					DestroyImmediate(block);
				else
					total_polys += 
						Vector3.Distance(cull_0.transform.position, block.transform.position) > BlockLevelControl.lq_dist ?
						low_polys[i] : polys[i];
				
				progress = ((x + w_half) * (h_half + 10) + y) * one_over_area;
			}
		}
		
		progress = 1f;
	}
}
