using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using System.IO;

namespace HouraiTeahouse {
  public class CCBuilderScriptableObject : ScriptableObject {
    public int IdCounter = 0;

    [System.Serializable]
    public class CCGeneral{
      public string Name = "Default Name";
      public int Id;
      public Rect Window = Rect.zero;

      public Rect LinkRect;
      public Vector2 GetLinkCenter => LinkRect.center + Window.position;
    }

    [System.Serializable]
    public class CCCharacterStates: CCGeneral{
      public string CharacterStates;
      public List<int> Links = new List<int>();
    }

    [System.Serializable]
    public class CCFunction: CCGeneral{
      public bool IsCharacterStateMulti;
      public string CharacterState;
      public string Function;
    }

    public Dictionary<int, CCGeneral> CCDictionary = new Dictionary<int, CCGeneral>();
    [SerializeField] private List<CCCharacterStates> CCCharacterStateList = new List<CCCharacterStates>();
    [SerializeField] private List<CCFunction> CCFuncitonList = new List<CCFunction>();
    [SerializeField] private Stack<int> IdDisposeStack = new Stack<int>();

    public List<CCCharacterStates> GetCharacterStates => CCCharacterStateList;
    public List<CCFunction> GetFunctions => CCFuncitonList;

    public void OnEnable() {
      CCDictionary = new Dictionary<int, CCGeneral>(CCCharacterStateList.Count + CCFuncitonList.Count);
      foreach (var item in CCCharacterStateList) CCDictionary.Add(item.Id, item);
      foreach (var item in CCFuncitonList) CCDictionary.Add(item.Id, item);
    }

    public int AddCharacterState(){
      var ID = GetID();

      var temp = new CCCharacterStates();
      temp.Id = ID;
      CCCharacterStateList.Add(temp);
      CCDictionary.Add(ID, temp);
      return ID;
    }

    public int AddFunction() {
      var ID = GetID();

      var temp = new CCFunction();
      temp.Id = ID;
      CCFuncitonList.Add(temp);
      CCDictionary.Add(ID, temp);
      return ID;
    }

    private int GetID(){
      var ID = -1;
      if (IdDisposeStack.Count > 0)
        ID = IdDisposeStack.Pop();
      else
        ID = IdCounter++;
      return ID;
    }

    public static CCBuilderScriptableObject GetCCBuilder(){
      var myInstance = (CCBuilderScriptableObject)Resources.Load("CCBuilder/CCBuilder") as CCBuilderScriptableObject;
      if (myInstance == null) {
        myInstance = CreateInstance<CCBuilderScriptableObject>();
        AssetDatabase.CreateAsset(myInstance, "Assets/Resources/CCBuilder/CCBuilder.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
      }
      return myInstance;
    }

    public void UpdateFile(){
      var fileContents = new List<string>() {
        "using HouraiTeahouse.FantasyCrescendo.Players;",
        "using System;",
        "using System.Collections.Generic;",
        "using System.Linq;",
        "using UnityEngine;",
        "namespace HouraiTeahouse.FantasyCrescendo.Characters {",
        "",
        "public partial class CharacterControllerBuilder {",
        "  public StateController<CharacterState, CharacterContext> BuildCharacterControllerImpl(StateControllerBuilder<CharacterState, CharacterContext> builder) {",
        "    Builder = builder;",
        "    InjectState(this);",
        ""
        };

      foreach(var item in CCCharacterStateList.Where(i => i.Links.Count > 0)){
        var singleItem = false;
        if (item.CharacterStates.Contains(',')){
          fileContents.Add(string.Format("    new[] {{{0}}}", item.CharacterStates.Trim()));
        } else {
          singleItem = true;
          fileContents.Add(string.Format("    {0}", item.CharacterStates.Trim()));
        }
        foreach (var linkItem in item.Links.Select(i => CCDictionary[i] as CCFunction)){
          if (linkItem.IsCharacterStateMulti){
            fileContents.Add(string.Format("      .AddTransition{0}<CharacterState, CharacterContext>(", 
                              singleItem ? "" : "s"));
            fileContents.Add(string.Format("        {0})", linkItem.Function));
          } else {
            fileContents.Add(string.Format("      .AddTransition{0}({1}, {2})",
                              singleItem ? "" : "s", linkItem.CharacterState.Trim(), linkItem.Function));
          }
        }
        fileContents[fileContents.Count - 1] += ";";
      }

      fileContents.AddRange(new List<string>() {
        "",
        "    Builder.WithDefaultState(Idle);",
        "    BuildCharacterController();",
        "    return Builder.Build();",
        "  }",
        "}",
        "}"
        });
      File.WriteAllLines(Application.dataPath + @"/Code/src/Runtime/Character/States/CharacterControllerBuilder.Template.cs", fileContents);
    }
  }
}