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

    public enum BattleState : int
    {
        FirstExam,
        SecondExam,
    }
}