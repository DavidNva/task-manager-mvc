using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerMVC.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }//En Entity Framework, una propiedad llamada Id o <ClassName>Id se considera automáticamente la clave primaria

        [Required]//Esto indica que la propiedad es obligatoria (no puede ser nula), equivalente a NOT NULL en la base de datos
        [StringLength(260)]//Comentamos esto para probar ahora usando Fluent API en ApplicationDBContext.cs
        public string Title { get; set; }
        //[Required]//Esto indica que la propiedad es obligatoria (no puede ser nula), equivalente a NOT NULL en la base de datos
        [StringLength(1000)]
        public string? Description { get; set; }

        public int Order { get; set; }
        //public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        //public DateTime? DueDate { get; set; }
        public string UserCreatorId { get; set; }//Clave foranea a IdentityUser
        //La relacion con IdentityUser es de muchos a uno, es decir, muchas tareas pueden ser creadas por un solo usuario. Ahora como sabe Entity Framework que esta propiedad es una clave foranea a IdentityUser? Lo sabe porque sigue la convención de nombrado <NavigationPropertyName>Id, donde NavigationPropertyName es el nombre de la propiedad de navegación que apunta a la entidad relacionada. Y como relaciona a usuario y no otra entidad? Porque la propiedad de navegacion es de tipo IdentityUser. Es decir toma el tipo de la propiedad de navegacion siguiente en el codigo.
        //En este caso por ejemplo, un caso hipotetico seria que si tuvieramos otra entidad llamada AdminUser y quisieramos relacionar TaskItem con AdminUser, entonces tendriamos que tener una propiedad de navegacion llamada AdminUser y la clave foranea seria AdminUserId. pero por ejemplo, si tuvieramos un UserCreatorId y despues colocamos una propiedad de navegacion llamada OtrasTareas, entonces Entity Framework no sabria a que entidad relacionar la clave foranea UserCreatorId. Lo generaria pero una clase o relacion incorrecta.
        public IdentityUser UserCreator { get; set; }//Propiedad de navegacion a IdentityUser. Ahora, porque es necesario colocar esta si ya tenemos la clave foranea UserCreatorId?
                                                     //La razon es que con esta propiedad de navegacion podemos acceder a los datos del usuario creador de la tarea directamente desde la tarea.
                                                     //Es decir, si tenemos una instancia de TaskItem, podemos acceder al usuario creador de la siguiente manera: taskItem.UserCreator
                                                     //Si no tuvieramos esta propiedad de navegacion, tendriamos que hacer una consulta adicional a la tabla de usuarios para obtener los datos del usuario creador.
                                                     //Entonces, en resumen, la propiedad de navegacion nos facilita el acceso a los datos relacionados y nos evita tener que hacer consultas adicionales.
        public List<Step> Steps { get; set; }//Con esto si cargo una tarea, tambien cargo sus pasos asociados.
                                             //public List<Step> Steps { get; set; } = new List<Step>();
                                             //La relacion con Steps es de uno a muchos, es decir una tarea puede tener muchos pasos asociados y cada paso pertenece a una sola tarea.
        public List<AttachedFile> AttachedFiles { get; set; }
        //La relacion con AttachedFiles es de uno a muchos, es decir una tarea puede tener muchos archivos adjuntos asociados y cada archivo adjunto pertenece a una sola tarea.


        //Ahora bien, si queremos que algun campo sea nullable, por ejemplo Description, entonces podemos hacer lo siguiente: 
        //public string? Description { get; set; } //El ? indica que puede ser nulo 
        //O si queremos que sea nullable un DateTime, entonces hacemos lo siguiente:
        //public DateTime? DueDate { get; set; } //El ? indica que puede ser nulo
        //public bool IsCompleted { get; set; } = false; //Por defecto es false. Seria el equivalente a poner un valor por defecto en la base de datos.
        //De igual forma, podemos poner en configuraciones de visual studio en las propiedades del proyecto, que :<PropertyGroup>
        //  <TargetFramework>net9.0</TargetFramework>
        //  <Nullable>enable</Nullable>
        //  <ImplicitUsings>enable</ImplicitUsings>
        //</PropertyGroup> 
        //Con esto, todas las referencias de tipos en el proyecto serán tratadas como no anulables (non-nullable) de forma predeterminada.
        //Entonces si queremos que alguna propiedad sea nullable, tenemos que poner el ? manualmente.
        //Pero si no queremos hacer esto, podemos poner <Nullable>disable</Nullable> y asi todas las propiedades seran nullable por defecto.



        //Otras formas de configurar la clave primaria y otras propiedades:
        //Con Data Annotations y Fluent API en ApplicationDBContext.cs
        //Las anotaciones de datos o Data Annotations son conjunto de atributos que podemos utilizar para realizar configuraciones en las clases de nuestro modelo., por ejemplo [Key], [Required], [MaxLength], [ForeignKey], etc.
        //Ejemplo supongamos que tenemos la propiead de Title y quiero que sea nvarchar(250) y que sea requerida, entonces puedo usar las Data Annotations de la siguiente manera:
        // [Required]
        // [MaxLength(250)]
        //O [StringLength(250)]
        // public string Title { get; set; }


        //Ahora si quiero hacer lo mismo pero con Fluent API en ApplicationDBContext.cs, entonces en el método OnModelCreating(ModelBuilder modelBuilder) puedo hacer lo siguiente:
        // modelBuilder.Entity<TaskItem>(entity =>
        // {
        //     entity.HasKey(e => e.Id); // Configura la clave primaria
        //     entity.Property(e => e.Title)
        //           .IsRequired() // Indica que es obligatorio (NOT NULL)
        //           .HasMaxLength(250); // Configura la longitud máxima
        // });

        //Ahora, cual de los dos se recomienda? 
        //Depende, si es algo sencillo y rápido, las Data Annotations son más fáciles de usar y entender. Pero si es algo más complejo o si quieres tener toda la configuración en un solo lugar, Fluent API es más poderoso y flexible.
        //Unas de las ventajas de DATA ANNOTATIONS es que son más fáciles de leer y entender, ya que están directamente en la clase del modelo. Pero una desventaja es que pueden hacer que la clase del modelo se vea desordenada si tienes muchas anotaciones.
        //Ademas con las data annotations podemos igual usar el validar el modelo en el controlador usando ModelState.IsValid es una gran ventaja.
        //Por otro lado, Fluent API es más poderoso y flexible, ya que te permite hacer configuraciones más complejas que no son posibles con las Data Annotations. Pero una desventaja es que puede ser más difícil de leer y entender, ya que la configuración está separada de la clase del modelo. Que seria como su tuvieramos hecho la base de datos a mano con SQL.
    }
}
