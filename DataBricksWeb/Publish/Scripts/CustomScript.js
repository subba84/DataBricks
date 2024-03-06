function fnIsNullorEmpty(value) {
    if (value == null || value.trim() == "") {
        return true;
    }
    return false;
};
function Warning(msg) {
    $("#msgWarning").html(msg);
    $("#divWarning").modal('show');
};
function Alert(msg) {
    $("#msgAlert").html(msg);
    $("#divAlert").modal('show');
};
function Confirmation(msg) {
    $("#msgConfirm").html(msg);
    $("#divConfirm").modal('show');
};