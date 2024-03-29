﻿using System;
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
		public static Texture2D LoadPNG(string filePath, bool pointFilter=false)
		{
			Texture2D texture2D = new Texture2D(2, 2);
			byte[] data = File.ReadAllBytes(filePath);
			texture2D.LoadImage(data);
			texture2D.filterMode = (pointFilter ? FilterMode.Point : FilterMode.Bilinear);
			texture2D.Apply();
			//Debug.Log("Loading with color format " + texture2D.format);
			return texture2D;
		}

		public static Texture2D LoadPNGAlt(byte[] data, bool pointFilter=false)
		{
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
			texture2D.filterMode = (pointFilter ? FilterMode.Point : FilterMode.Bilinear);
			texture2D.Apply();
			//Debug.Log("Loading with color format " + texture2D.format);
			return texture2D;
		}

		public static Texture2D LoadTex2D(string path,bool pointFilter=false, Texture2D T2D = null, bool fullPath=false)
        {
			string path2 = "Sprites/" + path + ".png";
			if (fullPath)
            {
				path2 = path;
            }
			string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path2);
			Texture2D texture2D = T2D==null? ImgHandler.LoadPNG(text,pointFilter):T2D;
			if (pointFilter)
			{
				texture2D.filterMode = FilterMode.Point;
				texture2D.Apply();
			}
			return texture2D;
		}
		


		public static byte[] LoadByteArray(string path)
        {
			string path2 = "Sprites/" + path + ".png";
			string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path2);
			return ImgHandler.LoadPNG(text).GetRawTextureData();
		}
		// Token: 0x06000014 RID: 20 RVA: 0x000022C4 File Offset: 0x000004C4

		public static AudioClip LoadClip(string path,int samples, int channels, int freq)
        {
			string path2 = "Sprites/" + path ;
			return AudioClip.Create(path2,samples,channels,freq,false);
			
		}

		public static Sprite LoadSprite(string path)
		{
			Texture2D texture2D = LoadTex2D(path,true);
			texture2D.name = path;
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
			Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
			sprite.name = path;
			return sprite;
		}

		public static Sprite LoadSpriteFull(string path)
		{
			Texture2D texture2D = LoadTex2D(path, true, fullPath: true);
			texture2D.name = path;
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
			Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
			sprite.name = path;
			return sprite;
		}

		public static Sprite LoadSprite(string path, Texture2D T2D = null)
		{
			Texture2D texture2D = T2D;
			texture2D.name = path;
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
			Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
			sprite.name = path;
			return sprite;
		}

		public static Sprite LoadSprite(string path, Vector2 pivot)
		{
			Texture2D texture2D = LoadTex2D(path);
			texture2D.name = path;
			texture2D.filterMode = FilterMode.Point;
			Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
			Sprite sprite = Sprite.Create(texture2D, rect, pivot, 16f);
			sprite.name = path;
			return sprite;
		}



	}
}
