function getData() {
    ajaxConnection("/Categories/GetData", "GET", null, null, "category", "categoryData", "Products");

     
}

function getById(id) {
    ajaxConnection(`/Categories/GetById/${id}`, "get", null, null, null, null, null);

    
}

function addData() {
    var categoryData = {
        CategoryName: $("#CategoryName").val().trim(),
    }

    ajaxConnection("/Categories/Create", "post", categoryData, function () {
         ClearForm("categoryForm", "CategoryId", "categoryData");
        getData()  
    } , null, null, null);
   
}

function updateData(id) {
    var categoryData = {
        CategoryId: parseInt(id),
        CategoryName: $("#CategoryName").val().trim()
    }

    ajaxConnection(`/Categories/Update/${id}`, "post", categoryData, function () {
        $("#submitCategoryData").text("Save Category").removeClass("btn-warning").addClass("btn-primary");
        ClearForm("categoryForm", "CategoryId", "categoryData");
        getData();
       
    }, null, null, null);
}

function deleteData(id) {
	ajaxConnection(`/Categories/Delete/${id}`, "post", null, function () {
		ClearForm("categoryForm", "CategoryId", "categoryData");
		getData();
	}, null, null, null);
}



// Implementation:

$(function () {
	getData();

	$(document).on("click", ".infoBtn", function (e) {

		var row = $(this).closest("tr");

		var data = row.data("row")
		var id = data.categoryId;
		var name = data.categoryName;


		getById(id);


	})

	$(document).on("click", ".editBtn", function () {
		$("#submitCategoryData").text("Update Category").removeClass("btn-primary").addClass("btn-warning");
		var data = $(this).closest("tr").data("row");

		// we must here call id also (because we're not mapping all fields here, becuase of full name)
		var data = $(this).closest("tr").data("row");

		var id = data.categoryId;
		$("#CategoryId").val(id);

		$("#CategoryName").val(data.categoryName);




	})

	$("#cancelCategoryData").click(function () {
		$("#submitCategoryData").text("Save Category").removeClass("btn-warning").addClass("btn-primary");
		$("#categoryForm")[0].reset();
		$("#CategoryId").val("0");



	})

	$(document).on("click", ".deleteBtn", function () {
		var row = $(this).closest("tr");
		
		data = row.data("row");

		var id = data.categoryId;
        var name = data.categoryName;
		console.log(id)
		console.log(JSON.stringify(data))
		if (confirm(`Are you sure you want to delete ${name}?`)) {
			deleteData(id);
		}
	})

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
				updateData(id);
			}
			else
				addData();
		}
	})
})