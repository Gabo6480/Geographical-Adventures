using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor.PackageManager;
using UnityEngine;

public class StateReader
{
	JsonTextReader reader;

	public State[] ReadStates(TextAsset stateFile)
	{

		reader = new JsonTextReader(new System.IO.StringReader(stateFile.text));

		List<State> stateList = new List<State>();

		ReadAhead(5);

		while (reader.Read())
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
                stateList.Add(ReadState());
			}
			else
			{
				break;
			}

		}

		reader.Close();

		return stateList.ToArray();
	}

	State ReadState()
	{
        State state = new State();

		int startDepth = reader.Depth;

		// Read country name and code from properties array
		while (reader.TokenType != JsonToken.EndArray)
		{
			reader.Read();
			if (reader.TokenType == JsonToken.PropertyName)
			{
				switch ((string)reader.Value)
				{
					case "admin":
                        state.nameOfficial = reader.ReadAsString();
						break;
					case "name":
                        state.name = reader.ReadAsString();
						break;
					case "type_en":
                        state.type = reader.ReadAsString();
						break;
					case "name_alt":
                        state.name_alt = reader.ReadAsString();
						break;
					case "adm0_a3":
						state.countryAlpha3Code = reader.ReadAsString();
						break;
                    case "geonunit":
                        state.countryName = reader.ReadAsString();
                        break;
                    case "abbrev":
                        state.abbreviation = reader.ReadAsString();
						break;
				}
			}
		}

		List<Polygon> polygons = new List<Polygon>();
		List<Path> pathsInCurrentPolygon = new List<Path>();
		List<Coordinate> coordList = new List<Coordinate>();

		// Read shape data from geometry array
		while (reader.Read() && reader.Depth > startDepth)
		{
			if (reader.TokenType == JsonToken.Float)
			{
				double x = (double)reader.Value;
				double y = (double)reader.ReadAsDouble();
				Coordinate coord = new Coordinate((float)x * Mathf.Deg2Rad, (float)y * Mathf.Deg2Rad);
				coordList.Add(coord);
				ReadAhead(2);
			}

			if (reader.TokenType == JsonToken.EndArray)
			{
				//	Debug.Log("Finished path " + pointsList[0] + "  -> " + pointsList[pointsList.Count - 1]);
				coordList.Add(coordList[0]); // duplicate start point at end for conveniece in some other code
				Path path = new Path() { points = coordList.ToArray() };
				pathsInCurrentPolygon.Add(path);

				coordList.Clear();
				ReadAhead(1);
			}
			if (reader.TokenType == JsonToken.EndArray)
			{
				//Debug.Log("Finished polygon (" + pathsInCurrentPolygon.Count + " paths)");
				Polygon polygon = new Polygon() { paths = pathsInCurrentPolygon.ToArray() };
				polygons.Add(polygon);

				pathsInCurrentPolygon.Clear();
				ReadAhead(1);
			}
		}

        state.shape = new Shape() { polygons = polygons.ToArray() };

		return state;
	}

	void ReadAhead(int n)
	{
		for (int i = 0; i < n; i++)
		{
			reader.Read();
		}
	}

}
