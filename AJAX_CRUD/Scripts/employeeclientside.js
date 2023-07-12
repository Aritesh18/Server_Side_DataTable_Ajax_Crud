$(document).ready(function () {
    $(".edit-link").click(function (e) {
        e.preventDefault();
        var url = $(this).data("url");
        var modalContent = $("#myModal .modal-content");
        $.ajax({
            url: url,
            type: "GET",
            success: function (data) {
                modalContent.html(data);
                $("#myModal").addClass("show");
            },
            error: function (error) {
                console.error("An error occurred while loading the content. Error message: " + error.statusText);
            }
        });
    });

    $(".delete-link").click(function (e) {
        e.preventDefault();
        var url = $(this).data("url");
        var row = $(this).closest("tr");
        Delete(url, row);
    });

    function Delete(url, row) {
        var ans = confirm("Are you sure you want to delete this Record?");
        if (ans) {
            $.ajax({
                url: url,
                type: "POST",
                data: JSON.stringify({
                    id: url.split("/").pop(),
                    "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                }),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {
                        alert("Employee deleted successfully.");
                        row.remove();
                    } else {
                        alert("Failed to delete employee.");
                    }
                },
                error: function (error) {
                    console.error("An error occurred while deleting the employee. Error message: " + error.statusText);
                }
            });
        }
    }

    $(".create-link").click(function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        var modalContent = $("#myModal .modal-content");
        $.ajax({
            url: url,
            type: "GET",
            success: function (data) {
                modalContent.html(data);
                $("#myModal").addClass("show");
            },
            error: function (error) {
                console.error("An error occurred while loading the content. Error message: " + error.statusText);
            }
        });
    });
});
