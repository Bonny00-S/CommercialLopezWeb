document.addEventListener("DOMContentLoaded", function () {

    const nameInput = document.getElementById("Name");
    const nameError = document.getElementById("nameError");
    const form = document.querySelector("form");

    // 🟦 Remove double spaces + convert to uppercase
    function cleanInput(input) {
        input.value = input.value.replace(/\s{2,}/g, " "); // no double spaces
        input.value = input.value.toUpperCase();           // convert to UPPERCASE
    }

    // 🟦 Real-time validation
    nameInput.addEventListener("input", () => {
        cleanInput(nameInput);

        if (nameInput.value.trim().length < 3) {
            nameError.textContent = "Name must have at least 3 characters.";
        } else {
            nameError.textContent = "";
        }
    });

    // 🟩 Validation before submit
    form.addEventListener("submit", function (e) {
        let valid = true;

        if (nameInput.value.trim().length < 3) {
            nameError.textContent = "Name must have at least 3 characters.";
            valid = false;
        }

        if (!valid) {
            e.preventDefault(); // ❌ Prevent form submission
        }
    });

});
