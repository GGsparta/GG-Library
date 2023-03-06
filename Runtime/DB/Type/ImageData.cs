using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GGL.DB.Type
{
    /// <summary>
    /// This <see cref="Data"/> type allows you to:
    /// - Save and a <see cref="Sprite"/> in local storage
    /// - Load in cache a <see cref="Sprite"/> by ID
    /// - Access the <see cref="Sprite"/> anytime
    /// </summary>
    public class ImageData : Data
    {
        [JsonProperty("PPP")] private float _ppp; // Pixels per units

        /// <value>
        /// Access the cached sprite.
        /// </value>
        /// <remarks>If not cached yet, will cache it for you :) .</remarks>
        [JsonIgnore] public Sprite Sprite => _sprite ??= TryLoad();
        [NonSerialized] private Sprite _sprite;

        /// <summary>
        /// Instantiate a new image defined by an ID.
        /// </summary>
        /// <param name="id">Id of the picture in database.</param>
        /// <remarks>That means that your image was previously added in database. You did, right?</remarks>
        public ImageData(ulong id) : base(id) {}
        
        
        /// <summary>
        /// Instantiate a new image defined by a <see cref="Sprite"/>.
        /// </summary>
        /// <param name="sprite">Your sprite. I don't really care where it comes from.</param>
        /// <param name="id">If you want to set an ID by yourself, then go for it...</param>
        /// <remarks>Your sprite will be saved in local storage.</remarks>
        public ImageData(Sprite sprite, ulong id = default) : base(id)
        {
            if (!sprite) return;
            string folder = $"{Application.persistentDataPath}/img";
            string path = $"{folder}/{Id}";
            _ppp = sprite.pixelsPerUnit;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            File.WriteAllBytes(path, sprite.texture.EncodeToPNG());
            Database.Insert(this);
        }
        
        /// <summary>
        /// Try to create Sprite from file computed by ID.
        /// </summary>
        /// <returns></returns>
        private Sprite TryLoad()
        {
            string folder = $"{Application.persistentDataPath}/img";
            string path = $"{folder}/{Id}";

            if (!File.Exists(path))
            {
                Debug.LogError("Unable to find " + path);
                return null;
            }

            Texture2D spriteTexture = LoadTexture(path);
            return Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height),
                new Vector2(0, 0), _ppp);
        }

        /// <summary>
        /// Load a PNG or JPG file from disk to a Texture.
        /// </summary>
        /// <param name="filePath">Static path to the file.</param>
        /// <returns>Return the image tecture if the loading succeded. Otherwise, returns null.</returns>
        private static Texture2D LoadTexture(string filePath)
        {
            if (!File.Exists(filePath)) return null; // Return null if load failed
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex2D = new(2, 2);
            return tex2D.LoadImage(fileData) ? tex2D : null;
        }
    }
}