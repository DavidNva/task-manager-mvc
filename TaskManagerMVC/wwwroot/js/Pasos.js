function manejarClickAgregarPaso() {
    const indice = tareaEditaVM.steps().findIndex(p => p.esNuevo());

    if (indice !== -1) {
        return;
    }

    tareaEditaVM.steps.push(new pasoViewModel({ modoEdicion: true, realizad: false }));
    $("[name=txtPasoDescription]:visible").focus();
}

function manejarClickCancelarPaso() {
    if (paso.esNuevo()) {
        tareaEditaVM.steps.pop();
    } else {
        console.log("Else cancelar steps")
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