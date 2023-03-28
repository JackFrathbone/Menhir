using UnityEngine;

public class UIForwarder : MonoBehaviour
{

    public void ForwardExit()
    {
        SceneLoader.instance.QuitGame();
    }

    public void ForwardLoadMenuScene(int i)
    {
        SceneLoader.instance.LoadMenuScene(i);
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
