// ==========================================
// 1. CORE AJAX ENGINE
// ==========================================
function CallAjax(url, method, data, successCallback, errorCallback) {
    var ajaxOptions = {
        url: url,
        type: method,
        dataType: 'json',
        success: function (response) {
            if (successCallback) {
                successCallback(response);
            }
        },
        error: function (xhr) {
            var response = xhr.responseJSON;

            if (errorCallback) {
                errorCallback(response, xhr);
                return;
            }

            var errorMsg = "Request failed.";
            if (response && response.message) {
                errorMsg = response.message;
            } else if (xhr && xhr.status) {
                errorMsg = "API returned Error " + xhr.status + ".";
            }

            ShowMessage(errorMsg, 'danger');
            console.error("AJAX Error:", xhr.status, response);
        }
    };

    if (method.toUpperCase() === 'GET') {
        if (data) {
            ajaxOptions.data = data;
        }
    } else {
        ajaxOptions.contentType = 'application/json; charset=utf-8';
        ajaxOptions.data = JSON.stringify(data || {});
    }

    return $.ajax(ajaxOptions);
}

// ==========================================
// 2. PERMISSIONS & SECURITY
// ==========================================
var currentPermissions = [];

function LoadCurrentPermissions(callback) {
    CallAjax(
        '/Account/GetPermissions',
        'GET',
        null,
        function (response) {
            currentPermissions = response.data || [];
            ApplyPermissionElements();
            if (callback) {
                callback();
            }
        }
    );
}

function HasPermission(permissionCode) {
    return currentPermissions.indexOf(permissionCode) !== -1;
}

function ApplyPermissionElements() {
    $('[data-permission]').each(function () {
        var permissionCode = $(this).attr('data-permission');
        if (HasPermission(permissionCode)) {
            $(this).removeClass('d-none');
        } else {
            $(this).addClass('d-none');
        }
    });
}

function Logout() {
    CallAjax('/Account/Logout', 'POST', {}, function () {
        window.location = '/Account';
    });
}

// ==========================================
// 3. UI UTILITIES & MESSAGING
// ==========================================
function ShowMessage(message, type) {
    var alertType = type || 'success';
    $('#pageMessage').html(
        '<div class="alert alert-' + alertType + ' alert-dismissible fade show" role="alert">' +
        EscapeHtml(message) +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>'
    );
}

function EscapeHtml(value) {
    return $('<div>').text(value == null ? '' : value).html();
}

function ClearForm(formName, id, divName) {
    $(`#${formName.trim()}`)[0].reset();
    $(`#${id}`).val("0");
    if (divName) {
        $(`#${divName.trim()}`).empty();
    }
}

// ==========================================
// 4. TEMPLATE UI WRAPPER & TABLE GENERATORS
// ==========================================
function ajaxConnection(url, method, data, callback, tableName, divName, hideColumn) {
    CallAjax(url, method, data,
        function (response) {
            if (method.toUpperCase() === "GET") {
                if (url.includes("ById")) {
                    openModal(response);
                } else {
                    if (!response || !response.data || response.data.length === 0) {
                        $(`#${divName}`).html("<div class='alert alert-info'>No records found.</div>");
                        return;
                    }
                    var table = createTable(tableName, response, hideColumn);
                    $(`#${divName}`).html(table);
                    new DataTable(`#${tableName}Table`);
                }
            } else if (method.toUpperCase() === "POST" || method.toUpperCase() === "PUT") {
                if (response.status) {
                    ShowMessage(response.message, 'success');
                    if (callback) callback(response);
                } else {
                    ShowMessage("Failed: " + response.message, 'danger');
                }
            }
        }
    );
}

function formatHeader(header) {
    return header
        .replace(/([a-z])([A-Z])/g, '$1 $2')
        .replace(/^./, function (char) { return char.toUpperCase(); })
        .replace(/Id\b/g, 'ID')
        .trim();
}

function createTable(id, response, hiddenColumn = "") {
    var headers = Object.keys(response.data[0]);
    var formattedHeaders = headers.filter(function (head) {
        return head.toLowerCase() !== hiddenColumn.toLowerCase();
    });

    var header = formattedHeaders.map(function (head) {
        return `<th>${formatHeader(head)}</th>`;
    }).join("");

    var table = `<table id='${id}Table' class='table striped-table'>
                    <thead>
                        ${header}
                        <th class="text-end" style="width:120px;">Actions</th>
                    </thead>
                    <tbody>`;

    // 1. Format the ID to match your permission matrix (e.g., "category" becomes "Category")
    var moduleName = id.charAt(0).toUpperCase() + id.slice(1);

    // 2. Pre-calculate the permissions for this specific table
    var canEdit = HasPermission(moduleName + '.Update');
    var canDelete = HasPermission(moduleName + '.Delete');
    var canView = HasPermission(moduleName + '.View');

    response.data.map(function (data) {
        var stringifiedItem = JSON.stringify(data).replace(/"/g, '&quot;');
        table += `<tr data-row="${stringifiedItem}">`;

        headers.map(function (head) {
            if (head.toLowerCase() === hiddenColumn.toLowerCase()) return;
            table += `<td>${data[head] !== null ? data[head] : ""}</td>`;
        });

        // 3. Inject ONLY the buttons the user is allowed to see
        table += `<td class="text-end">`;

        if (canEdit) {
            table += `<a href='#' class='editBtn'><i class="fa-solid fa-pen ms-3" style="color: rgb(11, 174, 124);"></i></a>`;
        }
        if (canDelete) {
            table += `<a href='#' class='deleteBtn'><i class="fa-solid fa-trash ms-3" style="color: rgb(215, 13, 43);"></i></a>`;
        }
        if (canView) {
            table += `<a href='#' class='infoBtn'><i class="fa-solid fa-eye ms-3" style="color: rgb(115, 131, 159);"></i></a>`;
        }

        table += `</td></tr>`;
    });

    table += `</tbody></table>`;
    return table;
}

function openModal(response) {
    var dataObj = Array.isArray(response.data) ? response.data[0] : response.data;
    var headers = Object.keys(dataObj);
    var modalContent = "";

    headers.map(function (head) {
        var values = dataObj[head];
        if (Array.isArray(values) && values !== null) {
            modalContent += `<div class="mb-3"><strong>${formatHeader(head)}:</strong><ul>`;
            values.map(function (item) {
                var displayValue = item.productName || item.name || JSON.stringify(item);
                modalContent += `<li>${displayValue}</li>`;
            });
            modalContent += `</ul></div>`;
        } else {
            modalContent += `<div class="mb-3">
                                <strong>${formatHeader(head)}:</strong>
                                <span>${values !== null ? values : "N/A"}</span>
                             </div>`;
        }
    });

    $("#infoModalBody").html(modalContent);
    $("#infoModal").modal("show");
}