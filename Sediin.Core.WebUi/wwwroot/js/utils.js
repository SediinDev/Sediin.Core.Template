
//sweetalert


function alert(text) {
    Swal.fire(text);
}

function playNotifyInfo() {
    playNotify("/Content/sound/info.mp3");
}

function playNotifyConfirm() {
    playNotify("/Content/sound/confirm2.mp3");
}

function playNotifyAlert() {
    playNotify("/Content/sound/alert.mp3");
}

function playNotifyError() {
    playNotify("/Content/sound/error.mp3");
}

function playNotifySuccess() {
    playNotify("/Content/sound/success.mp3");
}

function playNotifyWarning() {
    playNotify("/Content/sound/warning.mp3");
}

function playNotify(url) {
//    var mySound = soundManager.createSound({
//        url: url,
//        useConsole: false,
//        //consoleOnly:false
//    });
//    mySound.play();
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

//handle ajax error, HandleAjaxErrorAttribute
function handleError(xhr) {
    try {

        console.log("xhr.responseText: " + xhr.responseText);

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
