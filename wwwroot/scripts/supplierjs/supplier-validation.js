document.addEventListener("DOMContentLoaded", () => {

    const fields = {
        RazonSocial: document.getElementById("RazonSocial"),
        NIT: document.getElementById("NIT"),
        Phone: document.getElementById("Phone"),
        Email: document.getElementById("Email"),
        Address: document.getElementById("Address"),
        Country: document.getElementById("Country"),
        City: document.getElementById("City")
    };

    const errors = {
        RazonSocial: document.getElementById("RazonSocialError"),
        NIT: document.getElementById("NitError"),
        Phone: document.getElementById("PhoneError"),
        Email: document.getElementById("EmailError"),
        Address: document.getElementById("AddressError"),
        Country: document.getElementById("CountryError"),
        City: document.getElementById("CityError")
    };

    // =======================================
    // 🔤 MAYÚSCULAS Y SIN DOBLES ESPACIOS
    // =======================================
    ["RazonSocial", "Address"].forEach(key => {
        fields[key].addEventListener("input", () => {
            fields[key].value = fields[key].value.replace(/\s{2,}/g, " ");
            fields[key].value = fields[key].value.toUpperCase();
            validateRequired(key);
        });
        fields[key].addEventListener("blur", () => validateRequired(key));
    });

    // =======================================
    // 📌 VALIDACIÓN CAMPOS REQUERIDOS
    // =======================================
    function validateRequired(key) {
        if (!fields[key].value || fields[key].value.trim() === "") {
            errors[key].textContent = `${key} is required`;
            return false;
        }
        errors[key].textContent = "";
        return true;
    }

    // Aplicar validación en tiempo real
    Object.keys(fields).forEach(key => {
        fields[key].addEventListener("input", () => validateRequired(key));
        fields[key].addEventListener("blur", () => validateRequired(key));
    });

    // =======================================
    // 📱 VALIDACIÓN TELÉFONO SEGÚN PAÍS
    // =======================================
    const phoneRules = {
        "Bolivia": 8,
        "Chile": 9
    };

    function validatePhone() {
        const country = fields.Country.value;
        const maxLen = phoneRules[country] || 15;

        fields.Phone.value = fields.Phone.value.replace(/\D/g, "");
        if (fields.Phone.value.length > maxLen)
            fields.Phone.value = fields.Phone.value.slice(0, maxLen);

        if (fields.Phone.value.length < maxLen) {
            errors.Phone.textContent = `Phone must have ${maxLen} digits`;
            return false;
        }

        errors.Phone.textContent = "";
        return true;
    }

    fields.Phone.addEventListener("input", validatePhone);
    fields.Phone.addEventListener("blur", validatePhone);

    // =======================================
    // 🧾 VALIDACIÓN NIT SEGÚN PAÍS
    // =======================================
    const nitRules = {
        "Bolivia": 11,
        "Chile": 9
    };

    function validateNIT() {
        const country = fields.Country.value;
        const maxLen = nitRules[country] || 15;

        fields.NIT.value = fields.NIT.value.replace(/\D/g, "");
        if (fields.NIT.value.length > maxLen)
            fields.NIT.value = fields.NIT.value.slice(0, maxLen);

        if (fields.NIT.value.length < maxLen) {
            errors.NIT.textContent = `NIT for ${country} must have ${maxLen} digits`;
            return false;
        }

        errors.NIT.textContent = "";
        return true;
    }

    fields.NIT.addEventListener("input", validateNIT);
    fields.NIT.addEventListener("blur", validateNIT);

    // =======================================
    // 📝 PLACEHOLDERS DINÁMICOS POR PAÍS
    // =======================================
    const phonePlaceholders = {
        "Bolivia": "Ej: 71234567",
        "Chile": "Ej: 912345678"
    };

    const nitPlaceholders = {
        "Bolivia": "Ej: 12345678901",
        "Chile": "Ej: 123456789"
    };

    function updatePlaceholders() {
        const country = fields.Country.value;

        fields.Phone.placeholder = phonePlaceholders[country] || "Phone number";
        fields.NIT.placeholder = nitPlaceholders[country] || "NIT";
    }

    // =======================================
    // 🏙️ CARGAR CIUDADES POR PAÍS
    // =======================================
    const citiesByCountry = {
        "Bolivia": ["LA PAZ", "COCHABAMBA", "SANTA CRUZ", "SUCRE", "ORURO", "POTOSI", "TARIJA", "BENI", "PANDO"],
        "Chile": ["SANTIAGO", "VALPARAISO", "CONCEPCION", "LA SERENA", "ANTOFAGASTA", "RANCAGUA"]
    };

    function loadCities() {
        const selected = fields.Country.value;
        fields.City.innerHTML = `<option value="">-- SELECT A CITY --</option>`;

        if (citiesByCountry[selected]) {
            citiesByCountry[selected].forEach(city => {
                fields.City.innerHTML += `<option value="${city}">${city}</option>`;
            });
        }
    }

    // Validación al cambiar país
    fields.Country.addEventListener("change", () => {
        updatePlaceholders();
        validatePhone();
        validateNIT();
        validateRequired("Country");
        loadCities();
    });

    fields.Country.addEventListener("blur", () => validateRequired("Country"));
    fields.City.addEventListener("blur", () => validateRequired("City"));

    // Inicialización
    updatePlaceholders();
    loadCities();

    // =======================================
    // ✔ VALIDACIÓN FINAL AL SUBMIT
    // =======================================
    const form = document.querySelector("form");

    form.addEventListener("submit", (e) => {
        let valid = true;

        Object.keys(fields).forEach(key => {
            if (!validateRequired(key)) valid = false;
        });

        if (!validatePhone()) valid = false;
        if (!validateNIT()) valid = false;

        if (!valid) e.preventDefault();
    });

});
