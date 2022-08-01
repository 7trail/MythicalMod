using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Mythical
{
    public class StatusBarMod : MonoBehaviour
    {
        public PlayerStatusBar self;
        public void Update()
        {
            if (self.playerPortrait != null && ContentLoader.newPalette != null)
            {
                Material material = UnityEngine.Object.Instantiate<Material>(self.playerPortrait.material);
                material.SetFloat("_PaletteCount", 32 + ContentLoader.palettes.Count);
                material.SetTexture("_Palette", ContentLoader.newPalette);
                self.playerPortrait.material = material;
                Destroy(this);
            }
        }
    }
}
