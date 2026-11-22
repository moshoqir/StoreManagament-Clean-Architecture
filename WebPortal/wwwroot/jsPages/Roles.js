var selectedRoleId = 0;
var permissionRoleId = 0;
var rolesCache = [];

$(document).ready(function () {
    // Load permissions from the server (Replaces localStorage checks)
    LoadCurrentPermissions(function () {
        GetRoles();
        UpdateRoleButtons();
    });

    $('#btnRefresh').click(GetRoles);
    $('#btnSaveRole').click(SaveRole);
    $('#btnCancelRole').click(ClearRoleForm);
    $('#btnSaveRolePermissions').click(SaveRolePermissions);
});

function GetRoles() {
    CallAjax('/Roles/GetList', 'GET', null, function (response) {
        FillRolesTable(response.data || []);
    });
}

function GetRoleById(roleId) {
    CallAjax('/Roles/GetById', 'GET', { roleId: roleId }, function (response) {
        var role = response.data;

        selectedRoleId = role.roleId;
        $('#roleId').val(role.roleId);
        $('#roleNameEn').val(role.roleNameEn);
        $('#roleNameAr').val(role.roleNameAr);
        $('#roleIsActive').prop('checked', role.isActive);
        $('#btnCancelRole').removeClass('d-none');

        UpdateRoleButtons();
        $('#roleNameEn').focus();
    });
}

function SaveRole() {
    var isCreate = selectedRoleId === 0;
    var roleNameEn = $('#roleNameEn').val().trim();
    var roleNameAr = $('#roleNameAr').val().trim();

    if (!roleNameEn || !roleNameAr) {
        ShowMessage('Arabic and English role names are required.', 'danger');
        return;
    }

    var request = {
        roleNameAr: roleNameAr,
        roleNameEn: roleNameEn,
        isActive: $('#roleIsActive').is(':checked')
    };

    if (isCreate) {
        CallAjax('/Roles/Create', 'POST', request, RoleSaved, RoleApiError);
    }
    else {
        CallAjax('/Roles/Update?roleId=' + selectedRoleId, 'PUT', request, RoleSaved, RoleApiError);
    }
}

function RoleSaved(response) {
    ShowMessage(response.message, 'success');
    ClearRoleForm();
    GetRoles();
}

function DeleteRole(roleId) {
    if (!confirm('Are you sure you want to delete this role?')) return;

    CallAjax('/Roles/Delete?roleId=' + roleId, 'DELETE', null, function (response) {
        ShowMessage(response.message, 'success');
        ClearRoleForm();
        GetRoles();
    });
}

// Hybrid Table Generator for Roles
function FillRolesTable(roles) {
    rolesCache = roles;
    var html = '';

    // Prevent DataTables crashing by destroying the old instance first
    if ($.fn.DataTable.isDataTable('#rolesTable')) {
        $('#rolesTable').DataTable().destroy();
    }

    if (roles.length === 0) {
        $('#rolesBody').html('<tr><td colspan="5" class="text-center py-4">No roles found.</td></tr>');
        return;
    }

    $.each(roles, function (index, role) {
        html += '<tr>';
        html += '<td>' + role.roleId + '</td>';
        html += '<td>' + EscapeHtml(role.roleNameEn) + '</td>';
        html += '<td>' + EscapeHtml(role.roleNameAr) + '</td>';
        html += '<td>' + (role.isActive ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-secondary">Inactive</span>') + '</td>';

        html += '<td class="text-end">';

        // Action Buttons using Template Icons
        if (HasPermission('Roles.Update')) {
            html += `<a href='#' class='editBtn me-3' onclick='GetRoleById(${role.roleId})' title='Edit Role'>
                        <i class="fa-solid fa-pen" style="color: rgb(11, 174, 124);"></i>
                     </a>`;
        }

        if (HasPermission('Roles.ManagePermissions')) {
            html += `<a href='#' class='infoBtn me-3' onclick='OpenRolePermissions(${role.roleId})' title='Manage Permissions'>
                        <i class="fa-solid fa-user-shield" style="color: rgb(115, 131, 159);"></i>
                     </a>`;
        }

        if (HasPermission('Roles.Delete')) {
            html += `<a href='#' class='deleteBtn me-3' onclick='DeleteRole(${role.roleId})' title='Delete Role'>
                        <i class="fa-solid fa-trash" style="color: rgb(215, 13, 43);"></i>
                     </a>`;
        }

        html += '</td></tr>';
    });

    $('#rolesBody').html(html);

    // Re-initialize DataTable
    $('#rolesTable').DataTable({ destroy: true, responsive: true });
}

function OpenRolePermissions(roleId) {
    permissionRoleId = roleId;
    var role = rolesCache.find(function (item) { return item.roleId === roleId; });
    $('#permissionRoleName').text(role ? role.roleNameEn : '');
    $('#rolePermissionsBody').html('<tr><td colspan="3" class="text-center">Loading...</td></tr>');

    CallAjax('/Roles/GetPermissions', 'GET', { roleId: roleId }, function (response) {
        FillRolePermissions(response.data || []);
        bootstrap.Modal.getOrCreateInstance(document.getElementById('rolePermissionsModal')).show();
    });
}

function FillRolePermissions(permissions) {
    var html = '';

    $.each(permissions, function (index, item) {
        html += '<tr>';
        html += '<td>' + EscapeHtml(item.menuNameEn) + '</td>';
        html += '<td>' + EscapeHtml(item.actionNameEn) + '</td>';
        html += '<td class="text-center">';
        html += '<input class="form-check-input role-permission-check" type="checkbox" data-menu-action-id="' + item.menuActionId + '"' + (item.isAllowed ? ' checked' : '') + ' />';
        html += '</td>';
        html += '</tr>';
    });

    $('#rolePermissionsBody').html(html);
}

function SaveRolePermissions() {
    var menuActionIds = [];

    $('.role-permission-check:checked').each(function () {
        menuActionIds.push(Number($(this).attr('data-menu-action-id')));
    });

    CallAjax(
        '/Roles/UpdatePermissions?roleId=' + permissionRoleId,
        'PUT',
        { menuActionIds: menuActionIds },
        function (response) {
            ShowMessage(response.message, 'success');
            bootstrap.Modal.getInstance(document.getElementById('rolePermissionsModal')).hide();
        }
    );
}

function ClearRoleForm() {
    selectedRoleId = 0;
    $('#roleId').val('');
    $('#roleNameEn').val('');
    $('#roleNameAr').val('');
    $('#roleIsActive').prop('checked', true);
    $('#btnCancelRole').addClass('d-none');
    UpdateRoleButtons();
}

function UpdateRoleButtons() {
    var canSave = selectedRoleId === 0 ? HasPermission('Roles.Create') : HasPermission('Roles.Update');
    $('#btnSaveRole').toggleClass('d-none', !canSave).text(selectedRoleId === 0 ? 'Save Role' : 'Update Role');
}

function RoleApiError(response) {
    var message = response && response.message ? response.message : 'The operation could not be completed.';
    if (response && response.errors) {
        var firstKey = Object.keys(response.errors)[0];
        if (firstKey && response.errors[firstKey].length > 0) {
            message = response.errors[firstKey][0];
        }
    }
    ShowMessage(message, 'danger');
}