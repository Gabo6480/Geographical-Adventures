using UnityEngine;
[System.Serializable]
public class State
{
    public string name;
    public string? name_alt; // alternate
    public string name_sort; // alternate 2 (not sure what the _sort attribute means?)

    public string nameOfficial;

    public string type;

    public string abbreviation;

    public string countryName;
    public string countryAlpha3Code;

    public int population;

    public Shape shape;
}
