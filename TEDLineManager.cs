using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Mythical
{
    public class TEDLineManager : MonoBehaviour
    {

        public TrailRenderer p;
        GameObject g;
        public void Start()
        {
            g = new GameObject("Line");
            g.transform.parent = gameObject.transform;
            g.transform.position = transform.position + new Vector3(0, 0, -1f);
            p = g.AddComponent<TrailRenderer>();
            
            p.time = 0.5f;

            
            p.widthCurve = new AnimationCurve(new Keyframe[] {new Keyframe(0f,0), new Keyframe(0.2f, 0.7f), new Keyframe(0.8f, 0.3f),new Keyframe(1, 0) });

            



            //PoolManager.GetPoolItem<SectionedTrailEmitter>().emitParams.
            foreach (Renderer r in FindObjectsOfType<Renderer>())
            {
                if (r.transform.root != transform.root && r.material.shader != null && r.material.shader.name.ToLower().Contains("particle"))
                {
                    m = new Material(r.material.shader);
                    p.material = m;
                    Debug.Log("I GOT THE MATERIAL EEEE");
                    break;
                }
            }

            pl = GetComponentInParent<Player>();

        }
        GradientAlphaKey[] keysA;
        public Player pl;
        public Material m;
        public Texture2D s;
        public void Update()
        {
            if (ContentLoader.trails.ContainsKey(pl.outfitID))
            {
                g.SetActive(true);
                p.startColor = ContentLoader.trails[pl.outfitID];
                p.endColor = ContentLoader.trails2[pl.outfitID];

            }
            else
            {
                g.SetActive(false);
            }

        }
    }
}
