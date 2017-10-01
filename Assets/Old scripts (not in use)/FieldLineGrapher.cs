using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldLineGrapher : MonoBehaviour {

    [Range(10, 300)]
    public int resolution;
    private float increment;
    private int currentResolution;

    private ParticleSystem.Particle[][] pointsTemp;
    private ParticleSystem.Particle[] points;

    void Start()
    {
        CreatePoints();
    }

    private void CreatePoints()
    {

        currentResolution = resolution;
        increment = 5f / (resolution - 1);

        pointsTemp = new ParticleSystem.Particle[8][];

        pointsTemp[0] = CreateSomePoints(increment, 'x', 1);
        pointsTemp[1] = CreateSomePoints(-increment, 'x', -1);
        pointsTemp[2] = CreateSomePoints(-increment, 'x', 1);
        pointsTemp[3] = CreateSomePoints(increment, 'x', -1);

        pointsTemp[4] = CreateSomePoints(increment, 'z', 1);
        pointsTemp[5] = CreateSomePoints(-increment, 'z', -1);
        pointsTemp[6] = CreateSomePoints(-increment, 'z', 1);
        pointsTemp[7] = CreateSomePoints(increment, 'z', -1);

        points = SerializeArray(pointsTemp);
       
    }

    private ParticleSystem.Particle[] SerializeArray(ParticleSystem.Particle[][] pointsTemp)
    {
        int size = 0;

        foreach(ParticleSystem.Particle[] arr in pointsTemp)
        {
            size += arr.Length;
        }

        ParticleSystem.Particle[] serializedArray = new ParticleSystem.Particle[size];
        int position = 0;

        foreach (ParticleSystem.Particle[] arr in pointsTemp)
        {
            arr.CopyTo(serializedArray, position);
            position += arr.Length;
        }

        return serializedArray;
    }

    private ParticleSystem.Particle[] CreateSomePoints(float delta, char axis, float slope)
    {
        ParticleSystem.Particle[] somePoints = new ParticleSystem.Particle[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float variable = i * delta;

            if (axis == 'x')
                somePoints[i].position = new Vector3(variable, slope*variable, 0f);
            else if (axis == 'z')
                somePoints[i].position = new Vector3(0f, slope*variable, variable);
            
            somePoints[i].startColor = new Color(Math.Abs(variable), 0f, 0f, 0.5f);
            somePoints[i].startSize = 0.1f;
        }

        return somePoints;
    }

    void Update()
    {
        if (currentResolution != resolution || points == null || pointsTemp == null
            || pointsTemp[0] == null || pointsTemp[1] == null || pointsTemp[2] == null
            || pointsTemp[3] == null)
        {
            CreatePoints();
        }

        UpdateColor(pointsTemp[0]);
        UpdateColor(pointsTemp[1]);
        UpdateColor(pointsTemp[2]);
        UpdateColor(pointsTemp[3]);

        points = SerializeArray(pointsTemp);

        GetComponent<ParticleSystem>().SetParticles(points, points.Length);
    }

    private void UpdateColor(ParticleSystem.Particle[] quadrantPoints)
    {
        for (int i = 0; i < resolution; i++)
        {
            Color c = quadrantPoints[i].startColor;

            if (resolution > 2*i)
            {
                c.r = (float)i * 2f / ((float)resolution);
            }
            else
            {
                c.g = ((float)i * 2f / ((float)resolution)) - 1f;
                c.b = ((float)i * 2f / ((float)resolution)) - 1f;
                //c.a = c.a * 0.9f;
            }

            quadrantPoints[i].startColor = c;
        }
    }
}
