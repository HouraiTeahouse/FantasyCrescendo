using UnityEngine;
using UnityEngine.UI;

public class StageSlotUI : MonoBehaviour {

    /// <summary>
    /// The stage logo. This field is usually filled by other script
    /// except in the case of the Random buttom.
    /// </summary>
    public Image stageLogo;

    public string stageName = "stage";

    /// <summary>
    /// The stage name text in the canvas. This field is usually filled by other script
    /// except in the case of the Random buttom.
    /// </summary>
    public Text stageNameText;

    // Use this for initialization
    private void Start() {}

    public void setStageLogoElement(Image i) {
        stageLogo = i;
    }

    public void setStageTextElement(Text t) {
        stageNameText = t;
    }

    public void setStageName(string s) {
        stageName = s;
    }

    public void hoveringStage(bool b) {
        if (b)
            stageNameText.text = stageName;
        else
            stageNameText.text = "";
    }

}