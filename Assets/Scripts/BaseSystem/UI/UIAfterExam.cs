using UISystem;

namespace BaseSystem.UI
{
    /// <summary>
    /// 試験の後のシーンのUIを管理する
    /// </summary>
    public class UIAfterExam : UIView
    {
        public void GoToTitle()
        {
            SceneLoader.LoadScene("moch_Title");
        }
    }
}