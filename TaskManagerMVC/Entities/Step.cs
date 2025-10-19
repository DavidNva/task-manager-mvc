namespace TaskManagerMVC.Entities
{
    public class Step
    {
        public Guid Id { get; set; }// Unique identifier for the step, ejemplo: 2023e4567-e89b-12d3-a456-426614174000
        public int TaskItemId { get; set; } // Foreign key to the associated TaskItem. En Entity Framework, una propiedad llamada <ClassName>Id se considera automáticamente una clave foránea si existe una relación con otra entidad.
        public TaskItem TaskItem { get; set; } // Navigation property to the associated TaskItem. Con esto podremos acceder a la tarea asociada desde el paso. es decir como en SQL con el INNER JOIN. 
        public string Description { get; set; } = string.Empty; // Description of the step, ejemplo: "Design the user interface"
        public bool IsCompleted { get; set; } = false; // Indicates if the step is completed, por defecto es false
        public int Order { get; set; } // Order of the step within the task, ejemplo: 1, 2, 3, etc.
    }
}
