using Microsoft.EntityFrameworkCore;
using TaskManagerMVC.Entities;
namespace TaskManagerMVC
{
    //El nombre ApplicationDBContext puede ser cualquiera, es solo una convención. En si se hereda de DbContext
    public class ApplicationDBContext : DbContext
    {
        //DbContext es la clase base, la pieza central para trabajar con Entity Framework Core 
        //y representa una sesión con la base de datos, permitiendo realizar consultas y guardar cambios.   
        //podemos configurar la conexión a la base de datos y definir los conjuntos de entidades (tablas) que queremos mapear.
        //podemos hacer select, insert, update y delete de datos en la base de datos utilizando el contexto.

        //DbContextOptions es una clase que contiene las opciones de configuración para DbContext.
        //Estas opciones pueden incluir detalles como la cadena de conexión a la base de datos,
        //el proveedor de base de datos (por ejemplo, SQL Server, SQLite, etc.), y otras configuraciones específicas del contexto.
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<TaskItem> Tasks { get; set; }//Ya con esto , ya tenemos nuestra tabla Tasks en la base de datos.



        //DbContext es la clase base, la pieza central para trabajar con Entity Framework Core 
        //y representa una sesión con la base de datos, permitiendo realizar consultas y guardar cambios.   
        //podemos configurar la conexión a la base de datos y definir los conjuntos de entidades (tablas) que queremos mapear.
        //podemos hacer select, insert, update y delete de datos en la base de datos utilizando el contexto.
        //podemos definir DbSet<T> para cada entidad que queremos mapear a una tabla en la base de datos.
        //podemos configurar opciones adicionales, como convenciones de nombres, relaciones entre entidades, etc.
        //podemos utilizar migraciones para gestionar cambios en el esquema de la base de datos a lo largo del tiempo.
        //podemos utilizar LINQ para realizar consultas a la base de datos de manera sencilla y expresiva.
        //podemos utilizar el seguimiento de cambios para detectar y guardar automáticamente los cambios realizados en las entidades.
        //podemos utilizar transacciones para asegurar la integridad de los datos durante operaciones complejas.
        //podemos utilizar el patrón Repository para abstraer el acceso a los datos y mejorar la mantenibilidad del código.
        //podemos utilizar el patrón Unit of Work para gestionar múltiples operaciones de base de datos como una sola transacción.


        //Los comandos a usar son : 
        //dotnet ef migrations add InitialCreate
        //dotnet ef database update
        //con la consola del administrador de paquetes de visual studio
        //Add-Migration InitialCreate: Esto lo que hace es crear una nueva migración con el nombre "InitialCreate". en nuestor caso lo hicimos Add-Migration Tasks . Esto genera un archivo de migración que contiene las instrucciones para crear la tabla Tasks en la base de datos. Es decir una carpeta Migrations con archivos .cs
        //Otro comando es Update-Database: Este comando aplica la migración a la base de datos, creando la tabla Tasks según lo definido en la migración generada anteriormente. Y la base de datos se crea si no existe.
        //Estos comandos son parte de Entity Framework Core, una biblioteca ORM (Object-Relational Mapping) que facilita la interacción entre el código C# y la base de datos.
        //Cuando agreguemos mas tablas a nuestro proyecto, tendremos que crear nuevas migraciones y actualizar la base de datos nuevamente. Con el comando Add-Migration NombreDeLaMigracion y luego Update-Database
        //Ejemplo: Si agregamos una nueva entidad llamada User, tendríamos que crear una nueva migración para reflejar ese cambio en la base de datos. Es decir , Add-Migration AddUserTable y luego Update-Database para aplicar la migración.
        //Si queremos ver el estado actual de las migraciones, podemos usar el comando dotnet ef migrations list
        //Si queremos eliminar la última migración que no ha sido aplicada a la base de datos, podemos usar el comando dotnet ef migrations remove
        //Si queremos revertir la base de datos a un estado anterior, podemos usar el comando dotnet ef database update NombreDeLaMigracionAnterior
        //Si queremos ver el SQL que se generará para una migración específica, podemos usar el comando dotnet ef migrations script NombreDeLaMigracionAnterior NombreDeLaMigracionActual

        //Si queremos cambiar configuraciones de una tabla ya sea por ejemplo el nombre de la tabla, el nombre de una columna, nuevos índices, relaciones, campos, etc. Lo haremos en el método OnModelCreating




    }
}
