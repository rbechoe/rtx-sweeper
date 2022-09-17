using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateDatabase : Editor
{
    [MenuItem("Window/Geoffrey UPD/Generate Localisation Scriptable")]
    private static void Init()
    {
        string readPath = Directory.GetCurrentDirectory() + "/Assets/Resources/Localisation.tsv";
        string writePath = Directory.GetCurrentDirectory() + "/Assets/Scripts/TranslationTool/LanguageEnum.cs";

        LanguageDatabase database = ScriptableObject.CreateInstance<LanguageDatabase>();
        string currentReadingLine;
        List<Translate> SentenceTranslation = new List<Translate>();

        using (StreamReader sr = new StreamReader(readPath))
        {
            bool inLanguageRow = true;
            while((currentReadingLine = sr.ReadLine()) != null)
            {
                string[] lineElement = currentReadingLine.Split('\t');

                if (inLanguageRow)
                {
                    using (StreamWriter sw = new StreamWriter(writePath))
                    {
                        sw.WriteLine("public enum Language \n{");
                        // skip first cell
                        for (int i = 1; i < lineElement.Length; i++)
                        {
                            string newString = "";
                            string temp = lineElement[i];
                            for (int j = 0; j < lineElement[i].Length; j++)
                            {
                                if (temp[j] == '-')
                                {
                                    continue;
                                }
                                newString += temp[j];
                            }

                            if (i != lineElement.Length - 1)
                            {
                                sw.WriteLine("\t" + newString + $"= {i - 1},");
                            }
                            else
                            {
                                // last entry doesnt need comma
                                sw.WriteLine("\t" + newString + $"= {i - 1}");
                            }
                        }
                        sw.WriteLine("}");
                        sw.Flush();
                        inLanguageRow = false;
                    }
                }
                else
                {
                    List<string> translation = new List<string>();
                    for (int i = 1; i < lineElement.Length; i++)
                    {
                        translation.Add(lineElement[i]);
                        //Debug.Log(lineElement[i]);
                    }
                    Debug.Log(lineElement[0] + " " + translation[0]);
                    Translate t = new Translate(lineElement[0], translation);
                    Debug.Log(t.translation[0]);
                    SentenceTranslation.Add(new Translate(lineElement[0], translation));
                }
            }
            Debug.Log(SentenceTranslation.Count);

            database.translations = new List<Translate>();
            database.translations = SentenceTranslation;

            AssetDatabase.CreateAsset(database, "Assets/Resources/Languages.asset");
            AssetDatabase.Refresh();

            sr.Dispose();
            sr.Close();
        }
    }
}
