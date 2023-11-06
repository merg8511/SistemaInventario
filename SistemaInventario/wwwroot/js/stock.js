let dataTable;

$(document).ready(function () {
    $("#tbl-data").LoadingOverlay("show");
    loadDataTable();
    $("#tbl-data").LoadingOverlay("hide");

}); //fin de funcion

function loadDataTable() {
    dataTable = $("#tbl-data").DataTable({
        "ajax": {
            "url": "/Inventory/Stock/GetAll"
        },
        "columns": [
            { "data": "warehouse.name" },
            { "data": "product.description" },
            {
                "data": "product.cost", "className": "text-end",
                "render": function (data) {
                    var d = data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                    return d;
                }
            },
            { "data": "quantity", "className":"text-end" },
        ],
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros Por Pagina",
            "zeroRecords": "Ningun Registro",
            "info": "Mostrar pagina _PAGE_ de _PAGES_",
            "infoEmpty": "no hay registros",
            "infoFiltered": "(filtered from _MAX_ total registros)",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        }
    });
}
