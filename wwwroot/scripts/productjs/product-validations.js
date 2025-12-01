document.addEventListener("DOMContentLoaded", function () {

    console.log("VALIDACIONES CREATE ACTIVAS");

    // Obtener campos
    const description = document.getElementById("Description");
    const descriptionError = document.getElementById("descriptionError");

    const price = document.getElementById("Price");
    const priceError = document.getElementById("priceError");

    const stock = document.getElementById("Stock");
    const stockError = document.getElementById("stockError");

    const fileInput = document.getElementById("imageFileInput");
    const imageError = document.getElementById("imageError");

    const preview = document.getElementById("imagePreview");
    const form = document.querySelector("form");

    // ================================
    // VALIDACIÓN DESCRIPCIÓN
    // ================================
    function validateDescription() {
        let text = description.value.toUpperCase().replace(/\s+/g, " ").trimStart();
        description.value = text;

        if (text.length === 0) {
            descriptionError.textContent = "La descripción es obligatoria.";
            return false;
        }
        if (text.length < 4) {
            descriptionError.textContent = "Debe tener mínimo 4 caracteres.";
            return false;
        }
        if (text.length > 50) {
            descriptionError.textContent = "Máximo 50 caracteres permitidos.";
            return false;
        }

        descriptionError.textContent = "";
        return true;
    }

    // ================================
    // VALIDACIÓN PRECIO
    // ================================
    function validatePrice() {
        let value = price.value.replace(/[^0-9.,]/g, "").replace(",", ".");
        const parts = value.split(".");

        if (parts.length > 2)
            value = parts[0] + "." + parts[1];

        if (value.startsWith("."))
            value = "";

        if (parts.length === 2)
            value = parts[0] + "." + parts[1].slice(0, 2);

        price.value = value;

        if (value === "") {
            priceError.textContent = "El precio es obligatorio.";
            return false;
        }

        if (isNaN(Number(value))) {
            priceError.textContent = "Ingrese un número válido.";
            return false;
        }

        if (Number(value) <= 0) {
            priceError.textContent = "Debe ser mayor a 0.";
            return false;
        }

        priceError.textContent = "";
        return true;
    }

    // ================================
    // VALIDACIÓN STOCK
    // ================================
    function validateStock() {
        let value = stock.value.replace(/[^0-9]/g, "");
        stock.value = value;

        if (value === "") {
            stockError.textContent = "El stock es obligatorio.";
            return false;
        }

        if (Number(value) < 0) {
            stockError.textContent = "No puede ser negativo.";
            return false;
        }

        stockError.textContent = "";
        return true;
    }

    // ================================
    // VALIDACIÓN IMAGEN (OBLIGATORIA)
    // ================================
    function validateImage() {
        if (fileInput.files.length === 0) {
            imageError.textContent = "Debe seleccionar una imagen.";
            return false;
        }

        const file = fileInput.files[0];
        const validTypes = ["image/jpeg", "image/png", "image/webp"];

        if (!validTypes.includes(file.type)) {
            imageError.textContent = "Formato inválido (solo JPG, PNG, WEBP).";
            return false;
        }

        if (file.size > 3 * 1024 * 1024) { // 3MB
            imageError.textContent = "La imagen no debe superar los 3MB.";
            return false;
        }

        imageError.textContent = "";
        return true;
    }

    // ================================
    // PREVIEW DE IMAGEN
    // ================================
    fileInput.addEventListener("change", function () {
        validateImage(); // marca error si corresponde

        const file = this.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = e => {
            preview.src = e.target.result;
            preview.classList.remove("hidden");
        };
        reader.readAsDataURL(file);
    });

    // ================================
    // EVENTOS EN TIEMPO REAL (input)
    // ================================
    description.addEventListener("input", validateDescription);
    price.addEventListener("input", validatePrice);
    stock.addEventListener("input", validateStock);

    // ================================
    // EVENTOS al SALIR DEL CAMPO (blur)
    // ================================
    description.addEventListener("blur", validateDescription);
    price.addEventListener("blur", validatePrice);
    stock.addEventListener("blur", validateStock);
    fileInput.addEventListener("blur", validateImage);

    // ================================
    // VALIDACIÓN AL HACER SUBMIT
    // ================================
    form.addEventListener("submit", (e) => {
        let v1 = validateDescription();
        let v2 = validatePrice();
        let v3 = validateStock();
        let v4 = validateImage();

        if (!v1 || !v2 || !v3 || !v4) {
            e.preventDefault();
            console.log("❌ Formulario inválido");
        }
    });

});
