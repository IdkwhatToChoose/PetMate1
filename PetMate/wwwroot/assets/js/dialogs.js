

    var dialog = document.getElementById("confirmationDialog");


    function showDialog() {
        if (dialog) {
            dialog.style.top = '20px'; 
            dialog.style.opacity = '1'; 
        }
    }

// Function to close the dialog
function closeDialog() {
    dialog.style.top = '-100px'; // Move dialog out of view
    dialog.style.opacity = '0'; // Fade out
}


document.addEventListener("DOMContentLoaded", function () {
    showDialog();

    // Auto-close after 4 seconds
    setTimeout(function () {
        closeDialog();
    }, 4000);
});