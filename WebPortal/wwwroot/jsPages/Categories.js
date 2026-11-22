function getData() {
    ajaxConnection("/Categories/GetData", "GET", null, null, "category", "categoryData", "Products");
}

function getById(id) {
    ajaxConnection(`/Categories/GetById/${id}`, "GET", null, null, null, null, null);
}

function addData() {
    var categoryData = {
        CategoryName: $("#CategoryName").val().trim()
    }

    ajaxConnection("/Categories/Create", "POST", categoryData, function () {
        ClearForm("categoryForm", "CategoryId", "categoryData");
        getData();
    }, null, null, null);
}

function updateData(id) {
    var categoryData = {
        CategoryId: parseInt(id),
        CategoryName: $("#CategoryName").val().trim()
    }

    ajaxConnection(`/Categories/Update/${id}`, "POST", categoryData, function () {
        $("#submitCategoryData").text("Save Category").removeClass("btn-warning").addClass("btn-primary");
        ClearForm("categoryForm", "CategoryId", "categoryData");
        getData();
    }, null, null, null);
}

function deleteData(id) {
    ajaxConnection(`/Categories/Delete/${id}`, "POST", null, function () {
        ClearForm("categoryForm", "CategoryId", "categoryData");
        getData();
    }, null, null, null);
}

// Implementation:
$(function () {
    // THE FIX: Guarantee permissions are loaded into memory BEFORE drawing the table!
    LoadCurrentPermissions(function () {
        getData();
    });

    $(document).on("click", ".infoBtn", function (e) {
        var row = $(this).closest("tr");
        var data = row.data("row");
        getById(data.categoryId);
    });

    $(document).on("click", ".editBtn", function () {
        $("#submitCategoryData").text("Update Category").removeClass("btn-primary").addClass("btn-warning");
        var data = $(this).closest("tr").data("row");

        $("#CategoryId").val(data.categoryId);
        $("#CategoryName").val(data.categoryName);
    });

    $("#cancelCategoryData").click(function () {
        $("#submitCategoryData").text("Save Category").removeClass("btn-warning").addClass("btn-primary");
        $("#categoryForm")[0].reset();
        $("#CategoryId").val("0");
    });

    $(document).on("click", ".deleteBtn", function () {
        var row = $(this).closest("tr");
        var data = row.data("row");

        if (confirm(`Are you sure you want to delete ${data.categoryName}?`)) {
            deleteData(data.categoryId);
        }
    });

    $("#categoryForm").submit(function (e) {
        e.preventDefault();
        let isValid = true;
        let categoryName = $("#CategoryName").val();

        $(".error-span").text("").css("color", "transparent");

        if (categoryName.trim() === "") {
            isValid = false;
            $("#CategoryNameSpan").text("Category Name is required").css("color", "red");
        }

        var id = $("#CategoryId").val();

        if (isValid) {
            if (id !== "0") {
                // Front-end guard
                if (!HasPermission('Category.Update')) {
                    ShowMessage('You do not have permission to update categories.', 'danger');
                    return;
                }
                updateData(id);
            } else {
                // Front-end guard
                if (!HasPermission('Category.Create')) {
                    ShowMessage('You do not have permission to create categories.', 'danger');
                    return;
                }
                addData();
            }
        }
    });
});