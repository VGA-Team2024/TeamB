using UISystem;

namespace BaseSystem.UI
{
    /// <summary>
    /// 授業中のUIを管理する
    /// </summary>
    public class UIInClassView : UIView
    {
        public void GoToExamination()
        {
            SceneLoader.LoadScene("moch_Exam");
        }
    }
}