using System;
using System.Reflection;
using AlmanacClasses.Classes;

namespace AlmanacClasses.API;

public static class ClassesAPI
{
    private static readonly MethodInfo? API_AddExperience;

    public static void AddEXP(int amount)
    {
        API_AddExperience?.Invoke(null, new object[] { amount });
    }


    static ClassesAPI()
    {
        if (Type.GetType("AlmanacClasses.API.API, AlmanacClasses") is not { } api)
        {
            return;
        }
        
        API_AddExperience = api.GetMethod("AddExperience", BindingFlags.Public | BindingFlags.Static);
    }
}

public static class API
{
    public static void AddExperience(int amount) => PlayerManager.AddExperience(amount);
}