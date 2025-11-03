function agregarNuevaTareaAlListado() {
    tareaListadoViewModel.tareas.push(new TareaElementoListadoViewModel({ id: 0, titulo: '' }));

    $("[name=titulo-tarea]").last().focus();
}


function manegarFocusOutTituloTarea(tarea) {
    const titulo = tarea.titulo();

    if (!titulo) {
        tareaListadoViewModel.tareas.pop();//Eliminamos el elementos del final
        return;
    }

    tarea.id(1);
}