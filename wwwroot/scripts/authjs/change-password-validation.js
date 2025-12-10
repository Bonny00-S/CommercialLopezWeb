const newPasswordInput = document.getElementById("newPassword");
const confirmPasswordInput = document.getElementById("confirmPassword");
const passwordError = document.getElementById("passwordError");
const confirmError = document.getElementById("confirmError");

// Rules
const regexMinLength = /.{8,}/;
const regexUpper = /[A-Z]/;
const regexLower = /[a-z]/;
const regexNumber = /[0-9]/;
const regexSpecial = /[!@#$%^&*(),.?":{}|<>]/;

// 🔥 REAL-TIME VALIDATION NEW PASSWORD
newPasswordInput.addEventListener("input", function () {
    const pwd = newPasswordInput.value;

    passwordError.textContent = "";
    passwordError.classList.remove("text-green-600");
    passwordError.classList.add("text-red-600");

    if (!regexMinLength.test(pwd)) {
        passwordError.textContent = "Must be at least 8 characters long.";
    } else if (!regexUpper.test(pwd)) {
        passwordError.textContent = "Must contain at least 1 uppercase letter.";
    } else if (!regexLower.test(pwd)) {
        passwordError.textContent = "Must contain at least 1 lowercase letter.";
    } else if (!regexNumber.test(pwd)) {
        passwordError.textContent = "Must contain at least 1 number.";
    } else if (!regexSpecial.test(pwd)) {
        passwordError.textContent = "Must contain at least 1 special character.";
    } else {
        passwordError.textContent = "✔ Valid password";
        passwordError.classList.remove("text-red-600");
        passwordError.classList.add("text-green-600");
    }
});

// 🔥 REAL-TIME VALIDATION CONFIRM PASSWORD
confirmPasswordInput.addEventListener("input", function () {
    confirmError.textContent = "";
    confirmError.classList.remove("text-green-600");
    confirmError.classList.add("text-red-600");

    if (confirmPasswordInput.value !== newPasswordInput.value) {
        confirmError.textContent = "Passwords do not match.";
    } else if (confirmPasswordInput.value.length > 0) {
        confirmError.textContent = "✔ Passwords match";
        confirmError.classList.remove("text-red-600");
        confirmError.classList.add("text-green-600");
    }
});

// 🔥 FINAL VALIDATION ON SUBMIT
document.getElementById("changePasswordForm").addEventListener("submit", function (e) {
    let isValid = true;

    const newPassword = newPasswordInput.value.trim();
    const confirmPassword = confirmPasswordInput.value.trim();

    passwordError.textContent = "";
    confirmError.textContent = "";

    if (!regexMinLength.test(newPassword) ||
        !regexUpper.test(newPassword) ||
        !regexLower.test(newPassword) ||
        !regexNumber.test(newPassword) ||
        !regexSpecial.test(newPassword)) {

        passwordError.textContent = "Password does not meet the required rules.";
        isValid = false;
    }

    if (newPassword !== confirmPassword) {
        confirmError.textContent = "Passwords do not match.";
        isValid = false;
    }

    if (!isValid) e.preventDefault();
});
