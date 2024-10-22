using UISystem;

namespace BaseSystem.UI
{
    /// <summary>
    /// 試験のUIを管理する
    /// </summary>
    public class UIInExamView : UIView
    {
        /// <summary>
        /// 試験を終えるタイミングで呼び出す
        /// </summary>
        public void ExitExam()
        {
            SceneLoader.LoadScene("moch_AfterExam");
        }
    }
}