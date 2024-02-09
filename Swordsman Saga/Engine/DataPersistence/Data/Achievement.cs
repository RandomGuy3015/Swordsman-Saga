namespace Swordsman_Saga.Engine.DataPersistence.Data;

public class Achievement
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsCompleted { get; set; }
    public float Progress { get; set; }

    public Achievement(int id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
        IsCompleted = false;
        Progress = 0.0f;
    }
}