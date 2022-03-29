using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Mythical
{
	public static class ImgHandler
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002298 File Offset: 0x00000498
		public static Texture2D LoadPNG(string filePath)
		{
			Texture2D texture2D = new Texture2D(2, 2);
			byte[] data = File.ReadAllBytes(filePath);
			texture2D.LoadImage(data);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000022C4 File Offset: 0x000004C4
		public static Sprite LoadSprite(string path)
		{
			string path2 = "Sprites/" + path + ".png";
			string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path2);
			Texture2D texture2D = ImgHandler.LoadPNG(text);
			texture2D.name = text;
			texture2D.filterMode = FilterMode.Point;
			Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
			Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
			sprite.name = path;
			return sprite;
		}

	}
}
