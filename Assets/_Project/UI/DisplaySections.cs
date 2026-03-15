using System.Collections.Generic;

[System.Serializable]
public class DisplaySection
{
    public string title;
    public List<Content> contents;
    public int size;
    public Coord location;
}

[System.Serializable]
public class Content
{
    public string type;
    public string value;
}


[System.Serializable]
public class DisplaySectionContainer
{
    public DisplaySection[] sections;
}

[System.Serializable]
public struct Coord
{
    public float x;
    public float y;
}