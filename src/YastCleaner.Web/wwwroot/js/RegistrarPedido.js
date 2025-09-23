function calcularSaldo() {
    // Obtener el total del dataset del formulario
    let total = document.getElementById('pedidoForm').dataset.total;
    total = total.replace(/[^\d.-]/g, '');
    total = parseFloat(total) || 0;

    const adelanto = parseFloat(document.getElementById('montoAdelanto').value) || 0;
    const saldo = total - adelanto;

    document.getElementById('adelantoMostrado').textContent = 'S/ ' + adelanto.toFixed(2);
    document.getElementById('saldoPendiente').textContent = 'S/ ' + saldo.toFixed(2);

    // Validar que el adelanto no supere el total
    if (adelanto > total) {
        document.getElementById('montoAdelanto').classList.add('is-invalid');
    } else {
        document.getElementById('montoAdelanto').classList.remove('is-invalid');
    }
}

// Validación del formulario
function validarFormulario() {
    document.getElementById('pedidoForm').addEventListener('submit', function (e) {
        let total = document.getElementById('pedidoForm').dataset.total;
        total = total.replace(/[^\d.-]/g, '');
        total = parseFloat(total) || 0;

        const adelanto = parseFloat(document.getElementById('montoAdelanto').value) || 0;

        if (adelanto > total) {
            e.preventDefault();
            alert('El monto adelantado no puede ser mayor al total del pedido.');
        }
    });
}

document.addEventListener('DOMContentLoaded', function () {
    // Limpiar y convertir el total una vez al cargar
    let total = document.getElementById('pedidoForm').dataset.total;
    total = total.replace(/[^\d.-]/g, '');
    total = parseFloat(total) || 0;

    // Actualizar el HTML para usar el valor numérico
    document.getElementById('montoAdelanto').setAttribute('max', total);
    document.querySelector('.form-text').textContent = 'Máximo permitido: S/ ' + total.toFixed(2);

    calcularSaldo();
    validarFormulario();

    // Autocomplete
    $("#cliente").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Cliente/BuscarClienteJson',
                data: { term: request.term },
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            $("#cliente").val(ui.item.label);
            $("#clienteId").val(ui.item.value);
            return false;
        }
    });
});