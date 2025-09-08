function toggleMaintenance() {
    const options = document.getElementById('maintenanceOptions');
    const arrow = document.querySelector('.maintenance-header .arrow i');

    options.classList.toggle('show');

    if (options.classList.contains('show')) {
        arrow.classList.remove('fa-chevron-down');
        arrow.classList.add('fa-chevron-up');
    } else {
        arrow.classList.remove('fa-chevron-up');
        arrow.classList.add('fa-chevron-down');
    }
}

// Marcar elemento activo al hacer clic
document.querySelectorAll('.main-menu a').forEach(link => {
    link.addEventListener('click', function () {
        document.querySelectorAll('.main-menu a').forEach(a => a.classList.remove('active'));
        this.classList.add('active');
    });
});