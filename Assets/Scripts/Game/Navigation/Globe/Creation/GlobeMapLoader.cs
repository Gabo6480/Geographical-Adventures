using System.Collections;
using System.Collections.Generic;
using Seb.Meshing;
using UnityEngine;
using static GlobeMapLoader;

public class GlobeMapLoader : MonoBehaviour
{

	public bool autoLoadOnAwake;
	public TextAsset countriesLoadFile;
	public TextAsset oceanLoadFile;
    public CountryData countryData;

    public Material countryMaterial;
	public Material oceanMaterial;

    public Transform holder;

	public bool hasLoaded { get; private set; }
	public GameObject oceanObject { get; private set; }
	public RenderObject[] countryObjects { get; private set; }

	public CountryColour[] countryColours;
	public bool updateColoursFromRenderer;

	void Awake()
	{
		if (autoLoadOnAwake)
		{
			Load();
		}
	}
    void FixedUpdate()
    {
        if (updateColoursFromRenderer)
        {
            for (int i = 0; i < countryObjects.Length; i++)
            {
                countryColours[i].colour = countryObjects[i].material.color;
            }
        }

    }

    [ContextMenu("Load Globe Map")]
    public void Load()
	{
		LoadCountries();

		// Load ocean
		SimpleMeshData oceanMesh = MeshSerializer.BytesToMesh(oceanLoadFile.bytes);
		var oceanRenderObject = MeshHelper.CreateRendererObject("Ocean", oceanMesh, oceanMaterial, holder, holder.gameObject.layer);
		oceanObject = oceanRenderObject.gameObject;
		AddCollider(oceanRenderObject);
		hasLoaded = true;
	}



	void LoadCountries()
	{

		// Load countries
		SimpleMeshData[] meshes = MeshSerializer.BytesToMeshes(countriesLoadFile.bytes);
		countryObjects = new RenderObject[meshes.Length];
		countryColours = new CountryColour[meshes.Length];
        GameObject[] allObjects = new GameObject[meshes.Length];

		Dictionary<string, Country> countryByCode = new Dictionary<string, Country>();
		float highestAvgTemp = float.MinValue;
		float lowestAvgTemp = float.MaxValue;

        foreach (Country country in countryData.Countries)
		{
            if(country.avgTemperature > highestAvgTemp)
				highestAvgTemp = country.avgTemperature;
			if(country.avgTemperature < lowestAvgTemp)
				lowestAvgTemp = country.avgTemperature;

            countryByCode.Add(country.alpha3Code, country);
        }

		float tempRange = highestAvgTemp - lowestAvgTemp;

        for (int i = 0; i < meshes.Length; i++)
		{
			Material materialInstance = new Material(countryMaterial);
			Country country = countryByCode[meshes[i].name];
            countryColours[i].countryName = country.GetPreferredDisplayName();
            countryColours[i].colour = Color.HSVToRGB(0f, 0f, 1f - Mathf.Pow((country.avgTemperature - lowestAvgTemp) / tempRange, 2));

            materialInstance.color = countryColours[i].colour;
			var countryRenderObject = MeshHelper.CreateRendererObject(countryColours[i].countryName, meshes[i], materialInstance, holder, holder.gameObject.layer);

			AddCollider(countryRenderObject);
			countryObjects[i] = countryRenderObject;
			allObjects[i] = countryRenderObject.gameObject;
			allObjects[i].isStatic = true;
		}


		StaticBatchingUtility.Combine(allObjects, holder.gameObject);



	}

	void AddCollider(RenderObject renderObject)
	{
		// Add collider for detecting mouse over
		// Bake mesh during loading so doesn't have to do it when map is first enabled
		MeshCollider meshCollider = renderObject.gameObject.AddComponent<MeshCollider>();
		Physics.BakeMesh(renderObject.filter.mesh.GetInstanceID(), convex: false);
		meshCollider.sharedMesh = renderObject.filter.mesh;
	}


	[System.Serializable]
	public struct CountryColour
	{
		public string countryName;
		public Color colour;
    }

}

