function agregarNuevaTareaAlListado() {
    tareaListadoViewModel.tareas.push(new TareaElementoListadoViewModel({ id: 0, title: '' }));

    $("[name=title-tarea]").last().focus();
}


async function manegarFocusOutTituloTarea(tarea) {
    const title = tarea.title();

    if (!title) {
        tareaListadoViewModel.tareas.pop();//Eliminamos el elementos del final
        return;
    }
    const data = JSON.stringify(title);
    const respuesta = await fetch(`${urlTareas}/Crear`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        tarea.id(json.id);
    } else {
        //Si hay un error, mostramos un mensaje y eliminamos la tarea del listado
    }

}

async function obtenerTareas() {
    tareaListadoViewModel.cargando(true);
    const respuesta = await fetch(`${urlTareas}/Listar`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    if (!respuesta.ok) {
        return;
    }
    const json = await respuesta.json();
    tareaListadoViewModel.tareas([]);//Limpiamos el listado])
    json.forEach(tarea => {
        tareaListadoViewModel.tareas.push(new TareaElementoListadoViewModel(tarea));
    });

    tareaListadoViewModel.cargando(false);
}