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
        SecondExam,
        ExamClear,
        AllExams,
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