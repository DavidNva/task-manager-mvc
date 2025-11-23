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

async function manejarClickTarea(tarea) {
    if (tarea.esNuevo()) {
        return;
    }
    const respuesta = await fetch(`${urlTareas}/ObtenerTareaPorId/${tarea.id()}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }
    const json = await respuesta.json();
    

    tareaEditaVM.id = json.id;
    tareaEditaVM.title(json.title);
    tareaEditaVM.description(json.description);

    modalEditarTareaBootstrap.show();

}

async function manejarCambioEditarTarea() {
    const obj = {
        id: tareaEditaVM.id,
        title: tareaEditaVM.title(),
        description: tareaEditaVM.description()
    }

    if (!obj.title) {
        return;
    }
    await editarTareaCompleta(obj);

    const indice = tareaListadoViewModel.tareas().findIndex(t => t.id() === obj.id);
    const tarea = tareaListadoViewModel.tareas()[indice];
    tarea.title(obj.title);
}

async function editarTareaCompleta(tarea) {
    const data = JSON.stringify(tarea);
    const respuesta = await fetch(`${urlTareas}/EditarTarea/${tarea.id}`, {
        method: "PUT",
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        throw "error";
    }
}

function intentarBorrarTarea(tarea) {
    modalEditarTareaBootstrap.hide();

    confirmarAccion({
        callBackAceptar: () => {
            borrarTarea(tarea);
        },
        callBackCancelar: () => {
            modalEditarTareaBootstrap.show();
        },
        title: `¿Desea borrar la tarea ${tarea.title()}?`
    })
}

async function borrarTarea(tarea) {
    const idTarea = tarea.id;

    const respuesta = await fetch(`${urlTareas}/BorrarTarea/${idTarea}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const indice = obtenerIndiceTareaEnEdicion();
        tareaListadoViewModel.tareas.splice(indice, 1);
    }
}

function obtenerIndiceTareaEnEdicion() {
    return tareaListadoViewModel.tareas().findIndex(t => t.id() == tareaEditaVM.id);
}

$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTareas();
        }
    })

});

    
