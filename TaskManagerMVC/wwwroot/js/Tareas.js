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
        manejarErrorApi(respuesta);
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
        manejarErrorApi(respuesta);
        return;
    }
    const json = await respuesta.json();
    tareaListadoViewModel.tareas([]);//Limpiamos el listado])

    json.forEach(valor => {
        tareaListadoViewModel.tareas.push(new TareaElementoListadoViewModel(valor));
    });

    tareaListadoViewModel.cargando(false);
}

async function actualizarOrdenTareas() {
    const ids = obtenerIdsTareas();
    await enviarIdsTareasAlBackend(ids);

    const arregloOrdenado = tareaListadoViewModel.tareas.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });

    tareaListadoViewModel.tareas([])
    tareaListadoViewModel.tareas(arregloOrdenado);
}

function obtenerIdsTareas() {
    const ids = $("[name=title-tarea]").map(function () {
        return $(this).attr("data-id");
    }).get();

    return ids;
}


async function enviarIdsTareasAlBackend(ids) {
    let data = JSON.stringify(ids);
    await fetch(`${urlTareas}/ordenar`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTareas();
        }
    })

});

    
