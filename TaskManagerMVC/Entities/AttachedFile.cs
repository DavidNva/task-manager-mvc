using Microsoft.EntityFrameworkCore;

namespace TaskManagerMVC.Entities
{
    public class AttachedFile
    {
        public Guid Id { get; set; } //Usamos Guid para que sea unico y no se repita.
        public int TaskItemId { get; set; } //Clave foranea a TaskItem
        public TaskItem TaskItem { get; set; } //Propiedad de navegacion a TaskItem
        [Unicode(false)]//con esto en lugar de nvarchar sera varchar en la base de datos.
        public string Url { get; set; } //Url del archivo adjunto
        public string Title { get; set; } //Titulo del archivo adjunto
        public int Order { get; set; } //Orden del archivo adjunto
        public DateTime CreatedAt { get; set; } //Fecha de creacion del archivo adjunto
        
        //La diferencia entre nvarchar y varchar es que nvarchar soporta caracteres Unicode (como acentos, caracteres especiales, etc.) mientras que varchar no. En SQL Server, nvarchar utiliza 2 bytes por caracter, mientras que varchar utiliza 1 byte por caracter.
        //Regularmente se usa nvarchar para textos que pueden contener caracteres especiales, y varchar para textos que solo contienen caracteres ASCII.
        //Se recomienda usar nvarchar para la mayoría de los casos, a menos que se tenga una razón específica para usar varchar (como optimización de espacio en la base de datos). Como por ejemplo , para almacenar nombres de usuarios, correos electrónicos, URLs, etc., donde generalmente no se esperan caracteres especiales, emojis, etc.

    }
}
