$(document).ready(function () {
    customValidatorOnBegin();
    setPlaceholdersFromLabels();

    // MutationObserver per intercettare modifiche ai messaggi di errore e tradurli in italiano
    const observer = new MutationObserver(function (mutationsList) {
        for (const mutation of mutationsList) {
            if (mutation.type === 'childList') {
                const target = mutation.target;
                if ($(target).hasClass('field-validation-error')) {
                    const id = $(target).attr('data-valmsg-for');
                    if (id) {
                        const input = $("#" + id);
                        const labelText = getCleanLabelText(id);

                        const requiredMsg = input.attr("data-val-required");
                        const emailMsg = input.attr("data-val-email");
                        const lengthMsg = input.attr("data-val-length");
                        const regexMsg = input.attr("data-val-regex");

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

    // Gestione reset form per pulire errori e stati
    $("form").on("reset", function () {
        // Aspetta qualche ms per permettere al reset di avvenire
        setTimeout(() => {
            // Pulisci messaggi errore
            $("[data-valmsg-for]").each(function () {
                $(this).html("").removeClass("field-validation-error");
            });

            // Pulisci classi di stato (has-danger, has-warning, has-success e classi input)
            $(this).find("input, textarea, select").each(function () {
                const id = $(this).attr("id");
                if (!id) return;
                // Rimuovi classi errore/avviso/successo
                customValidatorOnBegin(id);
            });
        }, 10);

        removeValidClassFromCheckboxes();
    });
});

$("form button[type='submit'], form input[type='submit']").on("click", function () {
    removeValidClassFromCheckboxes()
});

// Funzione per leggere testo pulito del label (senza figli, asterisco e duplicati consecutivi)
//function getCleanLabelText(id) {
//    const label = $("label[for='" + id + "']");
//    if (!label.length) return "";
//    let text = label.clone().children().remove().end().text().replace(/\*/g, "").trim();
//    // Rimuove duplicati consecutivi, tipo CognomeCognome → Cognome
//    text = text.replace(/(\b\w+\b)\1+/gi, "$1");
//    return text;
//}

function getCleanLabelText(id) {
    const label = $("label[for='" + id + "']");
    if (!label.length) return "";
    let text = label.clone().children().remove().end().text().replace(/\*/g, "").trim();

    // Se la lunghezza è pari, prova a dividere a metà e confrontare le due metà
    if (text.length % 2 === 0) {
        const half = text.length / 2;
        const part1 = text.substring(0, half);
        const part2 = text.substring(half);
        if (part1 === part2) {
            text = part1; // rimuovo la seconda metà duplicata
        }
    }

    // Rimuove duplicati consecutivi di parole (es. CognomeCognome -> Cognome)
    text = text.replace(/(\b\w+\b)\1+/gi, "$1");

    return text;
}


// Rimuove parole duplicate consecutive in una stringa
function removeDuplicateWords(str) {
    if (!str) return str;
    return str.replace(/(\b\w+\b)\1+/gi, "$1");
}

function customValidatorOnBegin() {
    // Traduci messaggi data-val in italiano dinamicamente, solo se diverso da quello già presente
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
                let currentVal = $el.attr(attr);
                let newVal = translations[attr];

                currentVal = removeDuplicateWords(currentVal);
                newVal = removeDuplicateWords(newVal);

                if (currentVal !== newVal) {
                    $el.attr(attr, newVal);
                }
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
                otherLabel = getCleanLabelText(otherId) || otherId;
            }

            const thisLabel = labelText || thisId;
            const equalToMsg = `Il campo ${thisLabel} deve corrispondere al campo ${otherLabel}.`;

            if ($el.attr("data-val-equalto") !== equalToMsg) {
                $el.attr("data-val-equalto", equalToMsg);
            }
        }
    }

    // Assegna classi e traduci messaggi ai campi text, password, textarea
    $("form .form-group").find("input[type='text'], input[type='password'], textarea").each(function () {
        if (!$(this).attr("class") || $(this).attr("class").trim() === "") {
            $(this).addClass("form-control col-md-12");
        }
        const id = $(this).attr("id");
        if (id) {
            const labelText = getCleanLabelText(id);
            if (labelText) translateDataValMessages($(this), labelText);

            if ($(this).attr("type") === "password") {
                $(this).val("");
            }
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

        const labelText = getCleanLabelText(input.attr("id"));
        if (labelText) input.attr("placeholder", labelText);
    });

    // Validazione iniziale di tutti i campi (text, password, textarea)
    setWarningValidation();
    
    // Valida ogni volta che cambia il valore di un campo text, password, textarea
    $("form .form-group").find("input[type='text'], input[type='password'], textarea").on("input change", function () {
        validateInputField($(this).attr("id"));
    });

    $("form .form-check input[type='checkbox']").on("change", function () {
        removeValidClassFromCheckboxes();
    });
}

// FUNZIONI DI VALIDAZIONE

function setPlaceholdersFromLabels() {
    $("form .form-group").each(function () {
        const $group = $(this);
        const input = $group.find("input[type='text'], input[type='password'], input[type='search'], input:not([type]), textarea").first();
        if (!input.length) return;

        // Se ha già placeholder, skip
        if (input.attr("placeholder")) return;

        let labelText = "";

        // Prova label con for
        const inputId = input.attr("id");
        if (inputId) {
            const label = $("label[for='" + inputId + "']");
            if (label.length) {
                labelText = label.text().replace(/\*/g, "").trim();
            }
        }

        // Se non trovato, prova la prima label dentro form-group
        if (!labelText) {
            const label = $group.find("label").first();
            if (label.length) {
                labelText = label.text().replace(/\*/g, "").trim();
            }
        }

        if (labelText) {
            input.attr("placeholder", labelText);
        }
    });
}

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
            // Non richiesto → stato success (verde)
            removeValidationError(id);
            return;
        }

        if (!input.prop("disabled")) {
            addAsteriskToLabel(id);

            if (input.val() === "") {
                // Campo richiesto vuoto → stato warning (giallo)
                addValidationWarning(id);
                resetButtonsState($group);
            } else {
                // Campo richiesto con valore → stato success (verde)
                removeValidationError(id);
                resetButtonsState($group);
            }
        }
    });
}

function addAsteriskToLabel(id) {
    const label = $("label[for='" + id + "']");
    if (label.length) {
        // controlla se c'è già uno span con classe text-danger e testo esatto "*"
        const hasAsterisk = label.children("span.text-danger").filter(function () {
            return $(this).text().trim() === "*";
        }).length > 0;

        if (!hasAsterisk) {
            label.append("<span class='text-danger'> *</span>");
        }
    }
}

function resetButtonsState($group) {
    $group.find("input[type='submit'], input[type='button'], button")
        .removeClass("form-control-warning input-validation-error form-control-success");
}

function removeValidClassFromCheckboxes() {
    setTimeout(function () {
        $("form .form-check input[type='checkbox']").removeClass("valid");
    });
}