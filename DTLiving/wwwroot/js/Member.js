// #region Function => DOMContentLoaded    檢查是否為登入狀態
document.addEventListener("DOMContentLoaded", function () {
    LoginStatus();
});
// #endregion

// #region Function => LoginStatus()       檢查登入狀態
function LoginStatus() {

    var token = localStorage.getItem('JwtToken');

    if (token) {

        let clientbox = document.getElementById('membericon');
        clientbox.style.display = 'none';

        var tokenParts = token.split('.');
        var payload = JSON.parse(atob(tokenParts[1]));
        var userName = payload["UserName"];

        document.getElementById('JwtCatchName').innerText = userName;
        document.getElementById('Userlink').style.display = 'block';

    } else {

        let CatchJWTName = document.getElementById("JwtCatchName");
        var CatchLogout = document.getElementById('logout');

        CatchJWTName.style.display = "none";
        CatchLogout.style.display = 'none';
    }
}
// #endregion

// #region Function => RegisterformVerify()   會員註冊表單驗證

function RegisterFormVerify() {

    var usereid = document.getElementById('usereid');
    var username = document.getElementById('username');
    var usermail = document.getElementById('usermail');
    var userphone = document.getElementById('userphone');
    var userpassword = document.getElementById('userpassword');

    const IdRules = /^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,10}$/
    const PhoneRules = /^[09]\d{8}[0-9]$/;
    const MailRules = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
    const PasswordRules = /^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,13}$/;


    if (!MailRules.test(usermail.value)) {
        alert("請確認Email輸入格式");
        usermail.value = " ";
        return;
    }

    if (!PhoneRules.test(userphone.value)) {
        alert("請確認電話輸入格式");
        userphone.value = " ";
        return;
    }

    if (!PasswordRules.test(userpassword.value)) {
        alert("請確認密碼輸入格式");
        userpassword.value = " ";
        return;
    }

    MemberRegister();

}

// #endregion

// #region Function => LoginFormVerify()      會員登入表單驗證

function LoginFormVerify() {

    let loginaccount = document.getElementById('loginaccount');
    let loginpassword = document.getElementById('loginpassword');

    const IdRules = /^(?=.*[A-Z])[A-Za-z\d]{7,11}$/
    const PasswordRules = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,13}$/;

    if (!IdRules.test(loginaccount.value)) {
        alert("請確認您的帳號");
        loginaccount.value = " ";
        return;
    }

    if (!PasswordRules.test(loginpassword.value)) {
        alert("請確認您的密碼");
        loginpassword.value = " ";
        return;
    }

    MemberLogin()
}

// #endregion 

// #region Function => ClearFormData()     清除登入表單內容
function ClearFormData() {

    var form = document.getElementById("registerform");
    var inputs = form.getElementsByTagName("input");

    for (var i = 0; i < inputs.length; i++) {
        inputs[i].value = " ";
    }

    var selects = form.getElementsByTagName("select");
    for (var i = 0; i < selects.length; i++) {
        selects[i].selectedIndex = 0;
    }

    var textareas = form.getElementsByTagName("textarea");
    for (var i = 0; i < textareas.length; i++) {
        textareas[i].value = " ";
    }
}
// #endregion

// #region Function => MemberRegister()    建立會員
function MemberRegister() {

    // 檢查表單內容是否為空
    if ($('#usereid').val() === "" ||
        $('#userpassword').val() === "" ||
        $('#username').val() === "" ||
        $('#userphone').val() === "" ||
        $('#usermail').val() === "" ||
        $('#useraddress').val() === "") {

        alert("*為必填項目");
        return;
    }

    // JSON 格式 : 後端命名 : 前端資料
    var ClientRegisterData = {
        UserID: $('#usereid').val(),
        Userpassword: $('#userpassword').val(),
        UserName: $('#username').val(),
        UserPhone: $('#userphone').val(),
        UserMail: $('#usermail').val(),
        UserAddress: $('#useraddress').val()
    };

    $.ajax({
        type: 'POST',
        url: '/Client/ClientCheck',
        contentType: 'application/json',
        data: JSON.stringify(ClientRegisterData),
        success: function (response) {

            alert(response);

            if (response == '註冊成功') {
                window.location.href = 'Index.html';
            }
        },
        error: function (error) {
            console.log(JSON.stringify(ClientRegisterData))
            console.error(error);
        }
    });
    return false;
}
// #endregion

// #region Function => MemberLogin()       會員登入
function MemberLogin() {

    var count = 3;

    var ClientLoginData = {
        UserID: $('#loginaccount').val(),
        UserPassword: $('#loginpassword').val()
    };

    $.ajax({
        type: 'POST',
        url: '/Client/ClientLogin',
        contentType: 'application/json',
        data: JSON.stringify(ClientLoginData),
        success: function (response) {

            if (response.hasOwnProperty('token')) {

                // 成功登入，將 Token 儲存在 Local Storage 中
                console.log("Token 獲取结果 :", JSON.stringify(response.token))
                localStorage.setItem('JwtToken', response.token);
                alert("登入成功");
                window.location.href = "/Index.html";

            }
        },
        error: function (error) {

            alert("請重新登入");
            $('#loginaccount').val('');
            $('#loginpassword').val('');
        }
    });
}
// #endregion

// #region Function => MemberLogout()      會員登出
function MemberLogout() {

    localStorage.removeItem('JwtToken');

    window.location.href = "/Index.html";
}
// #endregion

// #region Function => EmployerLogin()     管理員登入

function EmployerLogin() {

    var EmployerId = $('#EmployId').val();

    console.log(EmployerId)

    $.ajax({
        type: 'GET',
        url: '/Employer/EmployerLogin?staffId=' + EmployerId,
        success: function (response) {

            console.log(response);
            window.location.href = "Home/Index";

        },
        error: function (error) {
            // 在這裡處理錯誤的情況
            console.error('Error:', error); // 在控制台中輸出錯誤信息
        }
    });
}

// #endregion

// #region Function => ShowForm()          顯示表單功能
function ShowRegisterForm() {
    const modalsize = document.getElementById('showmodal');
    modalsize.className = 'modal-dialog modal-xl';

    document.getElementById('registerform').style.display = 'block';
    document.getElementById('loginform').style.display = 'none';
    document.getElementById('adminform').style.display = 'none';
}

function ShowLoginForm() {
    const modalsize = document.getElementById('showmodal');
    modalsize.className = 'modal-dialog';

    document.getElementById('registerform').style.display = 'none';
    document.getElementById('loginform').style.display = 'block';
    document.getElementById('adminform').style.display = 'none';
}

function ShowAdminForm() {
    const modalsize = document.getElementById('showmodal');
    modalsize.className = 'modal-dialog';

    document.getElementById('registerform').style.display = 'none';
    document.getElementById('loginform').style.display = 'none';
    document.getElementById('adminform').style.display = 'block';
}
// #endregion

