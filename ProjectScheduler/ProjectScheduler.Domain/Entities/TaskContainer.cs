namespace Domain.Entities
{
    public class TaskContainer
    {
        public Guid Id { get; set; }
        public ContainerType containerType { get; set; }
        public string? Description { get; set; }
        public ICollection<TaskContainer>? SubTaskContainers { get; set; }
        public TaskContainer? Parent { get; set; }
    }

    public enum ContainerType
    {
        COMPANY,
        DEPARTMENT,
        PROJECT,
        TASK
    }
}