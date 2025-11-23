async function manejarErrorApi(respuesta) {
    let mensajeError = '';
    if (respuesta.status === 400) {
        mensajeError = await respuesta.text();
    } else if (respuesta.status === 404) {
        mensajeError = recursoNoEncontrado;
    } else {
        mensajeError = recursoInesperado;
    }

    mostrarError(mensajeError);
}


function mostrarError(mensaje) {
    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: mensaje,
    });
}



function confirmarAccion({ callBackAceptar, callBackCancelar, title }) {
    Swal.fire({
        title: title || "¿Realmente deseas hacer esto?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: 'Sí',
        focusConfirm: true
    }).then((resultado) => {
        if (resultado.isConfirmed) {
            callBackAceptar();
        } else if (callBackCancelar) {
            //El usuario ha presionado el botón de cancelar
            callBackCancelar();
        }
    })
}