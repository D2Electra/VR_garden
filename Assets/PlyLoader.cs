using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public class PlyLoader : MonoBehaviour
{
    public string fileName = "pointcloud.ply";
    public float scale = 1f;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        string[] lines = File.ReadAllLines(path);

        int vertexCount = 0;
        int headerEnd = 0;

        // читаем хедер
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("element vertex"))
            {
                vertexCount = int.Parse(lines[i].Split(' ')[2]);
            }
            if (lines[i] == "end_header")
            {
                headerEnd = i + 1;
                break;
            }
        }

        List<Vector3> points = new List<Vector3>();

        // читаем вершины
        for (int i = headerEnd; i < headerEnd + vertexCount; i++)
        {
            string[] parts = lines[i].Split(' ');
            float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float z = float.Parse(parts[2], CultureInfo.InvariantCulture);

            points.Add(new Vector3(x, y, z) * scale);
        }

        // отображаем через Particle System
        var ps = gameObject.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.maxParticles = points.Count;

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            particles[i].position = points[i];
            particles[i].startSize = 0.02f;
            particles[i].startColor = Color.white;
        }

        ps.SetParticles(particles, particles.Length);
    }
}