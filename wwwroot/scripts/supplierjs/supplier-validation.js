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
    // 🔠 MAYÚSCULAS Y SIN DOBLES ESPACIOS
    // =======================================
    ["RazonSocial", "Address"].forEach(key => {
        fields[key].addEventListener("input", () => {
            fields[key].value = fields[key].value.replace(/\s{2,}/g, " ");
            fields[key].value = fields[key].value.toUpperCase();
        });
    });

    // =======================================
    // 📱 TELÉFONO POR PAÍS (SOLO BOLIVIA Y CHILE)
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

        errors.Phone.textContent =
            fields.Phone.value.length < maxLen
                ? `Phone must have ${maxLen} digits`
                : "";
    }

    fields.Phone.addEventListener("input", validatePhone);

    // =======================================
    // 🧾 NIT POR PAÍS (SOLO BOLIVIA Y CHILE)
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

        errors.NIT.textContent =
            fields.NIT.value.length < maxLen
                ? `NIT for ${country} must have ${maxLen} digits`
                : "";
    }

    fields.NIT.addEventListener("input", validateNIT);

    // =======================================
    // 📝 PLACEHOLDERS POR PAÍS
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

    fields.Country.addEventListener("change", () => {
        updatePlaceholders();
        validatePhone();
        validateNIT();
        loadCities();
        selectCityOnEdit();
    });

    // =======================================
    // 🏙️ CIUDADES SOLO BOLIVIA Y CHILE
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

    function selectCityOnEdit() {
        const selectedCity = fields.City.getAttribute("data-selected");
        if (!selectedCity) return;

        for (let option of fields.City.options) {
            if (option.value === selectedCity) {
                option.selected = true;
                break;
            }
        }
    }

    // Ejecutar al cargar la página
    updatePlaceholders();
    loadCities();
    selectCityOnEdit();

    // =======================================
    // ✔ VALIDACIÓN FINAL
    // =======================================
    const form = document.querySelector("form");

    form.addEventListener("submit", (e) => {
        let valid = true;

        Object.keys(fields).forEach(key => {
            if (!fields[key].value || fields[key].value.trim() === "") {
                errors[key].textContent = `${key} is required`;
                valid = false;
            }
        });

        validatePhone();
        validateNIT();

        if (!valid) e.preventDefault();
    });

});
