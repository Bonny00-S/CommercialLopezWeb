document.addEventListener("DOMContentLoaded", () => {

    // Automatic uppercase except email
    document.querySelectorAll("input[type='text']:not(#Email)").forEach(input => {
        input.addEventListener("input", () => {
            input.value = input.value.toUpperCase();
        });
    });

    const form = document.getElementById("personForm");

    const fields = {
        ci: document.getElementById("ci"),
        name: document.getElementById("Name"),
        lastName: document.getElementById("LastName"),
        dateBirth: document.getElementById("DateBirth"),
        address: document.getElementById("Address"),
        phone: document.getElementById("Phone"),
        email: document.getElementById("Email"),
        role: document.getElementById("Role")
    };

    const errors = {
        ci: document.getElementById("ciError"),
        name: document.getElementById("NameError"),
        lastName: document.getElementById("LastNameError"),
        dateBirth: document.getElementById("DateBirthError"),
        address: document.getElementById("AddressError"),
        phone: document.getElementById("PhoneError"),
        email: document.getElementById("EmailError"),
        role: document.getElementById("RoleError")
    };

    const validators = {
        ci: v => /^[0-9]{8,10}$/.test(v.trim()),
        name: v => /^[A-Za-z ]+$/.test(v.trim()),
        lastName: v => /^[A-Za-z ]+$/.test(v.trim()),
        dateBirth: v => {
            if (!v) return false;
            const birth = new Date(v);
            const today = new Date();
            if (birth > today) return false;

            let age = today.getFullYear() - birth.getFullYear();
            const m = today.getMonth() - birth.getMonth();
            if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) age--;

            return age >= 16;
        },
        address: v => v.trim().length >= 5,
        phone: v => /^[0-9]{8}$/.test(v.trim()),
        email: v => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v.trim()),
        role: v => v !== ""
    };

    const messages = {
        ci: "The ID must contain only numbers (8 to 10 digits).",
        name: "Name must contain only letters.",
        lastName: "Lastname must contain only letters.",
        dateBirth: "You must be at least 16 years old.",
        address: "Address must contain at least 5 characters.",
        phone: "Phone number must have exactly 8 digits.",
        email: "Enter a valid email address.",
        role: "Select a role."
    };

    function validateOne(field) {
        const value = fields[field].value;
        const isValid = validators[field](value);

        if (!isValid) {
            errors[field].textContent = messages[field];
            return false;
        } else {
            errors[field].textContent = "";
            return true;
        }
    }

    // INPUT (real time)
    Object.keys(fields).forEach(f => {
        fields[f].addEventListener("input", () => validateOne(f));
    });

    // BLUR (leaves input)
    Object.keys(fields).forEach(f => {
        fields[f].addEventListener("blur", () => validateOne(f));
    });

    // SUBMIT
    form.addEventListener("submit", (e) => {
        let valid = true;

        Object.keys(fields).forEach(f => {
            if (!validateOne(f)) valid = false;
        });

        if (!valid) e.preventDefault();
    });
});
