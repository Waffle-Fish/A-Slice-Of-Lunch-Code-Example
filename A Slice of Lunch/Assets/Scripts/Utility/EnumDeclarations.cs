using System;

public enum BoxType { Japanese, Italian, French, Chinese }

[Serializable]
public struct CuisineTypes
{
    public enum Japanese { Egg, Katsu, Onigiri, Rice, Sausage, ShrimpTempura, Tamago }
    public enum Italian { Cavtappi, Conchiglie, Farafalle, GreenConchiglie, Macaroni, Penne, Raviloli}
}