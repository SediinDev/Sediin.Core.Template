$(document).ready(function () {
    customValidatorOnBegin();

    // MutationObserver per intercettare modifiche ai messaggi di errore e tradurli in italiano
    const observer = new MutationObserver(function (mutationsList) {
        for (const mutation of mutationsList) {
            if (mutation.type === 'childList') {
                const target = mutation.target;
                if ($(target).hasClass('field-validation-error')) {
                    const id = $(target).attr('data-valmsg-for');
                    if (id) {
                        const input = $("#" + id);
                        const label = $("label[for='" + id + "']").text().replace("*", "").trim();
                        const requiredMsg = input.attr("data-val-required");
                        const emailMsg = input.attr("data-val-email");
                        const lengthMsg = input.attr("data-val-length");
                        const regexMsg = input.attr("data-val-regex");
                        // ... aggiungi altre traduzioni se vuoi

                        // Sostituisco solo se il messaggio attuale è in inglese o diverso
                        let newMessage = null;
                        const currentMsg = $(target).text();

                        if (requiredMsg && currentMsg.toLowerCase().includes("required")) {
                            newMessage = requiredMsg;
                        } else if (emailMsg && currentMsg.toLowerCase().includes("email")) {
                            newMessage = emailMsg;
                        } else if (lengthMsg && currentMsg.toLowerCase().includes("length")) {
                            newMessage = lengthMsg;
                        } else if (regexMsg && currentMsg.toLowerCase().includes("format")) {
                            newMessage = regexMsg;
                        }

                        if (newMessage && newMessage !== currentMsg) {
                            $(target).text(newMessage);
                        }
                    }
                }
            }
        }
    });

    // Osservo tutti i messaggi di errore nel form
    $("[data-valmsg-for]").each(function () {
        observer.observe(this, { childList: true });
    });
});


function customValidatorOnBegin() {
    // Traduci messaggi data-val in italiano dinamicamente
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
            "data-val-equalto": null // gestito dopo
        };

        Object.keys(translations).forEach(function (attr) {
            if ($el.attr(attr) !== undefined && translations[attr] !== null) {
                $el.attr(attr, translations[attr]);
            }
        });

        // Gestione speciale per data-val-equalto
        if ($el.attr("data-val-equalto") !== undefined) {
            const thisId = $el.attr("id");
            const otherSelector = $el.attr("data-val-equalto-other");
            let otherId = null;

            if (otherSelector && otherSelector.startsWith("*.")) {
                otherId = otherSelector.substring(2);
            }

            let otherLabel = otherId;
            if (otherId) {
                otherLabel = $("label[for='" + otherId + "']").text().replace("*", "").trim() || otherId;
            }

            const thisLabel = labelText || thisId;

            $el.attr("data-val-equalto", `Il campo ${thisLabel} deve corrispondere al campo ${otherLabel}.`);
        }
    }

    // Assegna classi e traduci messaggi ai campi text, password, textarea
    $("form .form-group").find("input[type='text'], input[type='password'], textarea").each(function () {
        if (!$(this).attr("class") || $(this).attr("class").trim() === "") {
            $(this).addClass("form-control col-md-12");
        }
        const id = $(this).attr("id");
        if (id) {
            const label = $("label[for='" + id + "']").text().replace("*", "").trim();
            if (label) translateDataValMessages($(this), label);
        }
    });

    // Assegna classe control-label a label senza classe
    $("form .form-group").find("label").each(function () {
        if (!$(this).attr("class") || $(this).attr("class").trim() === "") {
            $(this).addClass("control-label");
        }
    });

    // Applica placeholder dal testo della prima label del form-group se non presente (anche su password)
    $("form .form-group").each(function () {
        const $group = $(this);
        const input = $group.find("input[type='text'], input[type='password'], input[type='search'], input:not([type]), textarea").first();
        if (!input.length || input.attr("placeholder")) return;

        const label = $group.find("label").first();
        if (label.length > 0) {
            const labelText = label.text().replace("*", "").trim();
            if (labelText) input.attr("placeholder", labelText);
        }
    });

    // Validazione iniziale di tutti i campi (text, password, textarea)
    setWarningValidation();

    // Valida ogni volta che cambia il valore di un campo text, password, textarea
    $("form .form-group").find("input[type='text'], input[type='password'], textarea").on("input change", function () {
        validateInputField($(this).attr("id"));
    });
}

// FUNZIONI DI VALIDAZIONE

function clearValidationSummaryErrors() {
    $(".validation-summary-errors").hide();
}

function addValidationWarning(id) {
    try {
        clearFieldClasses(id);
        addClasses(id, "has-warning", ["form-control-warning", "field-warning"]);
        clearFieldMessage(id);
    } catch (e) { }
}

function addValidationError(id) {
    try {
        if (!id) return;
        clearFieldClasses(id);
        addClasses(id, "has-danger", ["input-validation-error", "field-error"]);
        setFieldMessage(id, $("#" + id).attr("data-val-required"));
    } catch (e) { }
}

function addValidationErrorMessage(id, error) {
    try {
        if (!id) return;
        clearFieldClasses(id);
        addClasses(id, "has-danger", ["input-validation-error", "field-error"]);
        setFieldMessage(id, error);
    } catch (e) { }
}

function removeValidationError(id) {
    try {
        clearFieldClasses(id);
        addClasses(id, "has-success", ["form-control-success", "field-success"]);
        clearFieldMessage(id);
    } catch (e) { }
}

function validateInputField(id) {
    if (!requiredElement(id)) {
        removeValidationError(id);
        return;
    }
    const val = $("#" + id).val();
    if (val === "") {
        addValidationError(id); // se vuoto, mostra errore rosso
    } else {
        removeValidationError(id);
    }
}

function requiredElement(id) {
    const el = $("#" + id);
    return el.data("val") && el.data("val-required") && el.data("val") === true;
}

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
                addValidationError(id); // mostra errore rosso se vuoto
                resetButtonsState($group);
            } else {
                removeValidationError(id);
                resetButtonsState($group);
            }
        }
    });
}

function addAsteriskToLabel(id) {
    const label = $("label[for='" + id + "']");
    if (label.length && !label.html().includes("*")) {
        label.append("<span class='text-danger'> *</span>");
    }
}

function resetButtonsState($group) {
    $group.find("input[type='submit'], input[type='button'], button")
        .removeClass("form-control-warning input-validation-error form-control-success");
}
