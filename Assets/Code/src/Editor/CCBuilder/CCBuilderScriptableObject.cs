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
      public int Id;
      public Rect Window = Rect.zero;

      public List<int> Links = new List<int>();
      public List<int> LinkTargets = new List<int>();

      public List<Rect> LinkButtons = new List<Rect>();
      public Vector2 GetLinkCenter(int index) => LinkButtons[index].center + Window.position;
      public Vector2 GetLinkFreeCenter => GetLinkCenter(LinkButtons.Count - 1);

      public CCGeneral(CCBuilderScriptableObject reference, int ID){
        Id = ID;
        LinkButtons.Add(Rect.zero);

        reference.CCDictionary.Add(ID, this);
      }

      public int AddLink(int ID, int TargetID){
        Links.Add(ID);
        LinkTargets.Add(TargetID);

        LinkButtons.Add(Rect.zero);
        return LinkButtons.Count - 2;
      }

      public void SetLinkTarget(int TargetID){
        LinkTargets[LinkTargets.Count - 1] = TargetID;
      }

      public void RemoveLink(int ButtonID, out int itemID, out int linkTargetID){
        itemID = Links[ButtonID];
        linkTargetID = LinkTargets[ButtonID];

        Links.RemoveAt(ButtonID);
        LinkTargets.RemoveAt(ButtonID);
        LinkButtons.RemoveAt(ButtonID);
      }
    }

    [System.Serializable]
    public class CCCharacterStates: CCGeneral{
      public string CharacterStates;

      public CCCharacterStates(CCBuilderScriptableObject reference, int ID) : base(reference, ID){
        reference.CCCharacterStateList.Add(this);
      }
    }

    [System.Serializable]
    public class CCFunction: CCGeneral{
      public bool IsCharacterStateMulti;
      public string CharacterState;
      public string Function;

      public CCFunction(CCBuilderScriptableObject reference, int ID) : base(reference, ID) {
        reference.CCFuncitonList.Add(this);
      }
    }

    public Dictionary<int, CCGeneral> CCDictionary = new Dictionary<int, CCGeneral>();
    public List<CCCharacterStates> CCCharacterStateList = new List<CCCharacterStates>();
    public List<CCFunction> CCFuncitonList = new List<CCFunction>();
    [SerializeField] private List<int> IdDisposeStack = new List<int>();

    public void OnEnable() {
      CCDictionary = new Dictionary<int, CCGeneral>(CCCharacterStateList.Count + CCFuncitonList.Count);
      foreach (var item in CCCharacterStateList) CCDictionary.Add(item.Id, item);
      foreach (var item in CCFuncitonList) CCDictionary.Add(item.Id, item);
    }

    public int AddCharacterState(){
      var ID = GetID();
      var temp = new CCCharacterStates(this, ID);
      return ID;
    }

    public int AddFunction() {
      var ID = GetID();
      var temp = new CCFunction(this, ID);
      return ID;
    }

    public static bool isDifferentNodes(CCGeneral item1, CCGeneral item2){
      var conItem1 = item1 is CCCharacterStates ? item1 as CCCharacterStates : item2 as CCCharacterStates;
      var conItem2 = item1 is CCFunction ? item1 as CCFunction : item2 as CCFunction;
      return conItem1 != null && conItem2 != null;
    }

    private int GetID(){
      var ID = -1;
      if (IdDisposeStack.Count > 0)
        ID = IdDisposeStack[IdDisposeStack.Count - 1];
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