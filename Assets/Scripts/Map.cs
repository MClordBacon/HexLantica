using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject BaseHex;
    public bool StartGenerating;
    public int Width;
    public int Heigth;
    public int Radius;

    private const float hexSize = 0.86602540378f;



    void Start () {
	    if (StartGenerating)
	    {
	        for (int x = 0; x < Width; x++)
	        {
	            for (int z = 0; z < Heigth; z++)
	            {
	                var hex = Instantiate(BaseHex);
                    hex.transform.position = new Vector3(x*hexSize*2 + (z%2==0?0:hexSize*Radius), 0, z*1.5f*Radius);
	                hex.transform.localScale = Vector3.one;
                    hex.GetComponentInChildren<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value);
	            }
	        }
	    }
	}
	

	void Update () {
		
	}
}
