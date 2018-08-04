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
    public class CCNode{
      public int Id;
      public Rect Window = Rect.zero;

      public string Content;

      public List<int> Links = new List<int>();

      public Vector2 GetCenter => Window.center;
      public Vector2 GetDrawLineEnd(Vector2 Direction) => Window.center + (Vector2)Vector3.Cross(Direction, Vector3.forward) * 5;
      public Vector2 GetDrawLineArrowEnd (Vector2 Direction, Vector2 LineCenter, Vector3 CrossVector) => LineCenter - (5 * Direction) + (Vector2)Vector3.Cross(Direction, CrossVector) * 5;

      public CCNode(CCBuilderScriptableObject reference, int ID){
        Id = ID;

        reference.NodeList.Add(this);
        reference.NodeDictionary.Add(ID, this);
      }

      public void AddLink(int LinkID){
        Links.Add(LinkID);
      }

      public void RemoveLink(int LinkID) {
        Links.Remove(LinkID);
      }
    }

    public Dictionary<int, CCNode> NodeDictionary = new Dictionary<int, CCNode>();
    public List<CCNode> NodeList = new List<CCNode>();
    [SerializeField] private List<int> IdDisposeStack = new List<int>();

    public void OnEnable() {
      NodeDictionary = new Dictionary<int, CCNode>(NodeList.Count);
      foreach (var item in NodeList) NodeDictionary.Add(item.Id, item);
    }

    public void AddNode(){
      var ID = GetID();
      var temp = new CCNode(this, ID);
    }

    public void DeleteNode(CCNode item){
      // TODO: Delete links from item

      IdDisposeStack.Add(item.Id);
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
  }
}