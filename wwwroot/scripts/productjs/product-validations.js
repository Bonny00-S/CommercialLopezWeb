document.addEventListener("DOMContentLoaded", function () {

    console.log("CREATE VALIDATIONS ACTIVE");

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
            descriptionError.textContent = "Minimum length is 4 characters.";
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
            value = parts[0] + "." + parts[1].slice(0, 2);

        price.value = value;

        if (value === "") {
            priceError.textContent = "Price is required.";
            return false;
        }

        if (isNaN(Number(value))) {
            priceError.textContent = "Enter a valid number.";
            return false;
        }

        if (Number(value) <= 0) {
            priceError.textContent = "Value must be greater than 0.";
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

        if (value === "") {
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
    // VALIDATION: IMAGE
    // ================================
    function validateImage() {
        if (fileInput.files.length === 0) {
            imageError.textContent = "Image is required.";
            return false;
        }

        const file = fileInput.files[0];
        const validTypes = ["image/jpeg", "image/png", "image/webp"];

        if (!validTypes.includes(file.type)) {
            imageError.textContent = "Invalid format (only JPG, PNG, WEBP).";
            return false;
        }

        if (file.size > 3 * 1024 * 1024) {
            imageError.textContent = "Image must not exceed 3MB.";
            return false;
        }

        imageError.textContent = "";
        return true;
    }

    // ================================
    // IMAGE PREVIEW
    // ================================
    fileInput.addEventListener("change", function () {
        validateImage();

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
    // REAL-TIME EVENTS
    // ================================
    description.addEventListener("input", validateDescription);
    price.addEventListener("input", validatePrice);
    stock.addEventListener("input", validateStock);

    // ================================
    // BLUR EVENTS
    // ================================
    description.addEventListener("blur", validateDescription);
    price.addEventListener("blur", validatePrice);
    stock.addEventListener("blur", validateStock);
    fileInput.addEventListener("blur", validateImage);

    // ================================
    // SUBMIT VALIDATION
    // ================================
    form.addEventListener("submit", (e) => {
        let v1 = validateDescription();
        let v2 = validatePrice();
        let v3 = validateStock();
        let v4 = validateImage();

        if (!v1 || !v2 || !v3 || !v4) {
            e.preventDefault();
            console.log("❌ Invalid form");
        }
    });

});
