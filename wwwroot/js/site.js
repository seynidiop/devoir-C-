// Site JavaScript - Gestion des Approvisionnements

// Format number as currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('fr-FR').format(amount) + ' FCFA';
}

// Auto-dismiss alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function() {
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
});

// Confirm delete
function confirmDelete(message) {
    return confirm(message || 'Êtes-vous sûr de vouloir supprimer cet élément ?');
}

// Format date inputs
document.querySelectorAll('input[type="date"]').forEach(function(input) {
    if (!input.value) {
        input.value = new Date().toISOString().split('T')[0];
    }
});
