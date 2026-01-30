using System.Drawing;
using UnityEngine;

public class CountryIndexDisplay : MonoBehaviour
{
	public FilterMode filterMode;
	public MeshRenderer display;
	public int textureWidth;
	RenderTexture texture;


	void Start()
	{
		texture = FindFirstObjectByType<CountryIndexMapper>().CreateCountryIndexMap(textureWidth, textureWidth / 2);
		texture.filterMode = filterMode;
		display.material.mainTexture = texture;
	}

    [ContextMenu("Save Files")]
    void SaveFiles()
    {
        string folderPath = Application.dataPath + "/Data/Country Indices/";
        Debug.Log("Saving Country Index Map to: " + folderPath);
        FileHelper.SaveRenderTextureToPNG(folderPath + "Country Index Map.png", texture, (uint)textureWidth / 2, (uint)textureWidth);
    }

    void OnDestroy()
	{
		ComputeHelper.Release(texture);
	}

}
