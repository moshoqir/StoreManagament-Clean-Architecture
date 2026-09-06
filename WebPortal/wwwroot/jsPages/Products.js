function getData() {
	ajaxConnection("/Products/GetData", "GET", null, null, "product", "productData", "categoryId");


}

function getById(id) {
	ajaxConnection(`/Products/GetById/${id}`, "get", null, null, null, null, null);


}

function addData() {
	var productData = {
		ProductName: $("#ProductName").val().trim(),
		ProductPrice: $("#ProductPrice").val(),
		CategoryId: $("#CategoryId").val()
	}

	ajaxConnection("/Products/Create", "post", productData, function () {
		ClearForm("productForm", "ProductId", "productData");
		getData()
	}, null, null, null);

}

function updateData(id) {
	var productData = {
		ProductName: $("#ProductName").val().trim(),
		ProductPrice: $("#ProductPrice").val(),
		CategoryId: $("#CategoryId").val()
	}

	ajaxConnection(`/Products/Update/${id}`, "post", productData, function () {
		$("#submitProductData").text("Save Product").removeClass("btn-warning").addClass("btn-primary");
		ClearForm("productForm", "ProductId", "productData");
		getData();

	}, null, null, null);
}

function deleteData(id) {
	ajaxConnection(`/Products/Delete/${id}`, "post", null, function () {
		ClearForm("productForm", "ProductId", "productData");
		getData();
	}, null, null, null);
}



// Implementation:
function getCategories() {
	$.ajax({
		url: "/Categories/GetData",
		method: "Get",
		contentType: "application/json",
		data: "",
		success: function (response) {


			var menu = $("#CategoryId");

			response.data.map(function (item) {
				return menu.append(`<option value='${item.categoryId}'>${item.categoryName}</option>`)
			})

		}
	})
}
$(function () {
	getData();

	getCategories();

	$(document).on("click", ".infoBtn", function (e) {

		var row = $(this).closest("tr");

		var data = row.data("row")
		var id = data.productId;
	 


		getById(id);


	})

	$(document).on("click", ".editBtn", function () {
		$("#submitProductData").text("Update Product").removeClass("btn-primary").addClass("btn-warning");
		var data = $(this).closest("tr").data("row");

		// we must here call id also (because we're not mapping all fields here, becuase of full name)
		var data = $(this).closest("tr").data("row");

		var id = data.productId;
		var productPrice = data.productPrice;
		var categoryId = data.categoryId;

		$("#ProductId").val(id);

		$("#ProductName").val(data.productName);
		$("#ProductPrice").val(productPrice);
		$("#CategoryId").val(categoryId);




	})

	$("#cancelProductData").click(function () {
		$("#submitProductData").text("Save Product").removeClass("btn-warning").addClass("btn-primary");
		$("#productForm")[0].reset();
		$("#ProductId").val("0");



	})

	$(document).on("click", ".deleteBtn", function () {
		var row = $(this).closest("tr");

		data = row.data("row");

		var id = data.productId;
		var name = data.productName;
		console.log(id)
		console.log(JSON.stringify(data))
		if (confirm(`Are you sure you want to delete ${name}?`)) {
			deleteData(id);
		}
	})

	$("#productForm").submit(function (e) {
		e.preventDefault();

		let isValid = true;

		let productName = $("#ProductName").val();
		let productPrice = $("#ProductPrice").val();
		let categoryId = $("#CategoryId").val();


		$(".error-span").text("").css("color", "transparent");

		if (productName.trim() === "") {
			isValid = false;

			$("#ProductNameSpan").text("Product Name is required").css("color", "red");

		}

		if (productPrice === "" || productPrice <= 0) {
			isValid = false;

			$("#ProductPriceSpan").text("Product Price must be greater than 0").css("color", "red");
		}


		if (!categoryId) {
			isValid = false;

			$("#CategoryIdSpan").text("Category Id is required").css("color", "red");
		}




		var id = $("#ProductId").val();


		if (isValid) {

			if (id !== "0") {
				updateData(id);
			}
			else
				addData();
		}
	})
})