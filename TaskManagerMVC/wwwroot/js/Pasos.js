function manejarClickAgregarPaso() {
    const indice = tareaEditaVM.pasos().findIndex(p => p.esNuevo());

    if (indice !== -1) {
        return;
    }

    tareaEditaVM.pasos.push(new pasoViewModel({ modoEdicion: true, realizad: false }));
    $("[name=txtPasoDescription]:visible").focus();
}

function manejarClickCancelarPaso() {
    if (paso.esNuevo()) {
        tareaEditaVM.pasos.pop();
    } else {
        console.log("Else cancelar pasos")
    }
}

async function manejarClickSalvarPaso(paso) {
    paso.modoEdicion(false);
    const esNuevo = paso.esNuevo();
    const idTarea = tareaEditaVM.id;
    const data = obtenerCuerpoPeticionPaso(paso);

    if (esNuevo) {
        await insertarPaso(paso, data, idTarea);
    } else {
        console.log("Actualizando");
    }
}

async function insertarPaso(paso, data, idTarea) {
    const respuesta = await fetch(`${urlPasos}/CrearPaso/${idTarea}`, {
        body: data,
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        paso.id(json.id);
    } else {
        manejarErrorApi(respuesta);
    }
}
function obtenerCuerpoPeticionPaso(paso) {
    return JSON.stringify({
        description: paso.description(),
        realizado: paso.realizado()
    });
}