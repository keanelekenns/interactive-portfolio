[System.Serializable]
public class DisplaySection
{
    public string category;
    public string text;
    public int size;
}


[System.Serializable]
public class DisplaySectionContainer
{
    public DisplaySection[] sections;
}