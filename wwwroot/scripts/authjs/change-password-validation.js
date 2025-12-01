const newPasswordInput = document.getElementById("newPassword");
const confirmPasswordInput = document.getElementById("confirmPassword");
const passwordError = document.getElementById("passwordError");
const confirmError = document.getElementById("confirmError");

// Reglas
const regexMinLength = /.{8,}/;
const regexUpper = /[A-Z]/;
const regexLower = /[a-z]/;
const regexNumber = /[0-9]/;
const regexSpecial = /[!@#$%^&*(),.?":{}|<>]/;

// 🔥 VALIDACIÓN EN TIEMPO REAL NUEVA CONTRASEÑA
newPasswordInput.addEventListener("input", function () {
    const pwd = newPasswordInput.value;

    passwordError.textContent = "";

    if (!regexMinLength.test(pwd)) {
        passwordError.textContent = "Debe tener mínimo 8 caracteres.";
    } else if (!regexUpper.test(pwd)) {
        passwordError.textContent = "Debe contener al menos 1 mayúscula.";
    } else if (!regexLower.test(pwd)) {
        passwordError.textContent = "Debe contener al menos 1 minúscula.";
    } else if (!regexNumber.test(pwd)) {
        passwordError.textContent = "Debe contener al menos 1 número.";
    } else if (!regexSpecial.test(pwd)) {
        passwordError.textContent = "Debe contener al menos 1 caracter especial.";
    } else {
        passwordError.textContent = "✔ Contraseña válida";
        passwordError.classList.remove("text-red-600");
        passwordError.classList.add("text-green-600");
    }
});

// 🔥 VALIDACIÓN EN TIEMPO REAL CONFIRMAR CONTRASEÑA
confirmPasswordInput.addEventListener("input", function () {
    confirmError.textContent = "";

    if (confirmPasswordInput.value !== newPasswordInput.value) {
        confirmError.textContent = "Las contraseñas no coinciden.";
    } else if (confirmPasswordInput.value.length > 0) {
        confirmError.textContent = "✔ Coinciden";
        confirmError.classList.remove("text-red-600");
        confirmError.classList.add("text-green-600");
    }
});

// 🔥 VALIDACIÓN FINAL EN SUBMIT
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

        passwordError.textContent = "La contraseña no cumple los requisitos.";
        isValid = false;
    }

    if (newPassword !== confirmPassword) {
        confirmError.textContent = "Las contraseñas no coinciden.";
        isValid = false;
    }

    if (!isValid) e.preventDefault();
});
