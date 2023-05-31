using Udar.SceneField;
using UnityEngine;

public class UIForwarder : MonoBehaviour
{

    public void ForwardExit()
    {
        SceneLoader.instance.QuitGame();
    }

    public void ForwardLoadMenuScene(SceneFieldRef targetScene)
    {
        SceneLoader.instance.LoadMenuScene(targetScene.SceneField.BuildIndex);
    }
    public void ForwardSaveSaveSlot(int i)
    {
        DataManager.instance.SaveSaveSlot(i);
    }

    public void ForwardLoadSaveSlot(int i)
    {
        DataManager.instance.LoadSaveSlot(i);
    }
}
