using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/** insert spaces before capital letters 
 *  remove optional m_, or k followed by uppercase letter in front of the name
 *  replace _ with spaces
 */
public class RenameObjects_Split : ScriptableWizard
{
    [MenuItem("Vienna/Batch Rename...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Batch Split Rename", typeof(RenameObjects_Split), "Rename");
    }

    void UpdateSelectionHelper()
    {

        helpString = "";

        if (Selection.objects != null)
            helpString = "Number of objects selected: " + Selection.objects.Length;
    }
    void OnEnable()
    {
        UpdateSelectionHelper();
    }
    void OnSelectionChange()
    {
        UpdateSelectionHelper();
    }

    void OnWizardCreate()
    {

        // If selection is empty, then exit
        if (Selection.objects == null)
            return;

        // Current Increment
       
        List<GameObject> mySelection = new List<GameObject>(Selection.gameObjects);
        mySelection.Sort((go1, go2) => go1.transform.GetSiblingIndex().CompareTo(go2.transform.GetSiblingIndex()));

        foreach (var go in mySelection)
        {
            go.name = SplitToWords(go.name);
        }
    }

    private string SplitToWords(string _n)
    {
        //This function will insert spaces before capital letters and remove optional m_, 
        //_ or k followed by uppercase letter in front of the name.

        return ObjectNames.NicifyVariableName(_n).Replace("_", " "); ;
    } 
}