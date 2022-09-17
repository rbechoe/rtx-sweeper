using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LocalisationWindow : EditorWindow
{
    private string textInput;
    private List<string> sentencesFix = new List<string>();

    private LanguageDatabase database
    {
        get
        {
            return Resources.Load<LanguageDatabase>("Languages");
        }
    }

    [MenuItem("Window/Enchanted Works/Localisation")]
    static void Init()
    {
        LocalisationWindow window = new LocalisationWindow();
        window.name = "Localisation Edit window";
        window.Show();
    }

    private void OnGUI()
    {
        if (database == null)
        {
            GUILayout.Label("No Database found! Make sure it is in the \"Resources\" folder");
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("ID: ");

        textInput = GUILayout.TextField(textInput, GUILayout.Width(150));
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();

        if (textInput != "")
        {
            sentencesFix = database.GetSentences(textInput);

            if (sentencesFix.Count == 0)
            {
                GUILayout.Label("ID NOT FOUND");
                if (GUILayout.Button("Make new ID"))
                {
                    sentencesFix = new List<string>();
                    int count = Enum.GetNames(typeof(Language)).Length;

                    for (int i = 0; i < count; i++)
                    {
                        sentencesFix.Add("");
                    }

                    database.AddEntry(textInput);
                    for (int i = 0; i < sentencesFix.Count; i++)
                    {
                        Language l = (Language)i;
                        GUILayout.Label("Language = " + l.ToString());
                        sentencesFix[i] = GUILayout.TextField(sentencesFix[i]);
                    }
                }
            }

            if (sentencesFix.Count > 0)
            {
                if (GUILayout.Button("Delete Entry"))
                {
                    database.DeleteEntry(textInput);
                }

                for (int i = 0; i < sentencesFix.Count; i++)
                {
                    Language l = (Language)i;
                    GUILayout.Label("Language = " + l.ToString());
                    sentencesFix[i] = GUILayout.TextField(sentencesFix[i]);
                }
            }
        }

        if (GUILayout.Button("Generate TSV File"))
        {
            GenerateTSV();
        }

        GUILayout.EndVertical();
    }

    public void GenerateTSV()
    {
        using (StreamWriter sr = new StreamWriter(Directory.GetCurrentDirectory() + "Assets/Resources/Localisation.tsv"))
        {
            string languageLine = "\t";
            int languageLength = Enum.GetValues(typeof(Language)).Length;

            for (int i = 0; i < languageLength; i++)
            {
                if (i < (languageLength - 1))
                {
                    languageLine += (Language)i + "\t";
                }
                else
                {
                    languageLine += (Language)i;
                }
            }
            
            sr.WriteLine(languageLine);

            for (int i = 0; i < database.translations.Count; i++)
            {
                string line = database.translations[i].ID + "\t";
                int count = database.translations[i].sentence.Count;

                for (int j = 0; j < count; j++)
                {
                    if (j < count - 1)
                    {
                        line += database.translations[i].sentence[j] + "\t";
                    }
                    else
                    {
                        line += database.translations[i].sentence[j];
                        sr.WriteLine(line);
                    }
                }
            }
        }
    }
}
