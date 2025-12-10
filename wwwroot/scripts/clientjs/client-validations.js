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
            documentNumberError.textContent = "El CI/NIT es obligatorio.";
            return false;
        }

        // Validar caracteres peligrosos (seguridad)
        const dangerousChars = /[<>'"\/\\;=()]/;
        if (dangerousChars.test(text)) {
            documentNumberError.textContent = "Contiene caracteres no permitidos.";
            return false;
        }

        // Validar solo números, letras y guiones
        const regex = /^[0-9A-Za-z-]+$/;
        if (!regex.test(text)) {
            documentNumberError.textContent = "Solo se permiten números, letras y guiones.";
            return false;
        }

        if (text.length < 6) {
            documentNumberError.textContent = "Debe tener mínimo 6 caracteres.";
            return false;
        }

        if (text.length > 15) {
            documentNumberError.textContent = "Máximo 15 caracteres permitidos.";
            return false;
        }

        // No puede empezar con cero (solo para CI numérico)
        if (/^0/.test(text)) {
            documentNumberError.textContent = "El CI/NIT no puede empezar con cero.";
            return false;
        }

        documentNumberError.textContent = "";
        return true;
    }

    // ================================
    // VALIDACIÓN RAZÓN SOCIAL
    // ================================
    function validateCompanyName() {
        let text = companyName.value.replace(/\s+/g, " ").trimStart();
        companyName.value = text;

        if (text.length === 0) {
            companyNameError.textContent = "La Razón Social es obligatoria.";
            return false;
        }

        // Validar secuencias peligrosas (seguridad)
        const dangerousPatterns = /<script|<iframe|javascript:|onerror=|onclick=|DROP TABLE|INSERT INTO|DELETE FROM|SELECT.*FROM/i;
        if (dangerousPatterns.test(text)) {
            companyNameError.textContent = "Contiene contenido no permitido.";
            return false;
        }

        // Rechazar caracteres especiales peligrosos
        const dangerousChars = /[<>'"\/\\;=]/;
        if (dangerousChars.test(text)) {
            companyNameError.textContent = "Contiene caracteres no permitidos.";
            return false;
        }

        if (text.length < 3) {
            companyNameError.textContent = "Debe tener mínimo 3 caracteres.";
            return false;
        }

        if (text.length > 150) {
            companyNameError.textContent = "Máximo 150 caracteres permitidos.";
            return false;
        }

        // Validar caracteres permitidos (letras, números, acentos, espacios, ampersand, puntos, comas, guiones)
        const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9&.,\s-]+$/;
        if (!regex.test(text)) {
            companyNameError.textContent = "Contiene caracteres no válidos.";
            return false;
        }

        // No aceptar solo números
        if (/^[0-9\s.,&-]+$/.test(text)) {
            companyNameError.textContent = "Debe contener al menos letras.";
            return false;
        }

        // No aceptar solo símbolos
        if (/^[.,&\s-]+$/.test(text)) {
            companyNameError.textContent = "No puede contener solo símbolos.";
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
            phoneError.textContent = "Máximo 20 caracteres permitidos.";
            return false;
        }

        // Validar solo números y símbolos +, -, (), espacios
        const regex = /^[0-9+\-\s()]+$/;
        if (!regex.test(text)) {
            phoneError.textContent = "Solo números y símbolos +, -, (), espacios.";
            return false;
        }

        // Validar cantidad de dígitos
        const phoneDigits = text.replace(/\D/g, '');
        if (phoneDigits.length > 0 && (phoneDigits.length < 7 || phoneDigits.length > 15)) {
            phoneError.textContent = "Debe contener entre 7 y 15 dígitos.";
            return false;
        }

        phoneError.textContent = "";
        return true;
    }

    // ================================
    // VALIDACIÓN DIRECCIÓN (OPCIONAL)
    // ================================
    function validateAddress() {
        let text = address.value.replace(/\s+/g, " ").trimStart();
        address.value = text;

        // Si está vacío, es válido (campo opcional)
        if (text.length === 0) {
            addressError.textContent = "";
            return true;
        }

        if (text.length > 200) {
            addressError.textContent = "Máximo 200 caracteres permitidos.";
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
            console.log("❌ Formulario de cliente inválido");
        }
    });

});
