using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CountryLoader : MonoBehaviour
{
	public bool autoRunOnAwake = true;
	public TextAsset countryFile;
	public TextAsset statesFile;
	public TextAsset cityFile;
	public TextAsset capitalsFile;
    public TextAsset countryAvgTempFile;

    [SerializeField] Country[] countries;
	bool loaded;

	[ContextMenu("Run")]
	void Awake()
	{
		if (autoRunOnAwake)
		{
			Load();
		}
	}

	public Country[] GetCountries()
	{
		Load();
		return countries;
	}

	public int NumCountries
	{
		get
		{
			Load();
			return countries.Length;
		}
	}

	public void Load()
	{
		if (!loaded || !Application.isPlaying)
		{
			this.countries = LoadCountriesFromFiles(countryFile, statesFile, cityFile, capitalsFile, countryAvgTempFile);
            loaded = true;
		}
	}

	public static Country[] LoadCountriesFromFiles(TextAsset countryFile, TextAsset statesFile, TextAsset cityFile, TextAsset capitalsFile, TextAsset countryAvgTempFile)
	{
        Country[] countries = new Country[0];

        if (countryFile != null)
        {
            CountryReader countryReader = new CountryReader();
            countries = countryReader.ReadCountries(countryFile, countryAvgTempFile);
        }

        if (statesFile != null)
        {
            StateReader stateReader = new StateReader();
            State[] states = stateReader.ReadStates(statesFile);
            AddStatesToCountries(states, countries);
        }

        if (cityFile != null)
        {
            CityReader cityReader = new CityReader();
            City[] allCities = cityReader.ReadCities(cityFile, capitalsFile);
            AddCitiesToCountries(allCities, countries);
        }

        return countries;
    }

    public static void AddCitiesToCountries(City[] allCities, Country[] countries)
	{
		// Can happen due to mismatching country codes in files
		int numCountriesWithoutCity = 0;
		int numCitiesWithoutCountry = 0;

		HashSet<string> legitateCountryCodes = new HashSet<string>(countries.Select(x => x.alpha3Code));

		var citiesByCountry = new Dictionary<string, List<City>>();

		foreach (City city in allCities)
		{
			string countryCode = city.countryAlpha3Code;
			if (legitateCountryCodes.Contains(countryCode))
			{

				if (!citiesByCountry.ContainsKey(countryCode))
				{
					citiesByCountry.Add(countryCode, new List<City>());
				}
				citiesByCountry[countryCode].Add(city);
			}
			else
			{
				Debug.LogWarning("City without valid country code: " + city.name + " (" + countryCode + ")");
                numCitiesWithoutCountry++;
			}
		}


		foreach (Country country in countries)
		{
			List<City> citiesInCountry = new List<City>();
			if (citiesByCountry.TryGetValue(country.alpha3Code, out citiesInCountry))
			{
				country.cities = citiesInCountry.ToArray();
			}
			else
			{
				country.cities = new City[0];
				numCountriesWithoutCity++;
			}
		}

		Debug.Log("Num countries without a city: " + numCountriesWithoutCity + " Num cities without a country: " + numCitiesWithoutCountry);
	}

    public static void AddStatesToCountries(State[] allStates, Country[] countries)
    {
        // Can happen due to mismatching country codes in files
        int numCountriesWithoutStates = 0;
        int numStatesWithoutCountry = 0;

        HashSet<string> legitateCountryCodes = new HashSet<string>(countries.Select(x => x.alpha3Code));

        var statesByCountry = new Dictionary<string, List<State>>();

        foreach (State state in allStates)
        {
            string countryCode = state.countryAlpha3Code;
            if (legitateCountryCodes.Contains(countryCode))
            {

                if (!statesByCountry.ContainsKey(countryCode))
                {
                    statesByCountry.Add(countryCode, new List<State>());
                }
                statesByCountry[countryCode].Add(state);
            }
            else
            {
                numStatesWithoutCountry++;
            }
        }


        foreach (Country country in countries)
        {
            List<State> statesInCountry = new List<State>();
            if (statesByCountry.TryGetValue(country.alpha3Code, out statesInCountry))
            {
                country.states = statesInCountry.ToArray();
            }
            else
            {
                country.states = new State[0];
                numCountriesWithoutStates++;
            }
        }

        Debug.Log("Num countries without a state: " + numCountriesWithoutStates + " Num states without a country: " + numStatesWithoutCountry);
    }

    /*
	[ContextMenu("Create Country Info Template")]
	void CreateCountryInfoJsonTemplate()
	{
		Load();

		AllCountryInfo allCountriesInfo = new AllCountryInfo();
		allCountriesInfo.countryInfo = new CountryInfo[countries.Length];
		for (int i = 0; i < countries.Length; i++)
		{
			CountryInfo info = new CountryInfo() { countryName = countries[i].name, countryCode = countries[i].alpha3Code };
			allCountriesInfo.countryInfo[i] = info;
		}

		string jsonText = JsonUtility.ToJson(allCountriesInfo, prettyPrint: true);

		System.IO.StreamWriter writer = new System.IO.StreamWriter("./Assets/CountryInfoTemplate.json");
		writer.Write(jsonText);
		Debug.Log(jsonText);
		writer.Dispose();
		Debug.Log("Template file saved to Assets folder");
	}
	*/
}
