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
        None,
        FirstExam,
        FirstExamSuccess,
        FirstExamFail,
        SecondExam,
        SecondExamSuccess,
        SecondExamFail,
        ExamClear,
    }

    public enum ExamResult : int
    {
        None,
        Clear,
        Failed,
    }

    public enum LanguageType : int
    {
        None,
        Japanese,
        English,
    }
}