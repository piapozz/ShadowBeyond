using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ImageScriptable")]
public class ImageScriptable : ScriptableObject
{
    public List<Texture> textureList = new List<Texture>();
}
