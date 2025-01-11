
public class BuildState
{
    const string _hash = "06fe52d5-69d4-4a71-908e-4ff479dd95f9";
    public const string TeamID = "TeamB2024";

    public static string BuildHash
    {
        get
        {
#if UNITY_EDITOR
            return "UNITY_EDITOR";
#else
            return _hash;
#endif
        }
    }
};