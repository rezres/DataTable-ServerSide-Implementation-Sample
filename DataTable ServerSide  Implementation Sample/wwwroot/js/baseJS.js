var myDatatableAPI = null;

function Load(url) {
    showLoader();
    $('#myModalTitle').html("");

    $.get(url).done(function (response) {
        $('#myModalBody').html(response);
        hideLoader();
        $('#myModal').modal('show');

    });
}

function AjaxSubmit(form) {
    showLoader();
    var url = document.getElementById(form).getAttribute('action');
    var data = serializeFormData(form);
    $.post({
        url: url, data: data,
        success: function (response) {
            $('#myModalBody').html(response);
            hideLoader();
            if (typeof myDatatableAPI !== 'null') {
                myDatatableAPI.ajax.reload();
            }
        },
        error: function (exception) { hideLoader(); errorModal(exception.Msg); }
    });
}

function serializeFormData(form) {
    var inputs = document.getElementById(form).elements;
    var data = {};
    Array.from(inputs).forEach(function (val) {
        data[val.name] = val.value;
    });
    return data;
}

function showLoader() {
    document.getElementById('loaderSpan').removeAttribute('hidden');
}

function hideLoader() {
    document.getElementById('loaderSpan').setAttribute('hidden', "");
}

function successModal(body) {
    $('#myModalTitle').html("<i class='largeGlyph glyphicon glyphicon-check text-success'></i>");
    $('#myModalBody').html(body);
    $('#myModal').modal('show');
}

function errorModal(body) {
    $('#myModalTitle').html('<i class="largeGlyph glyphicon glyphicon-exclamation-sign text-danger"></i>');
    $('#myModalBody').html(body);
    $('#myModal').modal('show');
}