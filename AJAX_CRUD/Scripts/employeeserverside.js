var datatable;

$(document).ready(function () {
    datatable = $('#employeeTable').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/EmployeeServerSide/GetEmployees",
            "type": "POST",
            "dataType": "json",
            "contentType": "application/json",
            "data": function (data) {
                data.draw = data.draw || 1; 
                data.start = data.start || 0;
                data.length = data.length || 10; 
                data.sortCol = data.order[0].column || 0;
                data.sortDir = data.order[0].dir || "asc";
                data.search = data.search.value || ""; 
                return JSON.stringify(data);
            }
        },
        "columns": [
            { "title": 'EmployeeID', "data": 'EmployeeID' },
            { "title": 'Name', "data": 'FirstName' },
            { "title": 'Age', "data": 'Age' },
            { "title": 'State', "data": 'State' },
            { "title": 'Country', "data": 'Country' },
            {
                "title": 'Actions',
                "data": null,
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/EmployeeServerSide/Edit/${data.EmployeeID}" class="btn btn-success m-2">
                                <i class="fas fa-edit">Edit</i>
                            </a>
                            <button class="btn btn-danger" onclick="DeleteEmployee(${data.EmployeeID})">
                                <i class="fas fa-trash-alt">Delete</i>
                            </button>
                        </div>
                    `;
                }
            }
        ]
    });
});

function DeleteEmployee(employeeID) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `/EmployeeServerSide/DeleteEmployee`,
                type: 'POST',
                data: { employeeID: employeeID },
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: 'Deleted!',
                            text: 'Data deleted successfully.',
                            icon: 'success'
                        });
                        datatable.ajax.reload();
                    } else {
                        Swal.fire({
                            title: 'Cancelled',
                            text: 'Error while deleting data from the database.',
                            icon: 'error'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: 'Error',
                        text: 'An error occurred while processing the request.',
                        icon: 'error'
                    });
                }
            });
        }
    });
}

