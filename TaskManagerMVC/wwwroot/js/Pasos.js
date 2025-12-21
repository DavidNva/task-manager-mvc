function manejarClickAgregarPaso() {
    const indice = tareaEditaVM.steps().findIndex(p => p.esNuevo());

    if (indice !== -1) {
        return;
    }

    tareaEditaVM.steps.push(new pasoViewModel({ modoEdicion: true, isCompleted: false }));
    $("[name=txtPasoDescription]:visible").focus();
}

function manejarClickCancelarPaso(paso) {
    if (paso.esNuevo()) {
        tareaEditaVM.steps.pop();
    } else {
        paso.modoEdicion(false);
        paso.description(paso.descriptionAnterior);
    }
}

async function manejarClickSalvarPaso(paso) {
    paso.modoEdicion(false);
    const esNuevo = paso.esNuevo();
    const idTarea = tareaEditaVM.id;
    const data = obtenerCuerpoPeticionPaso(paso);

    const descripcion = paso.description();
    if (!descripcion) {
        paso.descripcion(paso.descriptionAnterior);
        if (esNuevo) {
            tareaEditaVM.pasos.pop();
        }
        return;
    }
    if (esNuevo) {
        await insertarPaso(paso, data, idTarea);
    } else {
        actualizarPaso(data, paso.id());
    }
}

async function insertarPaso(paso, data, idTarea) {
    const respuesta = await fetch(`${urlsteps}/CrearPaso/${idTarea}`, {
        body: data,
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        paso.id(json.id);

        const tarea = obtenerTareaEnEdicion();
        tarea.pasosTotal(tarea.pasosTotal() + 1);
        console.log(paso);
        if (paso.IsCompleted()) {
            tarea.pasosRealizados(tarea.pasosRealizados() + 1);
        }
    } else {
        manejarErrorApi(respuesta);
    }
}
function obtenerCuerpoPeticionPaso(paso) {
    return JSON.stringify({
        description: paso.description(),
        IsCompleted: paso.IsCompleted()
    });
}

function manejarClickDescripcionPaso(paso) {
    paso.modoEdicion(true);
    paso.descriptionAnterior = paso.description();
    $("[name=txtPasoDescription]:visible").focus();
}

async function actualizarPaso(data, id) {
    const respuesta = await fetch(`${urlsteps}/${id}`,{
        body: data,
        method:"PUT",
        headers: {
            'Content-Type':'application/json'
        }
    });
    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
    }
}


function manejarClickCheckboxPaso(paso) {
    if (paso.esNuevo()) {
        return true;
    }

    const data = obtenerCuerpoPeticionPaso(paso);
    console.log(data);
    actualizarPaso(data, paso.id());

    const tarea = obtenerTareaEnEdicion();
    let pasosRealizadosActual = tarea.pasosRealizados();
    if (paso.IsCompleted()) {
        pasosRealizadosActual++;
    } else {
        pasosRealizadosActual--;
    }
    tarea.pasosRealizados(pasosRealizadosActual);

    return true;
}

function manejarClickBorrarPaso(paso) {
    modalEditarTareaBootstrap.hide();
    confirmarAccion({
        callBackAceptar: () => {
            borrarPaso(paso);
            modalEditarTareaBootstrap.show();
        },
        callBackCancelar: () => {
            modalEditarTareaBootstrap.show();
        },
        title:`¿Desea borrar este paso?`
    })
}

async function borrarPaso(paso) {
    const respuesta = await fetch(`${urlsteps}/${paso.id()}`, {
        method: 'DELETE'
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }
    tareaEditaVM.steps.remove(function (item) { return item.id() == paso.id() })

    const tarea = obtenerTareaEnEdicion();
    tarea.pasosTotal(tarea.pasosTotal() - 1);
    if (paso.IsCompleted()) {
        tarea.pasosRealizados(tarea.pasosRealizados() - 1);
    }
}