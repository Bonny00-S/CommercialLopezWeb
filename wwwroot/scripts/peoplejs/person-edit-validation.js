document.addEventListener("DOMContentLoaded", () => {

    // -------------------------------------------------------------
    // MAYÚSCULAS AUTOMÁTICAS (menos email)
    // -------------------------------------------------------------
    document.querySelectorAll("input[type='text']:not(#Email)").forEach(input => {
        input.addEventListener("input", () => {
            input.value = input.value.toUpperCase();
        });
    });

    const form = document.getElementById("personForm");

    // Inputs
    const ci = document.getElementById("ci");
    const name = document.getElementById("Name");
    const lastName = document.getElementById("LastName");
    const dateBirth = document.getElementById("DateBirth");
    const address = document.getElementById("Address");
    const phone = document.getElementById("Phone");
    const email = document.getElementById("Email");
    const role = document.getElementById("Role");

    // Error span
    const ciError = document.getElementById("ciError");
    const nameError = document.getElementById("NameError");
    const lastNameError = document.getElementById("LastNameError");
    const dateBirthError = document.getElementById("DateBirthError");
    const addressError = document.getElementById("AddressError");
    const phoneError = document.getElementById("PhoneError");
    const emailError = document.getElementById("EmailError");
    const roleError = document.getElementById("RoleError");

    form.addEventListener("submit", (e) => {
        let valid = true;

        // -------------------------------------------------------------
        // VALIDAR CI (solo números de 8 a 10 dígitos)
        // -------------------------------------------------------------
        const ciRegex = /^[0-9]{8,10}$/;
        if (!ciRegex.test(ci.value.trim())) {
            ciError.textContent = "The ID must only have numbers (8 to 10 digits).";
            valid = false;
        } else {
            ciError.textContent = "";
        }

        // -------------------------------------------------------------
        // VALIDAR NOMBRE (solo letras, permite varios nombres con 1 espacio)
        // -------------------------------------------------------------
        const textRegex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ]+(\s[A-Za-zÁÉÍÓÚáéíóúÑñ]+)*$/;

        if (!textRegex.test(name.value.trim())) {
            nameError.textContent = "Just letters and a space between names";
            valid = false;
        } else {
            nameError.textContent = "";
        }

        // -------------------------------------------------------------
        // VALIDAR APELLIDO
        // -------------------------------------------------------------
        if (!textRegex.test(lastName.value.trim())) {
            lastNameError.textContent = "Only letters and a space between surnames.";
            valid = false;
        } else {
            lastNameError.textContent = "";
        }

        // -------------------------------------------------------------
        // VALIDAR FECHA NACIMIENTO (no futura y mínimo 16 años)
        // -------------------------------------------------------------
        const birth = new Date(dateBirth.value);
        const today = new Date();

        if (!dateBirth.value) {
            dateBirthError.textContent = "Please select a valid date";
            valid = false;
        } else {
            let age = today.getFullYear() - birth.getFullYear();
            const m = today.getMonth() - birth.getMonth();

            if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) {
                age--;
            }

            if (birth > today) {
                dateBirthError.textContent = "You cannot enter a future date.";
                valid = false;
            } else if (age < 16) {
                dateBirthError.textContent = "You must be at least 16 years old.";
                valid = false;
            } else {
                dateBirthError.textContent = "";
            }
        }

        // -------------------------------------------------------------
        // VALIDAR DIRECCIÓN (mínimo 5 caracteres)
        // -------------------------------------------------------------
        if (address.value.trim().length < 5) {
            addressError.textContent = "Enter a valid address.";
            valid = false;
        } else {
            addressError.textContent = "";
        }

        // -------------------------------------------------------------
        // VALIDAR TELÉFONO (exactamente 8 dígitos)
        // -------------------------------------------------------------
        const phoneRegex = /^[0-9]{8}$/;
        if (!phoneRegex.test(phone.value.trim())) {
            phoneError.textContent = "The phone number must have 8 digits.";
            valid = false;
        } else {
            phoneError.textContent = "";
        }

        // -------------------------------------------------------------
        // VALIDAR EMAIL
        // -------------------------------------------------------------
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email.value.trim())) {
            emailError.textContent = "Enter a valid email address.";
            valid = false;
        } else {
            emailError.textContent = "";
        }

        // -------------------------------------------------------------
        // VALIDAR ROL
        // -------------------------------------------------------------
        if (role.value === "") {
            roleError.textContent = "You must select a role.";
            valid = false;
        } else {
            roleError.textContent = "";
        }

        // -------------------------------------------------------------
        // EVITAR ENVÍO SI ALGO FALLA
        // -------------------------------------------------------------
        if (!valid) {
            e.preventDefault();
        }
    });

});
