using UnityEngine;
using UnityEngine.AddressableAssets;

public static class NameGenerator
{
    private static string[] MaleFirstnames;
    private static string[] FemaleFirstnames;
    private static string[] Surnames;
    private static string[] EmailDomains = new[] { "hotmale", "femail", "t-offline", "inlook", "woohoo", "yourmail", "whatever" };
    private static string[] EmailSuffixes = new[] { "com", "org", "net", "io", "de", "ai", "au", "xyz" };

    public static void Init()
    {
        //Addressables.LoadAssetAsync<TextAsset>("male_firstnames").Completed += (op) => { MaleFirstnames = op.Result.text.Replace("\"", "").Split(','); };
        //Addressables.LoadAssetAsync<TextAsset>("female_firstnames").Completed += (op) => { FemaleFirstnames = op.Result.text.Replace("\"", "").Split(','); };
        //Addressables.LoadAssetAsync<TextAsset>("surnames").Completed += (op) => { Surnames = op.Result.text.Replace("\"", "").Split(','); };
        MaleFirstnames = Addressables.LoadAssetAsync<TextAsset>("male_firstnames").WaitForCompletion().text.Replace("\"", "").Split(',');
        FemaleFirstnames = Addressables.LoadAssetAsync<TextAsset>("female_firstnames").WaitForCompletion().text.Replace("\"", "").Split(',');
        Surnames = Addressables.LoadAssetAsync<TextAsset>("surnames").WaitForCompletion().text.Replace("\"", "").Split(',');
    }

    public static void GenerateName(out string firstname, out string surname, out string email)
    {
        firstname = $"{(Random.Range(0, 2) == 0 ? MaleFirstnames.GetRandomElement() : FemaleFirstnames.GetRandomElement())}";
        surname = Surnames.GetRandomElement();
        int rand = Random.Range(0, 3);
        string first = rand == 0 ? firstname : rand == 1 ? firstname.Substring(0, 1) : firstname;
        string last = rand == 1 ? surname : rand == 0 ? surname.Substring(0, 1) : surname;
        string emailDomain = EmailDomains.GetRandomElement();
        string emailSuffix = EmailSuffixes.GetRandomElement();
        rand = Random.Range(0, 3);
        string separator = rand == 0 ? "" : rand == 1 ? "_" : ".";
        email = $"{first}{separator}{last}@{emailDomain}.{emailSuffix}".ToLower();
    }
}
