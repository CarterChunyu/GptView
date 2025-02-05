function SuccessAlert(title, msg) {
    SweatAlert(title, msg, 'success', '確定');
}

function ErrorAlert(title, msg) {
    SweatAlert(title, msg, 'error', '確定');
}


function SweatAlert(title, msg, icon, confirmButtonText) {
    Swal.fire({
        title: title,
        text: msg,
        icon: icon,
        confirmButtonText: confirmButtonText
    });
}

