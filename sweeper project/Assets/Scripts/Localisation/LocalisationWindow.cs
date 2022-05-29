using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        window.name = "Localisation Edit Window";
        window.Show();
    }

    private void OnGUI()
    {
        if (database == null)
        {
            GUILayout.Label("No database found, make sure you have the Language file in the Resources folder");
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("ID: ");

        textInput = GUILayout.TextField(textInput, GUILayout.Width(150));
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
    }

    public void GetSentence()
    {
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
    }
}
