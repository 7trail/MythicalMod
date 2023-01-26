using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    public class TEDHeadgear : MonoBehaviour
    {
        public Headgear gear;
        public Player pl;
        public void Start()
        {
            gear = GetComponent<Headgear>();
            pl = GetComponentInParent<Player>();
            CheckRobe();
            if (ContentLoader.headgears.ContainsKey(pl.outfitID))
            {
                gear.InitHeadGearSprites();
            }
        }
        string lastRobe="";
        void CheckRobe()
        {
            if (ContentLoader.headgears.ContainsKey(pl.outfitID))
            {
                gear.spriteRenderer.gameObject.SetActive(true);
                gear.spriteVariations = ContentLoader.headgears[pl.outfitID].sprites;
                if (pl.outfitID!=lastRobe)
                {
                    gear.Initialize(pl, "TournamentEditionHeadgear", null);
                    gear.InitHeadGearSprites();
                }
                
            }
            else
            {
                gear.spriteRenderer.gameObject.SetActive(false);
            }
            lastRobe = pl.outfitID;
        }

        public void Update()
        {
            CheckRobe();
        }
    }
}
