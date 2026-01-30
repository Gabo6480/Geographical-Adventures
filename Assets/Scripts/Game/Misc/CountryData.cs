using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Data/Country Data")]
public class CountryData : ScriptableObject
{
	[Header("Source")]
	[SerializeField] TextAsset countryFile;
    [SerializeField] TextAsset statesFile;
    [SerializeField] TextAsset cityFile;
	[SerializeField] TextAsset capitalsFile;
	[SerializeField] TextAsset countryAvgTempFile;

	[Header("Data")]
	[SerializeField] Country[] countries;


	public Country[] Countries
	{
		get
		{
			return countries;
		}
	}

	public int NumCountries
	{
		get
		{
			return countries.Length;
		}
	}

	public void Load()
	{
		Debug.Log("Load");
		countries = CountryLoader.LoadCountriesFromFiles(countryFile, statesFile, cityFile, capitalsFile, countryAvgTempFile);
    }

}
