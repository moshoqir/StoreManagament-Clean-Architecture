var selectedUserId = 0;
var permissionUserId = 0;
var usersCache = [];

$(document).ready(function () {
    // 1. Load permissions from the server (Replaces localStorage checks!)
    LoadCurrentPermissions(function () {
        GetRoles();
        GetUsers();
        UpdateUserButtons();
    });

    $('#btnRefresh').click(GetUsers);
    $('#btnSaveUser').click(SaveUser);
    $('#btnCancelUser').click(ClearUserForm);
    $('#btnSaveUserPermissions').click(SaveUserPermissions);
});

function GetRoles() {
    CallAjax('/Users/GetRoles', 'GET', null, function (response) {
        var html = '';
        $.each(response.data || [], function (index, role) {
            if (role.isActive) {
                html += '<option value="' + role.roleId + '">' + EscapeHtml(role.roleNameEn) + '</option>';
            }
        });
        $('#roleIds').html(html);
    });
}

function GetUsers() {
    CallAjax('/Users/GetList', 'GET', null, function (response) {
        FillUsersTable(response.data || []);
    });
}

function GetUserById(userId) {
    CallAjax('/Users/GetById', 'GET', { userId: userId }, function (response) {
        var user = response.data;

        selectedUserId = user.userId;
        $('#userId').val(user.userId);
        $('#userName').val(user.userName).prop('disabled', true);
        $('#fullName').val(user.fullName);
        $('#email').val(user.email || '');
        $('#phoneNumber').val(user.phoneNumber || '');
        $('#password').val('');
        $('#isActive').prop('checked', user.isActive);
        $('#roleIds').val((user.roleIds || []).map(String));
        $('#btnCancelUser').removeClass('d-none');

        UpdateUserButtons();
        $('#fullName').focus();
    });
}

function SaveUser() {
    var isCreate = selectedUserId === 0;

    if (isCreate && !HasPermission('Users.Create')) {
        ShowMessage('You do not have create permission.', 'danger');
        return;
    }

    if (!isCreate && !HasPermission('Users.Update')) {
        ShowMessage('You do not have update permission.', 'danger');
        return;
    }

    var userName = $('#userName').val().trim();
    var fullName = $('#fullName').val().trim();
    var password = $('#password').val();
    var roleIds = ($('#roleIds').val() || []).map(Number);

    // Validation
    if (isCreate && userName.length < 3) return ShowMessage('User name must be at least 3 characters.', 'danger');
    if (fullName.length < 2) return ShowMessage('Full name is required.', 'danger');
    if (isCreate && password.length < 6) return ShowMessage('Password must be at least 6 characters.', 'danger');
    if (!isCreate && password && password.length < 6) return ShowMessage('New password must be at least 6 characters.', 'danger');

    var requestData = {
        fullName: fullName,
        email: $('#email').val().trim() || null,
        phoneNumber: $('#phoneNumber').val().trim() || null,
        isActive: $('#isActive').is(':checked'),
        roleIds: roleIds
    };

    if (isCreate) {
        requestData.userName = userName;
        requestData.password = password;
        CallAjax('/Users/Create', 'POST', requestData, UserSaved, UserApiError);
    } else {
        requestData.newPassword = password || null;
        CallAjax('/Users/Update?userId=' + selectedUserId, 'PUT', requestData, UserSaved, UserApiError);
    }
}

function UserSaved(response) {
    ShowMessage(response.message, 'success');
    ClearUserForm();
    GetUsers();
}

function DeleteUser(userId) {
    if (!confirm('Are you sure you want to delete this user?')) return;

    CallAjax('/Users/Delete?userId=' + userId, 'DELETE', null, function (response) {
        ShowMessage(response.message, 'success');
        ClearUserForm();
        GetUsers();
    });
}

// 2. Hybrid Table Generator (Manual HTML mapped to DataTables with FontAwesome Icons)
function FillUsersTable(users) {
    usersCache = users;
    var html = '';

    // Prevent DataTables initialization errors by destroying the old instance first
    if ($.fn.DataTable.isDataTable('#usersTable')) {
        $('#usersTable').DataTable().destroy();
    }

    if (users.length === 0) {
        $('#usersBody').html('<tr><td colspan="5" class="text-center py-4">No users found.</td></tr>');
        return;
    }

    $.each(users, function (index, user) {
        html += '<tr>';
        html += '<td>' + EscapeHtml(user.userName) + '</td>';
        html += '<td>' + EscapeHtml(user.fullName) + '</td>';
        html += '<td>' + EscapeHtml((user.roleNames || []).join(', ')) + '</td>';
        html += '<td>' + (user.isActive ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-secondary">Inactive</span>') + '</td>';

        html += '<td class="text-end">';

        // Template Icons injected based on exact permissions
        if (HasPermission('Users.Update')) {
            html += `<a href='#' class='editBtn me-3' onclick='GetUserById(${user.userId})'><i class="fa-solid fa-pen" style="color: rgb(11, 174, 124);"></i></a>`;
        }

        if (HasPermission('Users.ManagePermissions')) {
            html += `<a href='#' class='infoBtn me-3' onclick='OpenUserPermissions(${user.userId})'><i class="fa-solid fa-user-shield" style="color: rgb(115, 131, 159);"></i></a>`;
        }

        if (HasPermission('Users.Delete')) {
            html += `<a href='#' class='deleteBtn me-3' onclick='DeleteUser(${user.userId})'><i class="fa-solid fa-trash" style="color: rgb(215, 13, 43);"></i></a>`;
        }

        html += '</td></tr>';
    });

    $('#usersBody').html(html);

    // Re-initialize DataTable with your template's styling
    $('#usersTable').DataTable({ destroy: true, responsive: true });
}

function OpenUserPermissions(userId) {
    permissionUserId = userId;
    var user = usersCache.find(function (item) { return item.userId === userId; });
    $('#permissionUserName').text(user ? user.userName : '');
    $('#userPermissionsBody').html('<tr><td colspan="5" class="text-center">Loading...</td></tr>');

    CallAjax('/Users/GetPermissions', 'GET', { userId: userId }, function (response) {
        FillUserPermissions(response.data || []);
        bootstrap.Modal.getOrCreateInstance(document.getElementById('userPermissionsModal')).show();
    });
}

function FillUserPermissions(permissions) {
    var html = '';
    $.each(permissions, function (index, item) {
        var value = item.userOverride === null || item.userOverride === undefined ? '' : String(item.userOverride);
        html += '<tr>';
        html += '<td>' + EscapeHtml(item.menuNameEn) + '</td>';
        html += '<td>' + EscapeHtml(item.actionNameEn) + '</td>';
        html += '<td>' + (item.roleAllowed ? '<span class="badge bg-success">Allow</span>' : '<span class="badge bg-secondary">Deny</span>') + '</td>';
        html += '<td>';
        html += '<select class="form-select form-select-sm user-permission-select" data-menu-action-id="' + item.menuActionId + '">';
        html += '<option value=""' + (value === '' ? ' selected' : '') + '>Use Role</option>';
        html += '<option value="true"' + (value === 'true' ? ' selected' : '') + '>Allow</option>';
        html += '<option value="false"' + (value === 'false' ? ' selected' : '') + '>Deny</option>';
        html += '</select>';
        html += '</td>';
        html += '<td>' + (item.effectiveAllowed ? '<span class="badge bg-success">Allowed</span>' : '<span class="badge bg-danger">Denied</span>') + '</td>';
        html += '</tr>';
    });
    $('#userPermissionsBody').html(html);
}

function SaveUserPermissions() {
    var permissions = [];
    $('.user-permission-select').each(function () {
        var value = $(this).val();
        permissions.push({
            menuActionId: Number($(this).attr('data-menu-action-id')),
            isAllowed: value === '' ? null : value === 'true'
        });
    });

    CallAjax('/Users/UpdatePermissions?userId=' + permissionUserId, 'PUT', { permissions: permissions }, function (response) {
        ShowMessage(response.message, 'success');
        bootstrap.Modal.getInstance(document.getElementById('userPermissionsModal')).hide();
    });
}

function ClearUserForm() {
    selectedUserId = 0;
    $('#userId').val('');
    $('#userName').val('').prop('disabled', false);
    $('#fullName').val('');
    $('#email').val('');
    $('#phoneNumber').val('');
    $('#password').val('');
    $('#isActive').prop('checked', true);
    $('#roleIds').val([]);
    $('#btnCancelUser').addClass('d-none');
    UpdateUserButtons();
}

function UpdateUserButtons() {
    var canSave = selectedUserId === 0 ? HasPermission('Users.Create') : HasPermission('Users.Update');
    $('#btnSaveUser').toggleClass('d-none', !canSave).text(selectedUserId === 0 ? 'Save User' : 'Update User');
}

function UserApiError(response) {
    var message = response && response.message ? response.message : 'The operation could not be completed.';
    if (response && response.errors) {
        var firstKey = Object.keys(response.errors)[0];
        if (firstKey && response.errors[firstKey].length > 0) {
            message = response.errors[firstKey][0];
        }
    }
    ShowMessage(message, 'danger');
}