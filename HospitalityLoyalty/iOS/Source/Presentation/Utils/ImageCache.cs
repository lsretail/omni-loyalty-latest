using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Utils
{
	public class ImageCache
	{
		private class ImageCacheItem
		{
			public string Key { get; set; }
			public ImageView Image { get; set; } 
		}

		private List<ImageCacheItem> cachedImages = new List<ImageCacheItem>();
		private const int maxSize = 1024 * 1024 * 4;  //4MB
		private int totalSize;

		public ImageView Get(string key)
		{
			var item = cachedImages.FirstOrDefault(image => image.Key == key);

			if (item != null)   //add to the back for LRU
			{
				lock (cachedImages)
				{
					cachedImages.Remove(item);
					cachedImages.Add(item);
				}

				return item.Image;
			}

			return null;
		}

		public void Put(string key, ImageView image)
		{
			lock (cachedImages)
			{
				cachedImages.Add(new ImageCacheItem(){Key = key, Image = image});

				totalSize += image.Image.Length * 3;

				if (totalSize >= maxSize)
				{
					while (totalSize >= maxSize && cachedImages.Count > 1)
					{
						var item = cachedImages[0];

						totalSize -= item.Image.Image.Length * 3;

						cachedImages.RemoveAt(0);
					}
				}
			}
		}

		public void Clear()
		{
			cachedImages.Clear();
			totalSize = 0;
		}

		/// <summary>
		/// Generates key based on ID and size.
		/// </summary>
		/// <returns>The key.</returns>
		/// <param name="image">Image.</param>
		public string GenerateKey(ImageView image)
		{
			return image.Id + "-" + image.ImgSize.Width + "x" + image.ImgSize.Height;
		}

		/// <summary>
		/// Generates key based on ID and size.
		/// </summary>
		/// <returns>The key.</returns>
		/// <param name="id">Image ID</param>
		/// <param name="size">Image size</param>
		public string GenerateKey(string id, ImageSize size)
		{
			return id + "-" + size.Width + "x" + size.Height;
		}
	}
}

// Old implementation
//using System;
//using System.Collections.Generic;
//using System.Linq;
//
//namespace Presentation.Utils
//{
//	public class ImageCache : IImageCache
//	{
//		private List<ImageCacheItem> cachedImages = new List<ImageCacheItem>();
//		private const int maxSize = 2000000;  //2MB
//		private int totalSize;
//
//		public ImageCache ()
//		{
//		}
//
//		public void AddImageToCache(ImageCacheItem imageCacheItem)
//		{
//			cachedImages.Add(imageCacheItem);
//			totalSize += imageCacheItem.Image.Length * 3;
//			if (totalSize >= maxSize)
//			{
//				lock (cachedImages)
//				{
//					while (totalSize >= maxSize)
//					{
//						var item = cachedImages[0];
//						totalSize -= item.Image.Length*3;
//						cachedImages.RemoveAt(0);
//					}
//				}
//			}
//		}
//
//		public string GetImageFromCache(string id)
//		{
//			var image = cachedImages.FirstOrDefault (x => x.ID == id);
//
//			return image == null ? null : image.Image;
//		}
//
//		public void Clear()
//		{
//			cachedImages.Clear();
//			totalSize = 0;
//		}
//	}
//}
//
