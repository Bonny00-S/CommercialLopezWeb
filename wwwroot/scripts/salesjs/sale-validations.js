document.addEventListener("DOMContentLoaded", function () {

    console.log("VALIDACIONES SALE CREATE ACTIVAS - POS MODE");

    // Variables globales
    let cart = [];
    let selectedProducts = availableProducts;

    // Elementos del DOM
    const hasClientCheckbox = document.getElementById("hasClientCheckbox");
    const registeredClientSection = document.getElementById("registeredClientSection");
    const unregisteredClientSection = document.getElementById("unregisteredClientSection");
    const clientId = document.getElementById("ClientId");
    const clientName = document.getElementById("ClientName");
    const clientDocument = document.getElementById("ClientDocument");
    const clientPhone = document.getElementById("ClientPhone");
    const invoiceType = document.getElementById("InvoiceType");
    const paymentType = document.getElementById("PaymentType");
    const discountType = document.getElementById("DiscountType");
    const discountValue = document.getElementById("DiscountValue");
    const productsList = document.getElementById("productsList");
    const subtotalDisplay = document.getElementById("subtotalDisplay");
    const totalDisplay = document.getElementById("totalDisplay");
    const form = document.getElementById("saleForm");

    // Modal
    const productModal = document.getElementById("productModal");
    const btnAddProduct = document.getElementById("btnAddProduct");
    const btnCancelModal = document.getElementById("btnCancelModal");
    const btnConfirmProduct = document.getElementById("btnConfirmProduct");
    const modalProductId = document.getElementById("modalProductId");
    const modalQuantity = document.getElementById("modalQuantity");
    const modalSubtotal = document.getElementById("modalSubtotal");
    const productInfo = document.getElementById("productInfo");
    const productPrice = document.getElementById("productPrice");
    const productStock = document.getElementById("productStock");

    // Errores
    const invoiceTypeError = document.getElementById("invoiceTypeError");
    const paymentTypeError = document.getElementById("paymentTypeError");
    const productsError = document.getElementById("productsError");
    const discountError = document.getElementById("discountError");
    const modalQuantityError = document.getElementById("modalQuantityError");

    // ================================
    // TOGGLE CLIENTE REGISTRADO/NO REGISTRADO
    // ================================
    hasClientCheckbox.addEventListener("change", function () {
        if (this.checked) {
            registeredClientSection.classList.remove("hidden");
            unregisteredClientSection.classList.add("hidden");
            // Limpiar campos no registrados
            clientName.value = "";
            clientDocument.value = "";
            clientPhone.value = "";
        } else {
            registeredClientSection.classList.add("hidden");
            unregisteredClientSection.classList.remove("hidden");
            // Limpiar cliente registrado
            clientId.value = "";
        }
    });

    // ================================
    // TIPO DE DESCUENTO
    // ================================
    discountType.addEventListener("change", function () {
        if (this.value === "none") {
            discountValue.disabled = true;
            discountValue.value = 0;
        } else {
            discountValue.disabled = false;
            discountValue.value = 0;
        }
        calculateTotals();
    });

    // ================================
    // CARGAR PRODUCTOS EN MODAL
    // ================================
    function loadProductsInModal() {
        modalProductId.innerHTML = '<option value="">-- Seleccione un producto --</option>';
        
        selectedProducts.forEach(product => {
            const option = document.createElement("option");
            option.value = product.id;
            option.textContent = `${product.description} - Bs. ${product.price.toFixed(2)}`;
            option.dataset.price = product.price;
            option.dataset.stock = product.stock;
            modalProductId.appendChild(option);
        });
    }

    // ================================
    // MOSTRAR/OCULTAR MODAL
    // ================================
    btnAddProduct.addEventListener("click", function () {
        loadProductsInModal();
        productModal.classList.remove("hidden");
        modalProductId.value = "";
        modalQuantity.value = 1;
        productInfo.classList.add("hidden");
        modalQuantityError.textContent = "";
    });

    btnCancelModal.addEventListener("click", function () {
        productModal.classList.add("hidden");
    });

    // ================================
    // SELECCIONAR PRODUCTO EN MODAL
    // ================================
    modalProductId.addEventListener("change", function () {
        const selectedOption = this.options[this.selectedIndex];
        
        if (selectedOption.value) {
            const price = parseFloat(selectedOption.dataset.price);
            const stock = parseInt(selectedOption.dataset.stock);
            
            productPrice.textContent = price.toFixed(2);
            productStock.textContent = stock;
            productInfo.classList.remove("hidden");
            
            // Calcular subtotal
            calculateModalSubtotal();
        } else {
            productInfo.classList.add("hidden");
        }
    });

    // ================================
    // CALCULAR SUBTOTAL EN MODAL
    // ================================
    function calculateModalSubtotal() {
        const selectedOption = modalProductId.options[modalProductId.selectedIndex];
        if (selectedOption.value) {
            const price = parseFloat(selectedOption.dataset.price);
            const quantity = parseInt(modalQuantity.value) || 0;
            const subtotal = price * quantity;
            modalSubtotal.textContent = subtotal.toFixed(2);
        }
    }

    modalQuantity.addEventListener("input", calculateModalSubtotal);

    // ================================
    // VALIDAR Y AGREGAR PRODUCTO AL CARRITO
    // ================================
    btnConfirmProduct.addEventListener("click", function () {
        const productId = parseInt(modalProductId.value);
        const quantity = parseInt(modalQuantity.value);

        modalQuantityError.textContent = "";

        if (!productId) {
            alert("Seleccione un producto");
            return;
        }

        if (quantity <= 0) {
            modalQuantityError.textContent = "La cantidad debe ser mayor a 0.";
            return;
        }

        const selectedOption = modalProductId.options[modalProductId.selectedIndex];
        const stock = parseInt(selectedOption.dataset.stock);
        const price = parseFloat(selectedOption.dataset.price);

        if (quantity > stock) {
            modalQuantityError.textContent = `Stock insuficiente. Disponible: ${stock}`;
            return;
        }

        // Verificar si el producto ya está en el carrito
        const existingIndex = cart.findIndex(item => item.productId === productId);
        
        if (existingIndex >= 0) {
            // Actualizar cantidad
            cart[existingIndex].quantity += quantity;
            cart[existingIndex].subtotal = cart[existingIndex].quantity * cart[existingIndex].unitPrice;
        } else {
            // Agregar nuevo producto
            cart.push({
                productId: productId,
                description: selectedOption.textContent.split(" - ")[0],
                quantity: quantity,
                unitPrice: price,
                subtotal: price * quantity
            });
        }

        renderCart();
        calculateTotals();
        productModal.classList.add("hidden");
    });

    // ================================
    // RENDERIZAR CARRITO
    // ================================
    function renderCart() {
        if (cart.length === 0) {
            productsList.innerHTML = '<p class="text-gray-500 text-center py-4">No hay productos agregados</p>';
            return;
        }

        productsList.innerHTML = "";

        cart.forEach((item, index) => {
            const productRow = document.createElement("div");
            productRow.className = "flex items-center justify-between bg-gray-50 p-3 rounded-lg";
            productRow.innerHTML = `
                <div class="flex-1">
                    <p class="font-semibold text-gray-800">${item.description}</p>
                    <p class="text-sm text-gray-600">
                        ${item.quantity} x Bs. ${item.unitPrice.toFixed(2)} = Bs. ${item.subtotal.toFixed(2)}
                    </p>
                </div>
                <button type="button" onclick="removeFromCart(${index})" class="text-red-600 hover:text-red-800 ml-4">
                    <i class="fas fa-trash"></i>
                </button>
            `;
            productsList.appendChild(productRow);
        });
    }

    // ================================
    // ELIMINAR DEL CARRITO
    // ================================
    window.removeFromCart = function (index) {
        if (confirm("¿Eliminar este producto?")) {
            cart.splice(index, 1);
            renderCart();
            calculateTotals();
        }
    };

    // ================================
    // CALCULAR TOTALES
    // ================================
    function calculateTotals() {
        const subtotal = cart.reduce((sum, item) => sum + item.subtotal, 0);
        let discountAmount = 0;
        
        const dtype = discountType.value;
        const dvalue = parseFloat(discountValue.value) || 0;
        
        if (dtype === "percent") {
            discountAmount = subtotal * (dvalue / 100);
        } else if (dtype === "amount") {
            discountAmount = dvalue;
        }
        
        const total = subtotal - discountAmount;

        subtotalDisplay.textContent = `Bs. ${subtotal.toFixed(2)}`;
        totalDisplay.textContent = `Bs. ${total.toFixed(2)}`;
    }

    discountValue.addEventListener("input", calculateTotals);

    // ================================
    // VALIDACIONES
    // ================================
    function validateInvoiceType() {
        if (!invoiceType.value) {
            invoiceTypeError.textContent = "Debe seleccionar un tipo de comprobante.";
            return false;
        }
        invoiceTypeError.textContent = "";
        return true;
    }

    function validatePaymentType() {
        if (!paymentType.value) {
            paymentTypeError.textContent = "Debe seleccionar un método de pago.";
            return false;
        }
        paymentTypeError.textContent = "";
        return true;
    }

    function validateProducts() {
        if (cart.length === 0) {
            productsError.textContent = "Debe agregar al menos un producto.";
            return false;
        }
        productsError.textContent = "";
        return true;
    }

    function validateDiscount() {
        const dtype = discountType.value;
        const dvalue = parseFloat(discountValue.value) || 0;
        const subtotal = cart.reduce((sum, item) => sum + item.subtotal, 0);

        if (dvalue < 0) {
            discountError.textContent = "El descuento no puede ser negativo.";
            return false;
        }

        if (dtype === "percent" && dvalue > 100) {
            discountError.textContent = "El porcentaje no puede ser mayor a 100%.";
            return false;
        }

        if (dtype === "amount" && dvalue > subtotal) {
            discountError.textContent = "El descuento no puede ser mayor al subtotal.";
            return false;
        }

        discountError.textContent = "";
        return true;
    }

    // ================================
    // ENVIAR FORMULARIO
    // ================================
    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        const v1 = validateInvoiceType();
        const v2 = validatePaymentType();
        const v3 = validateProducts();
        const v4 = validateDiscount();

        if (!v1 || !v2 || !v3 || !v4) {
            console.log("❌ Formulario de venta inválido");
            return;
        }

        const subtotal = cart.reduce((sum, item) => sum + item.subtotal, 0);
        const dtype = discountType.value;
        const dvalue = parseFloat(discountValue.value) || 0;
        
        let discountPercent = 0;
        let discountAmount = 0;
        
        if (dtype === "percent") {
            discountPercent = dvalue;
        } else if (dtype === "amount") {
            discountAmount = dvalue;
        }

        const saleData = {
            clientId: hasClientCheckbox.checked && clientId.value ? parseInt(clientId.value) : null,
            clientName: !hasClientCheckbox.checked ? clientName.value : null,
            clientDocument: !hasClientCheckbox.checked ? clientDocument.value : null,
            clientPhone: !hasClientCheckbox.checked ? clientPhone.value : null,
            discountPercent: discountPercent,
            discountAmount: discountAmount,
            invoiceType: invoiceType.value,
            paymentType: paymentType.value,
            details: cart.map(item => ({
                productId: item.productId,
                quantity: item.quantity,
                unitPrice: item.unitPrice,
                subtotal: item.subtotal
            }))
        };

        console.log("Datos de venta:", saleData);

        try {
            const response = await fetch('/Sales/Create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify(saleData)
            });

            const result = await response.json();

            if (response.ok) {
                alert(result.message);
                window.location.href = '/Sales/Index';
            } else {
                alert(result.message || "Error al registrar la venta");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("Error al procesar la venta");
        }
    });

    // ================================
    // EVENTOS EN TIEMPO REAL
    // ================================
    invoiceType.addEventListener("change", validateInvoiceType);
    paymentType.addEventListener("change", validatePaymentType);
    discountValue.addEventListener("blur", validateDiscount);

});
