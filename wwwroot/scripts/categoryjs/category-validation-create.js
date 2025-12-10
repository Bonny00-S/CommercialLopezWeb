document.addEventListener("DOMContentLoaded", function () {

    const nameInput = document.getElementById("Name");
    const descriptionInput = document.getElementById("Description");

    const nameError = document.getElementById("nameError");
    const descriptionError = document.getElementById("descriptionError");

    const form = document.querySelector("form");

    // 🟦 Elimina espacios duplicados y convierte a mayúsculas
    function cleanInput(input) {
        input.value = input.value.replace(/\s{2,}/g, " "); // sin doble espacio
        input.value = input.value.toUpperCase();           // convertir a MAYÚSCULAS
    }

    // =============================
    // 🔹 VALIDACIÓN EN TIEMPO REAL
    // =============================
    nameInput.addEventListener("input", () => {
        cleanInput(nameInput);

        if (nameInput.value.trim().length < 3) {
            nameError.textContent = "Name must be at least 3 characters.";
        } else {
            nameError.textContent = "";
        }
    });

    descriptionInput.addEventListener("input", () => {
        cleanInput(descriptionInput);

        if (descriptionInput.value.trim().length < 5) {
            descriptionError.textContent = "Description must be at least 5 characters.";
        } else {
            descriptionError.textContent = "";
        }
    });

    // =============================
    // 🔹 VALIDACIÓN AL ENVIAR
    // =============================
    form.addEventListener("submit", function (e) {

        let valid = true;

        if (nameInput.value.trim().length < 3) {
            nameError.textContent = "Name must be at least 3 characters.";
            valid = false;
        }

        if (descriptionInput.value.trim().length < 5) {
            descriptionError.textContent = "Description must be at least 5 characters.";
            valid = false;
        }

        if (!valid) {
            e.preventDefault();
        }
    });

});
