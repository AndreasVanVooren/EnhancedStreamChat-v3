using UnityEngine;

namespace EnhancedStreamChat.Utilities
{
	public class GraphicUtils
	{
		public static Texture2D? LoadTextureRaw(byte[]? file)
		{
			if (file == null || file.Length == 0)
			{
				return null;
			}

			var tex2D = new Texture2D(2, 2);
			return tex2D.LoadImage(file) ? tex2D : null;
		}

		public static Sprite? LoadSpriteRaw(byte[] image, float pixelsPerUnit = 100.0f)
		{
			return LoadSpriteFromTexture(LoadTextureRaw(image), pixelsPerUnit);
		}

		public static Sprite? LoadSpriteFromTexture(Texture2D? spriteTexture, float pixelsPerUnit = 100.0f)
		{
			return spriteTexture != null ? Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit) : null;
		}
	}
}