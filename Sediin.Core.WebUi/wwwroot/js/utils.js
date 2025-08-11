
function alert(text) {
    Swal.fire(text);
}

function playNotifyInfo() {
    playNotify("/sound/info.mp3");
}

function playNotifyConfirm() {
    playNotify("/sound/confirm2.mp3");
}

function playNotifyAlert() {
    playNotify("/Content/sound/alert.mp3");
}

function playNotifyError() {
    playNotify("/sound/error.mp3");
}

function playNotifySuccess() {
    playNotify("/sound/success.mp3");
}

function playNotifyWarning() {
    playNotify("/sound/warning.mp3");
}

function playNotify(url) {
    var mySound = soundManager.createSound({
        url: url,
        useConsole: false,
        //consoleOnly:false
    });
    mySound.play();
}

function alertWarning(text) {
    Swal.fire({
        icon: 'warning',
        //title: 'Attenzione',
        html: text,
    });
    playNotifyWarning();
}

function alertDanger(text) {
    Swal.fire({
        icon: 'error',
        //title: 'Errore',
        html: text,
    });
    playNotifyError();
}

function alertInfo(text) {
    Swal.fire({
        icon: 'info',
        html: text,
    });
    playNotifyInfo();
}

function alertInfoNoCloseButton(text) {
    Swal.fire({
        icon: 'info',
        html: text,
        showConfirmButton: false,
        allowOutsideClick: false,
        allowEscapeKey: false
    });
    playNotifyInfo();
}

function alertSuccess(text) {
    Swal.fire({
        icon: 'success',
        //title: 'Informazione',
        html: text,
    });
    playNotifySuccess();
}

function alertSuccessNoCloseButton(text) {
    Swal.fire({
        icon: 'success',
        //title: 'Informazione',
        html: text,
        showConfirmButton: false,
        allowOutsideClick: false,
        allowEscapeKey: false
    });
    playNotifySuccess();
}

function alertWaid(text) {
    Swal.fire({
        html: text == undefined ? "Attendere, operazione in corso..." : text,
        iconHtml: '<div class=\"spinner-border text-primary\"></div>',
        //title: 'Operazione in corso...',
        showConfirmButton: false,
        allowOutsideClick: false,
        allowEscapeKey: false
    });
}

function alertClose() {
    Swal.close();
}


function toastWarning(message, showDuration, position) {
    toast("warning", message, showDuration, position);
    playNotifyWarning();
}

function toastInfo(message, showDuration, position) {
    toast("info", message, showDuration, position);
    playNotifyInfo();
}

function toastSuccess(message, showDuration, position) {
    toast("success", message, showDuration, position);
    playNotifySuccess();
}
function toastError(message, showDuration, position) {
    toast("error", message, showDuration, position);
    playNotifyError();
}

function toast(tipo, message, showDuration, position) {

    //toast-top-right
    //toast-bottom-right
    //toast-bottom-left
    //toast-top-left
    //toast-top-full-width
    //toast-bottom-full-width
    //toast-top-center
    //toast-bottom-center

    var _position = "toast-bottom-right";
    if (position != undefined) {
        _position = position;
    }

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        "positionClass": _position,
        "preventDuplicates": true,
        "onclick": null,
        "closeDuration": false,
        "showDuration": showDuration == undefined ? "10000" : showDuration,
        "hideDuration": "1000",
        "timeOut": "12000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    toastr[tipo](message);
}


//handle ajax error, HandleAjaxErrorAttribute
function handleError(xhr) {
    try {

        //console.log("xhr.responseText: " + xhr.responseText);

        const res = JSON.parse(xhr.responseText);
        if (res && res.message) {
            alertDanger(res.message);
        } else {
            alertDanger("Si è verificato un errore sconosciuto.");
        }
    } catch {
        let msg = xhr.responseText;
        if (!msg || msg.trim() === "") {
            msg = "Si è verificato un errore imprevisto.";
        }
        alertDanger(msg);
    }
}

function scrollToElement(e) {
    try {
        $('html, body').animate({
            scrollTop: $(e).offset().top - 50
        }, 800);
    } catch (e) {

    }
}

function scrollModalTop() {
    $('.modal').animate({ scrollTop: 0 }, 'slow');
}

function scrollTop() {
    try {
        $('html, body').animate({
            scrollTop: 0
        }, 800);
    } catch (e) {
    }
}

//.jpg, .png, .pdf
function validateFileType(file, allowedExtensions) {
    const fileName = file.name.toLowerCase();
    const isValid = allowedExtensions.some(ext => fileName.endsWith(ext.toLowerCase()));
    if (!isValid) {
        return {
            valid: false,
            message: `Il file deve essere uno dei seguenti tipi: ${allowedExtensions.join(', ')}.`
        };
    }
    return { valid: true };
}

function validateFileSize(file, maxSizeMB) {
    const maxSizeBytes = maxSizeMB * 1024 * 1024;
    if (file.size > maxSizeBytes) {
        return {
            valid: false,
            message: `La dimensione dell'file non deve superare ${maxSizeMB}MB.`
        };
    }
    return { valid: true };
}

function confirmAndPostForm(formId, message) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mr-1',
            cancelButton: 'btn btn-danger mr-1'
        },
        buttonsStyling: false
    });

    playNotifyConfirm();

    swalWithBootstrapButtons.fire({
        html: message,
        icon: "question",
        showCancelButton: true,
        confirmButtonText: 'Conferma',
        cancelButtonText: 'Annulla',
        allowOutsideClick: false,
        allowEscapeKey: false
    }).then((result) => {
        if (result.isConfirmed) {
            $("#" + formId).submit();
        }
    });
}


/*  
esempio

function eliminaUtente(id) {
    confirmAndPostAction("@Url.Action("EliminaUtente", "Utenti")", { id }, "Sicuro di voler \"eliminare\" questa utenza?", onSuccessRimuoviUtenza);
}

function onSuccessRimuoviUtenza(data) {
    updateListRicerca();
    alertClose();
    toastSuccess("Utente eliminato");
}

*/
function confirmAndPostAction(action, params, message, callbackFunction) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mr-1',
            cancelButton: 'btn btn-danger mr-1'
        },
        buttonsStyling: false
    });

    playNotifyConfirm();

    swalWithBootstrapButtons.fire({
        html: message,
        icon: "question",
        showCancelButton: true,
        confirmButtonText: 'Conferma',
        cancelButtonText: 'Annulla',
        allowOutsideClick: false,
        allowEscapeKey: false
    }).then((result) => {
        if (result.isConfirmed) {
            alertWaid();
            $.post(action, params, function (data) {
                if (typeof callbackFunction === 'function') {
                    callbackFunction(data);
                } else {
                    alertClose();
                    toastSuccess(data);
                    updateListRicerca();
                }
            }).fail(function (err) {
                handleError(err);
            });
        }
    });
}
