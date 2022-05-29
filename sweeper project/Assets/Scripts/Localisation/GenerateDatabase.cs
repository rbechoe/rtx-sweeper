using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GenerateDatabase : Editor
{
    static string readPath = Directory.GetCurrentDirectory() + "/Assets/resources/Localisation.tsv";
    static string writePath = Directory.GetCurrentDirectory() + "/Assets/Scripts/Localisation/LanguageEnum.cs";

    [MenuItem("Window/Enchanted Works/Generate Localisation Scriptable")]
    private static void Init()
    {
        LanguageDatabase database = ScriptableObject.CreateInstance<LanguageDatabase>();
        string currentReadingLine;
        List<Translate> SentenceTranslation = new List<Translate>();

        using (StreamReader sr = new StreamReader(readPath))
        {
            bool languageRow = true;
            while((currentReadingLine = sr.ReadLine()) != null)
            {
                string[] lineElement = currentReadingLine.Split('\t');

                if (languageRow)
                {
                    using (StreamWriter sw = new StreamWriter(writePath))
                    {
                        sw.WriteLine("public enum Language \n{");

                        for (int i = 1; i < lineElement.Length; i++)
                        {
                            if (i != lineElement.Length - 1)
                            {
                                sw.WriteLine("\t" + lineElement[i] + $"= {i - 1},");
                            }
                            else
                            {
                                sw.WriteLine("\t" + lineElement[i] + $"= {i - 1}");
                            }
                        }
                        sw.WriteLine("}");
                        sw.Flush();
                        languageRow = false;
                    }
                }
                else
                {
                    List<string> translation = new List<string>();
                    for (int i = 1; i < lineElement.Length; i++)
                    {
                        translation.Add(lineElement[i]);
                    }
                    SentenceTranslation.Add(new Translate(lineElement[0], translation));
                }
            }

            database.translations = new List<Translate>();
            database.translations = SentenceTranslation;

            AssetDatabase.CreateAsset(database, "Assets/Scripts/Localisation/Languages.asset");
            AssetDatabase.Refresh();

            sr.Dispose();
            sr.Close();
        }
    }
}
