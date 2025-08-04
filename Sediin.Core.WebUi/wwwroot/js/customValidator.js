$(function () {
    $('form').submit(function () {

        if (!$(this).valid()) {
            return false;
        }

        $('.validation-summary-errors').each(function () {
            $(this).show();

            $(this).addClass('alert');
            $(this).addClass('alert-danger');
            return;

            var $_parent = $(this).parent();

            var $_ul = $(this).find("ul");
            $(this).remove();

            var resultArray = new Array();

            $_ul.find("li").each(function () {

                if (resultArray.indexOf($(this).text()) == -1)
                    resultArray.push($(this).text());

            });

            var _result = "";

            for (var i = 0; i < resultArray.length; i++) {
                _result += "<li>" + resultArray[i] + "</li>";
            }

            if (_result != "") {
                $("<div class='validation-summary-errors'><ul>" + _result + "</ul></div>").appendTo($_parent);
            }
        });

        if ($(this).valid()) {
            $(this).find('.form-group').each(function () {

                if ($(this).find('span.field-validation-error').length == 0) {
                    $(this).removeClass('has-danger');
                    $(this).addClass('has-success');
                }
            });
        }
    });

    // check each form-group for errors on ready
    $('form').each(function () {
        
        $(this).find('.form-group').each(function () {
            var _id = $(this).find("input,textarea,select").attr("id");

            if (_id == undefined) {
                return;
            }

            if (!requiredElement(_id)) {
                removeValidationError(_id);
                return;
            }

            if ($(this).find("input,textarea,select").prop("disabled") != true) {

                if (requiredElement(_id)) {
                    var label = $("label[for='" + _id + "']").html();

                    if (label != undefined) {
                        if (label.indexOf("*") == -1)
                            $("label[for='" + _id + "']").append("<span class='text-danger'> *</span>");
                    }
                }

                if ($("#" + _id).val() != "") {
                    removeValidationError(_id);
                    return;
                }

                if (requiredElement(_id) && $("#" + _id).val() == "") {

                    $(this).find("input,textarea,select").removeClass("field-warning");
                    $(this).find("input,textarea,select").removeClass("field-success");
                    $(this).find("input,textarea,select").addClass("field-warning");

                    $(this).addClass('has-warning');
                    $(this).find("input,textarea,select").addClass("form-control-warning");

                    $(this).find("input[type='submit']").removeClass("form-control-warning");
                    $(this).find("input[type='submit']").removeClass("input-validation-error");
                    $(this).find("input[type='submit']").removeClass("form-control-success");

                    $(this).find("input[type='button']").removeClass("form-control-warning");
                    $(this).find("input[type='button']").removeClass("input-validation-error");
                    $(this).find("input[type='button']").removeClass("form-control-success");

                    $(this).find("button").removeClass("form-control-warning");
                    $(this).find("button").removeClass("input-validation-error");
                    $(this).find("button").removeClass("form-control-success");

                }
                else {
                    $(this).find("input[type='submit']").removeClass("form-control-warning");
                    $(this).find("input[type='submit']").removeClass("input-validation-error");
                    $(this).find("input[type='submit']").removeClass("form-control-success");

                    $(this).find("input[type='button']").removeClass("form-control-warning");
                    $(this).find("input[type='button']").removeClass("input-validation-error");
                    $(this).find("input[type='button']").removeClass("form-control-success");

                    $(this).find("button").removeClass("form-control-warning");
                    $(this).find("button").removeClass("input-validation-error");
                    $(this).find("button").removeClass("form-control-success");

                    $(this).addClass("has-success");
                    $(this).find("input,textarea,select").addClass("form-control-success");
                }
            }

        });
    });
});

var page = function () {
    //Update the validator
    $.validator.setDefaults({
        highlight: function (element) {
            addValidationError(element.id);
        },
        unhighlight: function (element) {
            removeValidationError(element.id);
        }
    });
}();

function clearValidationSummaryErrors() {
    $(".validation-summary-errors").hide();
}

function addValidationWarning(id) {
    try {
        $("#" + id).closest("input,textarea,select").removeClass("input-validation-error");
        $("#" + id).closest("input,textarea,select").removeClass("form-control-success");
        $("#" + id).closest(".form-group").removeClass("has-danger");
        $("#" + id).closest(".form-group").removeClass("has-success");
        $("#" + id).closest("input,textarea,select").removeClass("field-error");
        $("#" + id).closest("input,textarea,select").removeClass("field-success");

        $("#" + id).closest(".form-group").addClass("has-warning");
        $("#" + id).closest("input,textarea,select").addClass("form-control-warning");
        $("#" + id).closest("input,textarea,select").addClass("field-warning");

        $("[data-valmsg-for='" + id + "']").html("");
        $("[data-valmsg-for='" + id + "']").removeClass("field-validation-error");

    } catch (e) {

    }
}

function addValidationError(id) {
    try {
        if (id == "") {
            return;
        }
        $("#" + id).closest("input,textarea,select").removeClass("form-control-warning");
        $("#" + id).closest("input,textarea,select").removeClass("form-control-success");
        $("#" + id).closest(".form-group").removeClass("has-warning");
        $("#" + id).closest(".form-group").removeClass("has-success");
        $("#" + id).closest("input,textarea,select").removeClass("field-warning");
        $("#" + id).closest("input,textarea,select").removeClass("field-success");

        $("#" + id).closest(".form-group").addClass("has-danger");
        $("#" + id).closest("input,textarea,select").addClass("input-validation-error");
        $("#" + id).closest("input,textarea,select").addClass("field-error");

        $("[data-valmsg-for='" + id + "']").html($("#" + id).attr("data-val-required"));
        $("[data-valmsg-for='" + id + "']").addClass("field-validation-error");

    } catch (e) {
    }
}

function addValidationErrorMessage(id, error) {
    try {
        if (id == "") {
            return;
        }
        $("#" + id).closest("input,textarea,select").removeClass("form-control-warning");
        $("#" + id).closest("input,textarea,select").removeClass("form-control-success");
        $("#" + id).closest(".form-group").removeClass("has-warning");
        $("#" + id).closest(".form-group").removeClass("has-success");
        $("#" + id).closest("input,textarea,select").removeClass("field-warning");
        $("#" + id).closest("input,textarea,select").removeClass("field-success");

        $("#" + id).closest(".form-group").addClass("has-danger");
        $("#" + id).closest("input,textarea,select").addClass("input-validation-error");
        $("#" + id).closest("input,textarea,select").addClass("field-error");

        $("[data-valmsg-for='" + id + "']").html(error);
        $("[data-valmsg-for='" + id + "']").addClass("field-validation-error");

    } catch (e) {
    }
}

function removeValidationError(id) {
    try {
        $("#" + id).closest("input,textarea,select").removeClass("input-validation-error");
        $("#" + id).closest("input,textarea,select").removeClass("form-control-warning");
        $("#" + id).closest(".form-group").removeClass("has-danger");
        $("#" + id).closest(".form-group").removeClass("has-warning");
        $("#" + id).closest("input,textarea,select").removeClass("field-warning");
        $("#" + id).closest("input,textarea,select").removeClass("field-error");

        $("#" + id).closest("input,textarea,select").addClass("form-control-success");
        $("#" + id).closest("input,textarea,select").addClass("field-success");
        $("#" + id).closest(".form-group").addClass("has-success");

        $("[data-valmsg-for='" + id + "']").html("");
        $("[data-valmsg-for='" + id + "']").removeClass("field-validation-error");
    } catch (e) {

    }
}

function validateInputField(id) {
    if (requiredElement(id) && $("#" + id).val() == "") {
        addValidationWarning(id);
    }
    else {
        removeValidationError(id);
    }
}

function requiredElement(id) {
    if ($("#" + id).data("val") != undefined &&
        $("#" + id).data("val-required") != undefined &&
        //$("#" + id).data("val-email") != undefined &&
        $("#" + id).data("val") == true) {
        return true;
    }

    return false;
}

function hashFieldErrorClass(id) {
    if ($("#" + id).length > 0 && $("#" + id).attr("disabled") == undefined) {
        return $("#" + id).hasClass("field-error");
    }
}

function setWarningValidation() {
    $("form").find('.form-group').each(function () {

        var _id = $(this).find("input,textarea,select").attr("id");
        $("[data-valmsg-for='" + _id + "']").html("");
        $("[data-valmsg-for='" + _id + "']").removeClass("field-validation-error");

        if (!requiredElement(_id)) {
            removeValidationError(_id);
            return;
        }

        if ($(this).find("input,textarea,select").prop("disabled") != true) {

            if (requiredElement(_id)) {
                var label = $("label[for='" + _id + "']").html();

                if (label != undefined) {
                    if (label.indexOf("*") == -1)
                        $("label[for='" + _id + "']").append("<span class='text-danger'> *</span>");
                }
            }

            if ($("#" + _id).val() != "") {
                removeValidationError(_id);
                return;
            }

            if (requiredElement(_id) && $("#" + _id).val() == "") {

                addValidationWarning(_id);

                $(this).find("input[type='submit']").removeClass("form-control-warning");
                $(this).find("input[type='submit']").removeClass("input-validation-error");
                $(this).find("input[type='submit']").removeClass("form-control-success");

                $(this).find("input[type='button']").removeClass("form-control-warning");
                $(this).find("input[type='button']").removeClass("input-validation-error");
                $(this).find("input[type='button']").removeClass("form-control-success");

                $(this).find("button").removeClass("form-control-warning");
                $(this).find("button").removeClass("input-validation-error");
                $(this).find("button").removeClass("form-control-success");

            }
            else {
                $(this).find("input[type='submit']").removeClass("form-control-warning");
                $(this).find("input[type='submit']").removeClass("input-validation-error");
                $(this).find("input[type='submit']").removeClass("form-control-success");

                $(this).find("input[type='button']").removeClass("form-control-warning");
                $(this).find("input[type='button']").removeClass("input-validation-error");
                $(this).find("input[type='button']").removeClass("form-control-success");

                $(this).find("button").removeClass("form-control-warning");
                $(this).find("button").removeClass("input-validation-error");
                $(this).find("button").removeClass("form-control-success");

                $(this).addClass("has-success");
                $(this).find("input,textarea,select").addClass("form-control-success");
            }
        }
    });
}