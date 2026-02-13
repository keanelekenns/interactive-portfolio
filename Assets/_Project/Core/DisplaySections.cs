using System.Collections.Generic;

[System.Serializable]
public class DisplaySection
{
    public string title;
    public List<Content> contents;
    public int size;
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