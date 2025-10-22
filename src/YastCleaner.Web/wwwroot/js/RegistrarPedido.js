document.addEventListener('DOMContentLoaded', function () {
    
    calcularSaldo();

    
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
            item.textContent = cliente.label; 
            item.dataset.id = cliente.value;  

            item.addEventListener('click', function () {
                clienteInput.value = cliente.label;
                clienteIdInput.value = cliente.value;
                sugerenciasContainer.style.display = 'none';
            });

            sugerenciasContainer.appendChild(item);
        });

        sugerenciasContainer.style.display = 'block';
    }


    document.addEventListener('click', function (e) {
        if (clienteInput && !clienteInput.contains(e.target) && !sugerenciasContainer.contains(e.target)) {
            sugerenciasContainer.style.display = 'none';
        }
    });

    
    const pedidoForm = document.getElementById('pedidoForm');
    const inputFecha = document.getElementById('fecha');

    if (inputFecha) {
        
        const ahora = new Date();
        const zonaLocal = new Date(ahora.getTime() - (ahora.getTimezoneOffset() * 60000));
        inputFecha.min = zonaLocal.toISOString().slice(0, 16);


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
    let montoAdelanto = parseFloat(montoAdelantoInput.value) || 0;

    if (montoAdelantoInput.value.includes('-') || montoAdelanto < 0) {
        montoAdelantoInput.value = '';
        montoAdelanto = 0;
        Swal.fire({
            title: 'Monto inválido',
            text: 'El monto adelantado no puede ser negativo.',
            icon: 'warning',
            confirmButtonColor: '#592af5'
        });
    }


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


        if (saldoPendiente === 0) {
            saldoPendienteElement.className = 'fw-bold text-success';
        } else {
            saldoPendienteElement.className = 'fw-bold text-warning';
        }
    }
}



function limpiarCliente() {
    const clienteInput = document.getElementById('cliente');
    const clienteIdInput = document.getElementById('clienteId');
    const sugerenciasContainer = document.getElementById('sugerencias');

    if (clienteInput) clienteInput.value = '';
    if (clienteIdInput) clienteIdInput.value = '';
    if (sugerenciasContainer) sugerenciasContainer.style.display = 'none';
}