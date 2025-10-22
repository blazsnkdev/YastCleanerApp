document.addEventListener('DOMContentLoaded', function () {
    // Cálculo del saldo pendiente
    calcularSaldo();

    // Configuración de autocompletado para clientes
    const clienteInput = document.getElementById('cliente');
    const clienteIdInput = document.getElementById('clienteId');
    const sugerenciasContainer = document.getElementById('sugerencias');

    if (clienteInput) {
        clienteInput.addEventListener('input', function () {
            const query = this.value.trim();

            if (query.length < 2) {
                sugerenciasContainer.style.display = 'none';
                return;
            }

            // LLAMADA REAL A TU ENDPOINT
            buscarClientes(query);
        });
    }

    function buscarClientes(query) {
        fetch(`/Cliente/BuscarClienteJson?term=${encodeURIComponent(query)}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Error en la respuesta del servidor');
                }
                return response.json();
            })
            .then(data => {
                mostrarSugerencias(data);
            })
            .catch(error => {
                console.error('Error al buscar clientes:', error);
                sugerenciasContainer.innerHTML = '<div class="sugerencia-item text-danger">Error al cargar clientes</div>';
            });
    }

    function mostrarSugerencias(clientes) {
        sugerenciasContainer.innerHTML = '';

        if (!clientes || clientes.length === 0) {
            sugerenciasContainer.innerHTML = '<div class="sugerencia-item">No se encontraron clientes</div>';
            return;
        }

        clientes.forEach(cliente => {
            const item = document.createElement('div');
            item.className = 'sugerencia-item';
            item.textContent = cliente.label; // Usando 'label' de tu endpoint
            item.dataset.id = cliente.value;  // Usando 'value' de tu endpoint

            item.addEventListener('click', function () {
                clienteInput.value = cliente.label;
                clienteIdInput.value = cliente.value;
                sugerenciasContainer.style.display = 'none';
            });

            sugerenciasContainer.appendChild(item);
        });

        sugerenciasContainer.style.display = 'block';
    }

    // Cerrar sugerencias al hacer clic fuera
    document.addEventListener('click', function (e) {
        if (clienteInput && !clienteInput.contains(e.target) && !sugerenciasContainer.contains(e.target)) {
            sugerenciasContainer.style.display = 'none';
        }
    });

    // Referencias
    const pedidoForm = document.getElementById('pedidoForm');
    const inputFecha = document.getElementById('fecha');

    if (inputFecha) {
        // Establecer fecha mínima (no se puede elegir una anterior al momento actual)
        const ahora = new Date();
        const zonaLocal = new Date(ahora.getTime() - (ahora.getTimezoneOffset() * 60000));
        inputFecha.min = zonaLocal.toISOString().slice(0, 16);

        // Validar al cambiar la fecha
        inputFecha.addEventListener('change', function () {
            const seleccionada = new Date(this.value);
            if (seleccionada < new Date()) {
                Swal.fire({
                    title: 'Fecha inválida',
                    text: 'La fecha y hora no pueden ser anteriores al momento actual.',
                    icon: 'error',
                    confirmButtonColor: '#592af5'
                });
                this.value = '';
            }
        });
    }

    if (pedidoForm) {
        pedidoForm.addEventListener('submit', function (e) {
            const clienteId = document.getElementById('clienteId').value;
            const metodoPago = document.getElementById('metodoPago').value;
            const fecha = inputFecha.value;

            // Validar cliente
            if (!clienteId) {
                e.preventDefault();
                Swal.fire({
                    title: 'Cliente requerido',
                    text: 'Por favor seleccione un cliente válido.',
                    icon: 'warning',
                    confirmButtonColor: '#592af5'
                });
                return;
            }

            // Validar fecha
            const ahora = new Date();
            const seleccionada = new Date(fecha);
            if (!fecha || seleccionada < ahora) {
                e.preventDefault();
                Swal.fire({
                    title: 'Fecha inválida',
                    text: 'Por favor seleccione una fecha y hora válida (no en el pasado).',
                    icon: 'error',
                    confirmButtonColor: '#592af5'
                });
                return;
            }

            // Validar método de pago
            if (!metodoPago) {
                e.preventDefault();
                Swal.fire({
                    title: 'Método de pago requerido',
                    text: 'Por favor seleccione un método de pago.',
                    icon: 'warning',
                    confirmButtonColor: '#592af5'
                });
                return;
            }
        });
    }



});

function calcularSaldo() {
    const totalElement = document.getElementById('pedidoForm');
    if (!totalElement) return;

    const total = parseFloat(totalElement.dataset.total) || 0;
    const montoAdelantoInput = document.getElementById('montoAdelanto');
    const montoAdelanto = parseFloat(montoAdelantoInput.value) || 0;

    // Validar que no exceda el total
    if (montoAdelanto > total) {
        montoAdelantoInput.value = total.toFixed(2);
        montoAdelanto = total;
    }

    const saldoPendiente = total - montoAdelanto;

    const adelantoMostrado = document.getElementById('adelantoMostrado');
    const saldoPendienteElement = document.getElementById('saldoPendiente');

    if (adelantoMostrado) {
        adelantoMostrado.textContent = 'S/ ' + montoAdelanto.toFixed(2);
    }

    if (saldoPendienteElement) {
        saldoPendienteElement.textContent = 'S/ ' + saldoPendiente.toFixed(2);

        // Cambiar color si el saldo es cero
        if (saldoPendiente === 0) {
            saldoPendienteElement.className = 'fw-bold text-success';
        } else {
            saldoPendienteElement.className = 'fw-bold text-warning';
        }
    }
}

// Función para limpiar el campo de cliente
function limpiarCliente() {
    const clienteInput = document.getElementById('cliente');
    const clienteIdInput = document.getElementById('clienteId');
    const sugerenciasContainer = document.getElementById('sugerencias');

    if (clienteInput) clienteInput.value = '';
    if (clienteIdInput) clienteIdInput.value = '';
    if (sugerenciasContainer) sugerenciasContainer.style.display = 'none';
}