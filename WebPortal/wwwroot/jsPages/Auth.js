$(function () {
    // Better UX: Auto-focus the username field when the page loads
    $('#UserName').focus();

    // Use the native form submit to automatically handle both button clicks and the 'Enter' key
    $("#loginForm").submit(function (e) {
        e.preventDefault();
        Login();
    });
});

function Login() {
    var userName = $('#UserName').val().trim();
    var password = $('#Password').val();

    // 1. Client-Side Validation
    if (!userName || !password) {
        $("#LoginSpan").text("User name and password are required.").css("color", "red");
        return;
    }

    // 2. Reset UI State
    $("#LoginSpan").text("");
    $("#login").prop("disabled", true).text("Signing in...");

    // 3. API Call using the unified CallAjax engine
    CallAjax(
        '/Account/Login', // Pointing to your C# WebPortal controller, not the direct WebApi
        'POST',
        {
            UserName: userName,
            Password: password
        },
        function (response) {
            // SUCCESS: The C# controller successfully created the secure Auth Cookie.
            // We NO LONGER use localStorage. We just follow the dynamic redirect URL.
            window.location.href = response.data.redirectUrl;
        },
        function (response) {
            // ERROR: Re-enable the button and show the error message
            $("#login").prop("disabled", false).text("Sign In");

            var errorMsg = response && response.message ? response.message : "Invalid username or password.";
            $("#LoginSpan").text(errorMsg).css("color", "red");
        }
    );
}