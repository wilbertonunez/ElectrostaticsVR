using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapher1 : MonoBehaviour {

    [Range(10,100)]
    public int resolution = 50;
    private int currentResolution;
    private ParticleSystem.Particle[] points;

	void Start () {
        CreatePoints();
	}

    private void CreatePoints(){

        currentResolution = resolution;

        points = new ParticleSystem.Particle[resolution];

        float increment = 1f / (resolution - 1);
        for (int i = 0; i < resolution; i++)
        {
            float x = i * increment;
            points[i].position = new Vector3(x, 0f, 0f);
            points[i].startColor = new Color(x, 0f, 0f);
            points[i].startSize = 0.1f;
        }
    }
	
	void Update () {
        if(currentResolution != resolution || points == null)
        {
            CreatePoints();
        }

        for(int i=0; i< resolution; i++){
            Vector3 p = points[i].position;
            p.y = Sine(p.x);
            points[i].position = p;
            Color c = points[i].color;
            c.g = p.y;
            points[i].startColor = c;
        }

        GetComponent<ParticleSystem>().SetParticles(points, points.Length);
    }

    private static float Sine(float x){
        return 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * x);
    }
}
