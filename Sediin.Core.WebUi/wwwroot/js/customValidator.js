$(document).ready(function () {

    // Funzione per tradurre messaggi comuni in italiano con nome campo dinamico
    function translateDataValMessages($el, labelText) {
        const translations = {
            "data-val-required": `Il campo ${labelText} è obbligatorio.`,
            "data-val-email": `Il campo ${labelText} deve contenere un indirizzo email valido.`,
            "data-val-length": `Il campo ${labelText} deve rispettare la lunghezza richiesta.`,
            "data-val-regex": `Il campo ${labelText} ha un formato non valido.`,
            "data-val-date": `Il campo ${labelText} deve essere una data valida.`,
            "data-val-number": `Il campo ${labelText} deve essere un numero.`,
            "data-val-minlength": `Il campo ${labelText} è troppo corto.`,
            "data-val-maxlength": `Il campo ${labelText} è troppo lungo.`,
            "data-val-range": `Il campo ${labelText} è fuori dall'intervallo consentito.`,
            "data-val-equalto": null // lo gestiamo dopo
        };

        Object.keys(translations).forEach(function (attr) {
            if ($el.attr(attr) !== undefined && translations[attr] !== null) {
                $el.attr(attr, translations[attr]);
            }
        });

        // Gestione speciale per data-val-equalto
        if ($el.attr("data-val-equalto") !== undefined) {
            const thisId = $el.attr("id");
            const otherSelector = $el.attr("data-val-equalto-other"); // es: "*.Email"
            let otherId = null;

            if (otherSelector && otherSelector.startsWith("*.")) {
                otherId = otherSelector.substring(2); // prendo "Email"
            }

            let otherLabel = otherId;
            if (otherId) {
                otherLabel = $("label[for='" + otherId + "']").text().replace("*", "").trim() || otherId;
            }

            const thisLabel = labelText || thisId;

            $el.attr("data-val-equalto", `Il campo ${thisLabel} deve corrispondere al campo ${otherLabel}.`);
        }
    }

    // Assegna classi form-control e col-md-12 se mancanti
    $("form .form-group").find("input[type='text'], textarea").each(function () {
        if (!$(this).attr("class") || $(this).attr("class").trim() === "") {
            $(this).addClass("form-control col-md-12");
        }

        const id = $(this).attr("id");
        if (id) {
            const label = $("label[for='" + id + "']").text().replace("*", "").trim();
            if (label) {
                translateDataValMessages($(this), label);
            }
        }
    });

    // Assegna classe control-label a label senza classe
    $("form .form-group").find("label").each(function () {
        if (!$(this).attr("class") || $(this).attr("class").trim() === "") {
            $(this).addClass("control-label");
        }
    });

    setWarningValidation();
    applyLabelPlaceholder();
});

// Nasconde gli errori nel summary
function clearValidationSummaryErrors() {
    $(".validation-summary-errors").hide();
}

// Aggiunge warning (campo obbligatorio non compilato)
function addValidationWarning(id) {
    try {
        clearFieldClasses(id);
        addClasses(id, "has-warning", ["form-control-warning", "field-warning"]);
        clearFieldMessage(id);
    } catch (e) { }
}

// Aggiunge errore di validazione (vuoto)
function addValidationError(id) {
    try {
        if (!id) return;
        clearFieldClasses(id);
        addClasses(id, "has-danger", ["input-validation-error", "field-error"]);
        setFieldMessage(id, $("#" + id).attr("data-val-required"));
    } catch (e) { }
}

// Aggiunge errore con messaggio custom
function addValidationErrorMessage(id, error) {
    try {
        if (!id) return;
        clearFieldClasses(id);
        addClasses(id, "has-danger", ["input-validation-error", "field-error"]);
        setFieldMessage(id, error);
    } catch (e) { }
}

// Rimuove tutti gli stati di errore e mostra successo
function removeValidationError(id) {
    try {
        clearFieldClasses(id);
        addClasses(id, "has-success", ["form-control-success", "field-success"]);
        clearFieldMessage(id);
    } catch (e) { }
}

// Validazione semplice per un singolo campo
function validateInputField(id) {
    if (requiredElement(id) && $("#" + id).val() === "") {
        addValidationWarning(id);
    } else {
        removeValidationError(id);
    }
}

// Controlla se il campo è obbligatorio secondo data-val
function requiredElement(id) {
    const el = $("#" + id);
    return el.data("val") && el.data("val-required") && el.data("val") === true;
}

// Verifica se ha errori visivi
function hashFieldErrorClass(id) {
    const el = $("#" + id);
    return el.length > 0 && !el.prop("disabled") && el.hasClass("field-error");
}

// Valida tutti i campi di un form
function setWarningValidation() {
    $("form .form-group").each(function () {
        const $group = $(this);
        const input = $group.find("input,textarea,select").first();
        const id = input.attr("id");

        if (!id) return;

        clearFieldMessage(id);

        if (!requiredElement(id)) {
            removeValidationError(id);
            return;
        }

        if (!input.prop("disabled")) {
            addAsteriskToLabel(id);

            if (input.val() === "") {
                addValidationWarning(id);
                resetButtonsState($group);
            } else {
                removeValidationError(id);
                resetButtonsState($group);
            }
        }
    });
}

// Applica il placeholder automatico leggendo dal <label> associato
function applyLabelPlaceholder() {
    $("form .form-group").each(function () {
        const $group = $(this);
        const input = $group.find("input[type='text'], input[type='search'], input:not([type]), textarea").first();

        if (!input.length || input.attr("placeholder")) return;

        const label = $group.find("label").first();
        if (label.length > 0) {
            const labelText = label.text().replace("*", "").trim();
            if (labelText) {
                input.attr("placeholder", labelText);
            }
        }
    });
}

//function applyLabelPlaceholder() {
//    $("form .form-group").each(function () {
//        const $group = $(this);
//        const input = $group.find("input[type='text'], input[type='search'], input:not([type]), textarea").first();
//        if (!input.length || input.attr("placeholder")) return;

//        let id = input.attr("id");
//        let labelText = "";

//        // Primo caso: cerca <label for="id">
//        if (id) {
//            const label = $group.find("label[for='" + id + "']");
//            if (label.length > 0) {
//                labelText = label.text().replace("*", "").trim();
//            }
//        }

//        // Secondo caso: nessun for, prendi il primo <label> nel form-group
//        if (!labelText) {
//            const label = $group.find("label").first();
//            if (label.length > 0) {
//                labelText = label.text().replace("*", "").trim();
//            }
//        }

//        // Applica il placeholder se trovato
//        if (labelText) {
//            input.attr("placeholder", labelText);
//        }
//    });
//}

// Helpers

function clearFieldClasses(id) {
    const sel = $("#" + id);
    const group = sel.closest(".form-group");

    sel.removeClass("input-validation-error form-control-warning form-control-success field-error field-warning field-success");
    group.removeClass("has-danger has-warning has-success");
}

function addClasses(id, groupClass, controlClasses) {
    const sel = $("#" + id);
    const group = sel.closest(".form-group");
    group.addClass(groupClass);
    controlClasses.forEach(c => sel.addClass(c));
}

function setFieldMessage(id, message) {
    const msgEl = $("[data-valmsg-for='" + id + "']");
    msgEl.html(message);
    msgEl.addClass("field-validation-error");
}

function clearFieldMessage(id) {
    const msgEl = $("[data-valmsg-for='" + id + "']");
    msgEl.html("");
    msgEl.removeClass("field-validation-error");
}

function resetButtonsState($group) {
    $group.find("input[type='submit'], input[type='button'], button")
        .removeClass("form-control-warning input-validation-error form-control-success");
}

function addAsteriskToLabel(id) {
    const label = $("label[for='" + id + "']");
    if (label.length && !label.html().includes("*")) {
        label.append("<span class='text-danger'> *</span>");
    }
}
