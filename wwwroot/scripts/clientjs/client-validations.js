document.addEventListener("DOMContentLoaded", function () {

    console.log("VALIDACIONES CLIENT CREATE ACTIVAS");

    // Obtener campos
    const documentNumber = document.getElementById("DocumentNumber");
    const documentNumberError = document.getElementById("documentNumberError");

    const companyName = document.getElementById("CompanyName");
    const companyNameError = document.getElementById("companyNameError");

    const phone = document.getElementById("Phone");
    const phoneError = document.getElementById("phoneError");

    const address = document.getElementById("Address");
    const addressError = document.getElementById("addressError");

    const form = document.querySelector("form");

    // ================================
    // VALIDACIÓN CI/NIT
    // ================================
    function validateDocumentNumber() {
        let text = documentNumber.value.trim();
        documentNumber.value = text;

        if (text.length === 0) {
            documentNumberError.textContent = "The CI/NIT is mandatory.";
            return false;
        }

        // Validar caracteres peligrosos (seguridad)
        const dangerousChars = /[<>'"\/ \\;=()]/;
        if (dangerousChars.test(text)) {
            documentNumberError.textContent = "Contains prohibited characters.";
            return false;
        }

        // Validar solo números, letras y guiones
        const regex = /^[0-9A-Za-z-]+$/;
        if (!regex.test(text)) {
            documentNumberError.textContent = "Only numbers, letters, and hyphens are allowed.";
            return false;
        }

        if (text.length < 6) {
            documentNumberError.textContent = "It must have a minimum of 6 characters.";
            return false;
        }

        if (text.length > 15) {
            documentNumberError.textContent = "Maximum 15 characters allowed.";
            return false;
        }

        // No puede empezar con cero (solo para CI numérico)
        if (/^0/.test(text)) {
            documentNumberError.textContent = "The CI/NIT cannot start with zero.";
            return false;
        }

        documentNumberError.textContent = "";
        return true;
    }

    // ================================
    // VALIDACIÓN REASON SOCIAL
    // ================================
    function validateCompanyName() {
        let text = companyName.value.trim();
        companyName.value = text;

        if (text.length === 0) {
            companyNameError.textContent = "The Reason Social is mandatory.";
            return false;
        }

        if (text.length < 2 || text.length > 100) {
            companyNameError.textContent = "The full name must be between 2 and 100 characters.";
            return false;
        }

        // Verificar que no tenga dobles espacios
        if (text.includes("  ")) {
            companyNameError.textContent = "Multiple spaces in a row are not allowed.";
            return false;
        }

        // Solo letras, espacios y acentos
        const regex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$/;
        if (!regex.test(text)) {
            companyNameError.textContent = "The name can only contain letters and spaces.";
            return false;
        }

        companyNameError.textContent = "";
        return true;
    }

    // ================================
    // VALIDACIÓN TELÉFONO (OPCIONAL)
    // ================================
    function validatePhone() {
        let text = phone.value.trim();
        phone.value = text;

        // Si está vacío, es válido (campo opcional)
        if (text.length === 0) {
            phoneError.textContent = "";
            return true;
        }

        if (text.length > 20) {
            phoneError.textContent = "Maximum 20 characters allowed.";
            return false;
        }

        // Validar solo números y símbolos +, -, (), espacios
        const regex = /^[0-9+\-\s()]+$/;
        if (!regex.test(text)) {
            phoneError.textContent = "Only numbers and symbols +, -, (), spaces.";
            return false;
        }

        // Validar cantidad de dígitos
        const phoneDigits = text.replace(/\D/g, '');
        if (phoneDigits.length > 0 && (phoneDigits.length < 7 || phoneDigits.length > 15)) {
            phoneError.textContent = "It must contain between 7 and 15 digits.";
            return false;
        }

        phoneError.textContent = "";
        return true;
    }

    // ================================
    // VALIDACIÓN DIRECCIÓN (OPCIONAL)
    // ================================
    function validateAddress() {
        let text = address.value.replace(/\s+/g, " ").trim();
        address.value = text;

        // Si está vacío, es válido (campo opcional)
        if (text.length === 0) {
            addressError.textContent = "";
            return true;
        }

        if (text.length < 5 || text.length > 200) {
            addressError.textContent = "La dirección debe tener entre 5 y 200 caracteres.";
            return false;
        }

        // Validar caracteres permitidos: letras, números, espacios y símbolos comunes
        const regex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9 #.,\-\/ºº°()]+$/;
        if (!regex.test(text)) {
            addressError.textContent = "La dirección contiene caracteres no permitidos.";
            return false;
        }

        addressError.textContent = "";
        return true;
    }

    // ================================
    // EVENTOS EN TIEMPO REAL (input)
    // ================================
    documentNumber.addEventListener("input", validateDocumentNumber);
    companyName.addEventListener("input", validateCompanyName);
    phone.addEventListener("input", validatePhone);
    address.addEventListener("input", validateAddress);

    // ================================
    // EVENTOS al SALIR DEL CAMPO (blur)
    // ================================
    documentNumber.addEventListener("blur", validateDocumentNumber);
    companyName.addEventListener("blur", validateCompanyName);
    phone.addEventListener("blur", validatePhone);
    address.addEventListener("blur", validateAddress);

    // ================================
    // VALIDACIÓN AL HACER SUBMIT
    // ================================
    form.addEventListener("submit", (e) => {
        let v1 = validateDocumentNumber();
        let v2 = validateCompanyName();
        let v3 = validatePhone();
        let v4 = validateAddress();

        if (!v1 || !v2 || !v3 || !v4) {
            e.preventDefault();
            console.log("❌ Invalid customer form");
        }
    });

});
