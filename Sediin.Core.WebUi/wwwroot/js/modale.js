var _modalArray = new Array();

/////////////////////////////////////////////////////////////////////////////////////
// Helper ///////////////////////////////////////////////////////////////////////////
// no usare direttamente ////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////

function showModalFullscreenOverModal(title, html, id) {
    var _id = "modalOverModal_" + id;
    var _modaleHtml = "";

    _modaleHtml += '<div role="dialog" data-bs-focus="false" class="modal fade modal-fullscreen" id="' + _id + '" tabindex="-1" data-bs-keyboard="false" data-bs-backdrop="static" style="" aria-modal="true">';
    _modaleHtml += '<div class="modal-dialog">';
    _modaleHtml += '<div class="modal-content">';
    _modaleHtml += '<div class="modal-header">';
    _modaleHtml += '<h4 class="modal-title" id="' + _id + 'Header">';
    _modaleHtml += title;
    _modaleHtml += '</h4>';
    _modaleHtml += '<a href="javascript:void(0)" style="margin-left:auto" aria-hidden="true" onclick="closeModalOverModal(\'' + _id + '\')"><i class="close fas fa-window-close text-danger" style="opacity:1 !important; font-size:40px"></i></a>';
    _modaleHtml += '</div>';
    _modaleHtml += '<div id="' + _id + 'Body" class="modal-body" style="overflow:hidden; min-height:340px">';
    _modaleHtml += html
    _modaleHtml += '</div>';
    _modaleHtml += '<div class="modal-footer">';
    _modaleHtml += '<button type="button" class="btn btn-danger" onclick="closeModalOverModal(\'' + _id + '\')">';
    _modaleHtml += 'Chiudi</button>';
    _modaleHtml += '</div>';
    _modaleHtml += '<div style="height:15px"></div>';
    _modaleHtml += '</div>';
    _modaleHtml += '</div>';
    _modaleHtml += '</div>';

    $("body").append(_modaleHtml);
    $("#" + _id).modal("show");
}

function closeModalOverModal(id) {
    //var _id = "modalOverModal_" + id;
    $("#" + id).remove();

    //    $("#modalOverModal").removeAttr("style");
    //    $("#modalOverModal").removeClass("show");
}

function showModalNoFooter_FullScreen(title, html) {

    dynamicModal_CreateModal(modalGenerateUUID(), "modal-fullscreen", title, html, true, false, true);
    //enableAllBtn();
}

function showModalNoHeaderFooter_FullScreen(html) {
    showDynamicModal_FullScreen_NoHeaderFooter(modalGenerateUUID(), "Informazioni", html);
    //enableAllBtn();
}

function showModalNoHeader_FullScreen(html) {
    showDynamicModal_FullScreen(modalGenerateUUID(), "Informazioni", html, false, true);
    //enableAllBtn();
}

function showModal(title, html) {
    showDynamicModal(modalGenerateUUID(), title, html, true, true);
    //enableAllBtn();
}

function showModal_NoHeaderFooter(html) {
    showDynamicModal(modalGenerateUUID(), "", html, false, false);
    //enableAllBtn();
}

function showModal_NoHeader(html) {
    showDynamicModal(modalGenerateUUID(), "", html, false, true);
    //enableAllBtn();
}

function showModal_NoFooter(title, html) {
    showDynamicModal(modalGenerateUUID(), title, html, true, false);
    //enableAllBtn();
}

function hideModal() {
    //enableAllBtn();
    var _id = getModalGenerateUUID();
    dynamicModal_RemovewModal(_id);
    $("#" + _id).remove();
    $("body").attr("style", "");
    $('.blurEffect').removeClass('blur');
    $("#waidModalshowWaidModal").remove();
    alertClose();

}

function showModalFullScreen(title, html, showHeader, showFooter, showDismissButton) {
    showHeader = showHeader == undefined ? true : showHeader;
    showFooter = showFooter == undefined ? true : showFooter;
    showDismissButton = showDismissButton == undefined ? true : showDismissButton;

    dynamicModal_CreateModal(modalGenerateUUID(), "modal-fullscreen", title, html, showHeader, showFooter, showDismissButton);
    //enableAllBtn();
}

function showModal_IsOpen() {
    if ($("[role='dialog']").hasClass("modal")) {
        return true;
    }
    return false;
}


/////////////////////////////////////////////////////////////////////////////////////
// Helper ///////////////////////////////////////////////////////////////////////////
// no usare direttamente ////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////

function dynamicModal_PushNewModal(id, modalHtml) {

    try {
        if (id == "dynamicModal_waid")
            return;

        $.each(_modalArray, function (index, item) {
            $("#" + item.id).modal("hide");
            if (item.id == id) {
                _modalArray.remove()
            }
        })

        for (var i = 0; i < _modalArray.length; i++) {
            $("#" + _modalArray[i][0]).modal("hide");
        }
        _modalArray.push({ id: id, html: modalHtml });

    } catch (e) {
        //alert("dynamicModal_PushNewModal: " + e.description);
    }
}

function dynamicModal_RemovewModal(id) {

    try {

        // 
        // setTimeout(function () {

        $("body").removeClass("modal-open");
        //$('body').removeClass('blur');

        $('.blurEffect').removeClass('blur');


        $(".modal").removeClass("fade").removeClass("show");
        $(".modal").addClass("fade").addClass("hide");

        $(".modal").remove();

        $(".modal-backdrop").removeClass("fade").removeClass("show");
        $(".modal-backdrop").addClass("fade").addClass("hide");

        $(".modal-backdrop").remove();

        $("body").attr("style", "padding-right: 0px !important;");

        //$(".modal-backdrop").each(function (e) {
        //    $(this).remove();
        //});
        // }, 250);

        return;

        //TODO
        //setTimeout(function () {
        //    $("body").removeAttr("class");
        //    $("body").attr("style", "");
        //}, 10);


        var _modalArray_new = Array();

        for (var i = 0; i < _modalArray.length; i++) {
            if (_modalArray[i][0] != id) {
                _modalArray_new.push([_modalArray[i][0], _modalArray[i][1]]);
            }
        }

        if (_modalArray_new.length > 0) {

            setTimeout(function () {
                $("#dynamicModal_waid").modal("hide");
                $("#dynamicModal_waid").remove();

                // $(".blurEffect").removeClass("blur");

                $("#" + id).hide();
                $("#" + id).remove();


                $(".modal-backdrop").each(function (e) {
                    $(this).remove();
                });


                var _lastModal = _modalArray_new[_modalArray_new.length - 1];
                if (_lastModal != undefined) {

                    $("#" + _lastModal[0]).modal("show");
                    $("body").addClass("modal-open");
                    $("body").attr("style", "overflow:hidden; padding-right:0");
                    $(".blurEffect").addClass("blur");
                }

            }, 100);

        }
        else {

            setTimeout(function () {
                $("#dynamicModal_waid").modal("hide");
                $("#dynamicModal_waid").remove();

                // $(".blurEffect").removeClass("blur");

                $("#" + id).hide();
                $("#" + id).remove();

                $(".modal-backdrop").each(function (e) {
                    $(this).remove();
                });

                $("body").removeClass("modal-open");
                $("body").attr("style", "");
                $(".blurEffect").removeClass("blur");
            }, 100);
        }

        _modalArray = new Array();

        _modalArray = _modalArray_new;

    } catch (e) {
        //alert(e.message);
    }
}

function dynamicModal_CreateModal(id, classname, title, html, showHeader, showFooter, showDismissButton) {
    try {
        showHeader = showHeader == undefined ? false : showHeader;
        showFooter = showFooter == undefined ? false : showFooter;
        showDismissButton = showDismissButton == undefined ? true : showDismissButton;

        var _lastid = getModalGenerateUUID();

        if (_lastid != undefined) {
            var _lastHtml = $("#" + _lastid + "-body").html();
            dynamicModal_PushNewModal(_lastid, _lastHtml);
        }

        $("[data-toggle='tooltip']").tooltip("hide");

        //  alert(_modalArray.length)
        //alert("id: " + id);

        //$("#dynamicModal_waid").modal("hide");
        //$("#dynamicModal_waid").remove();

        //$(".modal-backdrop").each(function (e) {
        //    //$(this).removeClass("fade in");
        //    //$(this).addClass("fade out");
        //    $(this).remove();
        //});

        var _id = id;
        var _modaleHtml = "<div role=\"dialog\" data-bs-focus=\"false\" class=\"modal fade\" id=\"" + _id + "\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"" + id + "-label\" aria-hidden=\"true\" data-bs-keyboard=\"false\" data-bs-backdrop=\"static\">"; //data-bs-focus=\"false\"
        _modaleHtml += "<div class=\"modal-dialog\">";
        _modaleHtml += "<div class=\"modal-content\">";

        if (showHeader == undefined || showHeader) {
            _modaleHtml += "<div class=\"modal-header\">";
            _modaleHtml += "<h4 class=\"modal-title\" id=\"" + _id + "-label\">" + title + "</h4>";

            if (showDismissButton == true) {
                _modaleHtml += "<a href=\"javascript:void(0)\" style=\"margin-left:auto\" data-dismiss=\"modal\" aria-hidden=\"true\" onclick=\"dynamicModal_RemovewModal('" + _id + "')\"><i class=\"close fas fa-window-close text-danger\" style=\"opacity:1 !important; font-size:40px\"></i></a>";
            }
            else {
                _modaleHtml += "<button type=\"button\" class=\"btn-close\" data-dismiss=\"modal\" aria-hidden=\"true\" onclick=\"dynamicModal_RemovewModal('" + _id + "')\"></button>";
            }

            _modaleHtml += "</div>";
        }
        else {
            if (showDismissButton) {
                _modaleHtml += "<div class=\"/*modal-header*/\">";
                _modaleHtml += "<a href=\"javascript:void(0)\" data-dismiss=\"modal\" aria-hidden=\"true\" onclick=\"dynamicModal_RemovewModal('" + _id + "')\"><i class=\"close fas fa-window-close text-danger\" style=\"opacity:1 !important; font-size:40px\"></i></a>";
                _modaleHtml += "</div>";
            }
        }

        _modaleHtml += "<div id=\"" + _id + "-body\" class=\"modal-body\" style=\"overflow:hidden\">";

        if (classname == "" || classname == undefined)
            _modaleHtml += "<p>" + html + "</p>";
        else
            _modaleHtml += html;

        //_modaleHtml += html;
        _modaleHtml += "</div>";

        if (showFooter == undefined || showFooter) {
            _modaleHtml += "<div class=\"modal-footer\">";
            _modaleHtml += "<button type=\"button\" class=\"btn btn-danger\" data-dismiss=\"modal\" style=\"margin-bottom:20px\" onclick=\"dynamicModal_RemovewModal('" + _id + "')\" id=\"closeButton_" + id + "\">Chiudi</button>";
            _modaleHtml += "<span id=\"" + _id + "_ModalButtonDelete\" style=\"margin-left:10px\"></span>";
            //_modaleHtml += "<a class=\"btn\" role=\"button\" onclick=\"showDynamicModal('myModal_7', 'm','00', true, true)\" href=\"#myModal_7\" data-toggle=\"modal\">Launch other modal</a>";
            _modaleHtml += "</div>";
        }
        else {
            _modaleHtml += "<div style=\"height:15px\"></div>";
        }

        _modaleHtml += "</div>";
        _modaleHtml += "</div>";
        _modaleHtml += "</div>";

        ////nascondi quelli visibili prima di aggiungere uno nuovo
        //for (var i = 0; i < _modalArray.length; i++) {
        //    $("#" + _modalArray[i][0]).modal("hide");
        //}

        ////nascondi waid modale
        //$("#dynamicModal_waid").modal("hide");

        $("#waidModalshowWaidModal").remove();

        $("body").append(_modaleHtml);

        $("#" + _id).addClass(classname);

        //dynamicModal_PushNewModal(_id, html);

        $("#" + _id).modal("show");

        $('.blurEffect').addClass('blur');
        //$('body').addClass('blur');
        //$('footer').addClass('blur');
        $("body").attr("style", "padding-right: 0px !important;");

        //setTimeout(function () {
        //    //$('.blurEffect').addClass('blur');
        //    //$('footer').addClass('blur');
        //    $("#" + _id).modal("show");
        //    $("body").attr("style", "overflow:hidden");
        //    $("body").addClass("modal-open");
        //}, 250);

    } catch (e) {

        //alert(e.message);
    }
}

//basic functions
function dynamicModal_GetCloseButtonID() {
    return "#closeButton_" + getModalGenerateUUID();
}

function showDynamicModal_FullScreenNoScroll(id, title, html, showHeader, showFooter) {
    dynamicModal_CreateModal(id, "modal-fullscreen-noscroll", title, html, showHeader, showFooter, true);
}

function showModalFullScreen_NoHeaderFooterNoScroll(html) {
    showDynamicModal_FullScreenNoScroll(modalGenerateUUID(), "Informazioni", html, false, false);
    //enableAllBtn();
}

function showDynamicModal_FullScreen_NoHeaderFooter(id, title, html) {
    dynamicModal_CreateModal(id, "modal-fullscreen", title, html, false, false, true);
}

function showDynamicModal(id, title, html, showHeader, showFooter, showDismissButton) {
    dynamicModal_CreateModal(id, "", title, html, showHeader, showFooter, showDismissButton);
}

function showDynamicModal_NoHeaderFooter(id, title, html) {
    dynamicModal_CreateModal(id, "", title, html, false, false);
}

function showDynamicModal_FullScreen(id, title, html, showHeader, showFooter) {
    dynamicModal_CreateModal(id, "modal-fullscreen", title, html, showHeader, showFooter, true);
}

function getModalGenerateUUID() {
    return $("[role='dialog']").attr("id");
}

function modalGenerateUUID() {
    var d = new Date().getTime();
    if (window.performance && typeof window.performance.now === "function") {
        d += performance.now(); //use high-precision timer if available
    }
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
}

