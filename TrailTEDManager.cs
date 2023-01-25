using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Mythical
{
    public class TrailTEDManager : MonoBehaviour {

        public ParticleSystem p;
        GameObject g;
        public void Start()
        {
            g = new GameObject("Trail");
            g.transform.parent = gameObject.transform;
            g.transform.position = transform.position + new Vector3(0,0,-1f);
            p = g.AddComponent<ParticleSystem>();
            MainModule main = p.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startLifetime = 3;
            MinMaxCurve mm = main.startSpeed;
            mm.mode = ParticleSystemCurveMode.TwoConstants;
            mm.constantMin = 0.1f;
            mm.constantMax = 0.3f;

            ColorOverLifetimeModule colm= p.colorOverLifetime;
            colm.enabled = true;
            MinMaxGradient mm2 = colm.color;
            mm2.mode = ParticleSystemGradientMode.Gradient;
            mm2.gradient = new Gradient();
            GradientColorKey[] keys = new GradientColorKey[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) };
            GradientAlphaKey[] keysA = new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 0.5f), new GradientAlphaKey(0, 1) };

            mm2.gradient.SetKeys(keys, keysA);


            main.startSpeed = mm;
            colm.color = mm2;
            EmissionModule emission = p.emission;
            emission.rateOverTime = 8;
            NoiseModule noise = p.noise; 
            noise.frequency = 0.2f;
            noise.strength = 0.1f;
            noise.enabled = true;
            ShapeModule shape = p.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.25f;

            

            //PoolManager.GetPoolItem<SectionedTrailEmitter>().emitParams.
            foreach(Renderer r in FindObjectsOfType<Renderer>())
            {
                if (r.transform.root!=transform.root && r.material.shader!=null && !r.material.shader.name.ToLower().Contains("palette"))
                {
                    m = r.material;
                    break;
                }
            }

            pl = GetComponentInParent<Player>();
        }
        public Player pl;
        public Material m;
        public Texture2D s;
        public void Update()
        {
            if (ContentLoader.particles.ContainsKey(pl.outfitID))
            {
                g.SetActive(true);
                (g.GetComponent<Renderer>()).material = m;
                (g.GetComponent<Renderer>()).material.mainTexture = ContentLoader.particles[pl.outfitID];
            } else
            {
                g.SetActive(false);
            }
            
        }
    }
}
