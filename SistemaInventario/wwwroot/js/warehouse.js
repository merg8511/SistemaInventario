let dataTable;

$(document).ready(function () {
    loadDataTable();
}); //fin de funcion

function loadDataTable() {
    dataTable = $("#tbl-data").DataTable({
        "ajax": {
            "url": "/Admin/Warehouse/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "description", "width": "40%" },
            {
                "data": "whState",
                "render": function (data) {
                    if (data == true) {
                        return '<span class="badge bg-success">Activo</span>';
                    }
                    else {
                        return '<span class="badge bg-secondary">Inactivo</span>';
                    }
                },
                "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Warehouse/Upsert/${data}" class="btn btn-info text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>

                             <a onclick=Delete("/Admin/Warehouse/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                <i class="bi bi-trash3-fill"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "20%"
            }
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

function Delete(url) {
    swal({
        title: "¿Esta seguro de eliminar la bodega?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((del) => {
        if (del) {
            $("#tbl-data").LoadingOverlay("show");
            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    $("#tbl-data").LoadingOverlay("hide");
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        $("#tbl-data").LoadingOverlay("hide");
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}