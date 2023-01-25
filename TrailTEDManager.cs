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

        public void Start()
        {
            p = gameObject.AddComponent<ParticleSystem>();
            EmissionModule emission = p.emission;
            emission.rateOverTime = 0;
            emission.rateOverDistance = 5;
            //GetComponent<ParticleSystemRenderer>().m
        }
    }
}
