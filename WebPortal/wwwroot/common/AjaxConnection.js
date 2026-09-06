function ajaxConnection(url, method, data, callback,tableName, divName, hideColumn ) {

    if (method.toUpperCase() === "GET") {
        if (url.includes("ById")) {
            $.ajax({
                url: url,
                method: method,
                contentType: "application/json",
                data: data,
                success: function (response) {
                    openModal(response);
                },
                error: function (xhr, status, error) {
                    console.error("AJAX Error:", status, error);
                }
            });
        }
         
        else {
            $.ajax({
                url: url,
                method: method,
                contentType: "application/json",
                data: data,
                success: function (response) {
                    // Safety check: If database is completely empty, don't crash CreateTable!
                    if (!response || !response.data || response.data.length === 0) {
                        $(`#${divName}`).html("<div class='alert alert-info'>No records found.</div>");
                        return;
                    }

                    var table = createTable(tableName, response, hideColumn);
                    $(`#${divName}`).html(table);
                    new DataTable(`#${tableName}Table`);
                },
                error: function (xhr, status, error) {
                    console.error("AJAX Error:", status, error);
                }
            });
        }
    }

    if (method.toUpperCase() === "POST") {

        //if (url.includes("update") || url.includes("edit")) {
        //    $.ajax({
        //        url: url,
        //        method: method,
        //        contentType: "application/json",
        //        data: JSON.stringify(data),
        //    })
        //}

        $.ajax({
            url: url,
            method: method,
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (response) {
                if (response.status) {
                    alert(response.message)

                    if (callback) {
                        callback(response);
                    }
                }
                else {
                    alert("Failed: " + response.message);
                }
            },

            error: function (response) {
                alert(response.responseJSON.message);
            }
        })
    }


    if (method.toUpperCase() === "PUT") {


        $.ajax({
            url: url,
            method: method,
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (response) {
                if (response.status) {
                    alert(response.message)

                    if (callback) {
                        callback(response);
                    }
                }
                else {
                    alert("Failed: " + response.message);
                }
            },

            error: function (response) {
                alert(response);
            }
        })
    }
   
}

function ClearForm(formName, id, divName) {
    $(`#${formName.trim()}`)[0].reset();
    $(`#${id}`).val("0");
    $(`#${divName.trim()}`).empty();
}

function openModal(response) {
    
    var dataObj = Array.isArray(response.data) ? response.data[0] : response.data;
    var headers = Object.keys(dataObj);

    

    var modalContent = "";

    headers.map(function (head) {
        var values = dataObj[head];

        // checking if the type of value is object (or array)
        if (Array.isArray(values) && values !== null) {
            modalContent += `
            <div class="mb-3">
                <strong>${formatHeader(head)}:</strong>
                <ul>`;

            // Loop through the actual data array, NOT the 'head' string
            values.map(function (item) {
                // Because 'item' is an object, we grab the name property (or stringify it as a fallback)
                var displayValue = item.productName || item.name || JSON.stringify(item);

                modalContent += `<li>${displayValue}</li>`;
            });
            // Close the HTML for the list
            modalContent += `
                </ul>
            </div>`;
        }

        else {
            modalContent += `
            <div class="mb-3">
                <strong>${formatHeader(head)}:</strong>
                <span>${values !== null ? values : "N/A"}</span>
            </div>`;
        }
      
    });

     
    $("#infoModalBody").html(modalContent);

    $("#infoModal").modal("show");
}