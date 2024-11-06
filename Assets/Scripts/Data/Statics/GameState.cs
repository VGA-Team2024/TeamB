namespace TeamB.Data
{
    /// <summary>
    /// ゲームのステート
    /// </summary>
    public enum GameState : int
    {
        None,
        Title,
        Class,
        Exam,
        CharmUp,
        SuddenlyEvent,
    }

    public enum ExamState : int
    {
        FirstExam,
        SecondExam,
    }

    public enum LanguageType : int
    {
        Japanese,
        English,
    }
}