document.addEventListener("DOMContentLoaded", function () {

    console.log("EDIT VALIDATIONS ACTIVE");

    const description = document.getElementById("Description");
    const descriptionError = document.getElementById("descriptionError");

    const price = document.getElementById("Price");
    const priceError = document.getElementById("priceError");

    const stock = document.getElementById("Stock");
    const stockError = document.getElementById("stockError");

    const fileInput = document.getElementById("imageFileInput");
    const preview = document.getElementById("imagePreview");

    const form = document.querySelector("form");

    // ================================
    // VALIDATION: DESCRIPTION
    // ================================
    function validateDescription() {
        let text = description.value.toUpperCase().replace(/\s+/g, " ").trimStart();
        description.value = text;

        if (text.length === 0) {
            descriptionError.textContent = "Description is required.";
            return false;
        }
        if (text.length < 4) {
            descriptionError.textContent = "Must be at least 4 characters.";
            return false;
        }
        if (text.length > 50) {
            descriptionError.textContent = "Maximum allowed is 50 characters.";
            return false;
        }

        descriptionError.textContent = "";
        return true;
    }

    // ================================
    // VALIDATION: PRICE
    // ================================
    function validatePrice() {
        let value = price.value.replace(/[^0-9.,]/g, "").replace(",", ".");
        const parts = value.split(".");

        if (parts.length > 2)
            value = parts[0] + "." + parts[1];

        if (value.startsWith("."))
            value = "";

        if (parts.length === 2)
            parts[1] = parts[1].slice(0, 2);

        price.value = parts.join(".");

        if (value === "") {
            priceError.textContent = "Price is required.";
            return false;
        }
        if (isNaN(Number(value))) {
            priceError.textContent = "Enter a valid number.";
            return false;
        }
        if (Number(value) <= 0) {
            priceError.textContent = "Must be greater than 0.";
            return false;
        }

        priceError.textContent = "";
        return true;
    }

    // ================================
    // VALIDATION: STOCK
    // ================================
    function validateStock() {
        let value = stock.value.replace(/[^0-9]/g, "");
        stock.value = value;

        if (value.length === 0) {
            stockError.textContent = "Stock is required.";
            return false;
        }
        if (Number(value) < 0) {
            stockError.textContent = "Cannot be negative.";
            return false;
        }

        stockError.textContent = "";
        return true;
    }

    // ================================
    // REAL-TIME EVENTS
    // ================================
    description.addEventListener("input", validateDescription);
    price.addEventListener("input", validatePrice);
    stock.addEventListener("input", validateStock);

    description.addEventListener("blur", validateDescription);
    price.addEventListener("blur", validatePrice);
    stock.addEventListener("blur", validateStock);

    // ================================
    // SUBMIT VALIDATION
    // ================================
    form.addEventListener("submit", (e) => {
        if (!validateDescription() || !validatePrice() || !validateStock()) {
            e.preventDefault();
        }
    });

    // ================================
    // IMAGE PREVIEW ON CHANGE
    // ================================
    fileInput.addEventListener("change", function () {
        const file = this.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = e => {
            preview.src = e.target.result;
            preview.classList.remove("hidden");
        };
        reader.readAsDataURL(file);
    });

});
