
    document.addEventListener("DOMContentLoaded", function () {

        const nameInput = document.getElementById("Name");
    const descriptionInput = document.getElementById("Description");

    const nameError = document.getElementById("nameError");
    const descriptionError = document.getElementById("descriptionError");

    const form = document.querySelector("form");

    // 🟦 Elimina espacios duplicados mientras se escribe
    function noDoubleSpaces(input) {
        input.value = input.value.replace(/\s{2,}/g, " ");
        }

        nameInput.addEventListener("input", () => {
        noDoubleSpaces(nameInput);

    if (nameInput.value.trim().length < 3) {
        nameError.textContent = "El nombre debe tener al menos 3 caracteres.";
            } else {
        nameError.textContent = "";
            }
        });

        descriptionInput.addEventListener("input", () => {
        noDoubleSpaces(descriptionInput);

    if (descriptionInput.value.trim().length < 5) {
        descriptionError.textContent = "La descripción debe tener al menos 5 caracteres.";
            } else {
        descriptionError.textContent = "";
            }
        });

    // 🟩 Validar antes de enviar
    form.addEventListener("submit", function (e) {

        let valid = true;

    if (nameInput.value.trim().length < 3) {
        nameError.textContent = "El nombre debe tener al menos 3 caracteres.";
    valid = false;
            }

    if (descriptionInput.value.trim().length < 5) {
        descriptionError.textContent = "La descripción debe tener al menos 5 caracteres.";
    valid = false;
            }

    if (!valid) {
        e.preventDefault(); // ❌ No envía el formulario si hay errores
            }
        });

    });

