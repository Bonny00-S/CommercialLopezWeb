// Variables globales
let cart = [];
let selectedProduct = null;

// Función para mostrar notificaciones
function showNotification(message, type = 'success') {
    const notification = document.getElementById('notification');
    const notificationMessage = document.getElementById('notificationMessage');
    const notificationIcon = document.getElementById('notificationIcon');
    
    notificationMessage.textContent = message;
    
    if (type === 'success') {
        notification.className = 'fixed top-4 right-4 z-50 px-6 py-4 rounded-lg shadow-lg max-w-md bg-green-100 border border-green-400';
        notificationIcon.innerHTML = '<svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>';
    } else {
        notification.className = 'fixed top-4 right-4 z-50 px-6 py-4 rounded-lg shadow-lg max-w-md bg-red-100 border border-red-400';
        notificationIcon.innerHTML = '<svg class="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>';
    }
    
    notification.classList.remove('hidden');
    
    setTimeout(() => {
        notification.classList.add('hidden');
    }, 4000);
}

function closeNotification() {
    document.getElementById('notification').classList.add('hidden');
}

// Elementos del DOM
const saleForm = document.getElementById('saleForm');
const searchDocumentInput = document.getElementById('SearchDocument');
const clientSearchResults = document.getElementById('clientSearchResults');
const btnClearClient = document.getElementById('btnClearClient');
const clientIdInput = document.getElementById('ClientId');
const clientNameInput = document.getElementById('ClientName');
const searchClientError = document.getElementById('searchClientError');
const searchClientSuccess = document.getElementById('searchClientSuccess');
const clientError = document.getElementById('clientError');
const paymentTypeSelect = document.getElementById('PaymentType');
const discountInput = document.getElementById('Discount');
const btnSaveSale = document.getElementById('btnSaveSale');
const productsList = document.getElementById('productsList');
const subtotalDisplay = document.getElementById('subtotalDisplay');
const totalDisplay = document.getElementById('totalDisplay');

// Elementos de búsqueda de productos
const searchProductInput = document.getElementById('SearchProduct');
const productSearchResults = document.getElementById('productSearchResults');
const productQuantityInput = document.getElementById('ProductQuantity');
const selectedProductInfo = document.getElementById('selectedProductInfo');
const selectedProductName = document.getElementById('selectedProductName');
const selectedProductPrice = document.getElementById('selectedProductPrice');
const selectedProductStock = document.getElementById('selectedProductStock');
const selectedProductSubtotal = document.getElementById('selectedProductSubtotal');
const btnAddProductToCart = document.getElementById('btnAddProductToCart');
const productError = document.getElementById('productError');

// Buscar productos mientras escribe
searchProductInput.addEventListener('input', function() {
    const searchTerm = this.value.trim().toLowerCase();
    
    if (searchTerm.length < 2) {
        productSearchResults.classList.add('hidden');
        productSearchResults.innerHTML = '';
        return;
    }
    
    const filteredProducts = availableProducts.filter(p => 
        p.Description.toLowerCase().includes(searchTerm)
    );
    
    if (filteredProducts.length === 0) {
        productSearchResults.innerHTML = '<div class="p-3 text-gray-500 text-sm">No se encontraron productos</div>';
        productSearchResults.classList.remove('hidden');
        return;
    }
    
    productSearchResults.innerHTML = '';
    filteredProducts.forEach(product => {
        const div = document.createElement('div');
        div.className = 'p-3 hover:bg-gray-100 cursor-pointer border-b border-gray-200';
        div.innerHTML = `
            <div class="font-semibold">${product.Description}</div>
            <div class="text-sm text-gray-600">Precio: Bs. ${product.Price.toFixed(2)} | Stock: ${product.Stock}</div>
        `;
        div.addEventListener('click', function() {
            selectProduct(product);
        });
        productSearchResults.appendChild(div);
    });
    
    productSearchResults.classList.remove('hidden');
});

// Seleccionar producto
function selectProduct(product) {
    selectedProduct = product;
    searchProductInput.value = product.Description;
    productSearchResults.classList.add('hidden');
    
    selectedProductName.textContent = product.Description;
    selectedProductPrice.textContent = `Bs. ${product.Price.toFixed(2)}`;
    selectedProductStock.textContent = product.Stock;
    
    updateProductSubtotal();
    selectedProductInfo.classList.remove('hidden');
    productQuantityInput.focus();
    productError.textContent = '';
}

// Actualizar subtotal del producto seleccionado
productQuantityInput.addEventListener('input', updateProductSubtotal);

function updateProductSubtotal() {
    if (!selectedProduct) return;
    
    const quantity = parseInt(productQuantityInput.value) || 0;
    const subtotal = selectedProduct.Price * quantity;
    selectedProductSubtotal.textContent = `Bs. ${subtotal.toFixed(2)}`;
}

// Agregar producto al carrito
btnAddProductToCart.addEventListener('click', function() {
    if (!selectedProduct) {
        productError.textContent = 'You must search and select a product';
        return;
    }
    
    const quantity = parseInt(productQuantityInput.value);
    
    if (!quantity || quantity <= 0) {
        productError.textContent = 'Quantity must be greater than 0';
        return;
    }
    
    if (quantity > selectedProduct.Stock) {
        productError.textContent = `The quantity exceeds the available stock. (${selectedProduct.Stock})`;
        return;
    }
    
    // Verificar si el producto ya está en el carrito
    const existingProduct = cart.find(item => item.ProductId === selectedProduct.Id);
    if (existingProduct) {
        existingProduct.Quantity += quantity;
        existingProduct.Subtotal = existingProduct.Quantity * existingProduct.UnitPrice;
    } else {
        cart.push({
            ProductId: selectedProduct.Id,
            Description: selectedProduct.Description,
            Quantity: quantity,
            UnitPrice: selectedProduct.Price,
            Subtotal: quantity * selectedProduct.Price,
            MaxStock: selectedProduct.Stock
        });
    }
    
    // Limpiar selección
    searchProductInput.value = '';
    productQuantityInput.value = 1;
    selectedProductInfo.classList.add('hidden');
    selectedProduct = null;
    productError.textContent = '';
    
    renderCart();
    calculateTotals();
    searchProductInput.focus();
});

// Buscar cliente mientras escribe
searchDocumentInput.addEventListener('input', async function() {
    const searchTerm = this.value.trim();
    
    searchClientError.textContent = '';
    searchClientSuccess.textContent = '';
    
    if (searchTerm.length < 2) {
        clientSearchResults.classList.add('hidden');
        clientSearchResults.innerHTML = '';
        return;
    }
    
    try {
        const response = await fetch(`/Clients/SearchClients?search=${encodeURIComponent(searchTerm)}`);
        
        if (response.ok) {
            const clients = await response.json();
            
            if (clients.length === 0) {
                clientSearchResults.innerHTML = '<div class="p-3 text-gray-500 text-sm">No clients found</div>';
                clientSearchResults.classList.remove('hidden');
                return;
            }
            
            clientSearchResults.innerHTML = '';
            clients.forEach(client => {
                const div = document.createElement('div');
                div.className = 'p-3 hover:bg-gray-100 cursor-pointer border-b border-gray-200';
                div.innerHTML = `
                    <div class="font-semibold">${client.companyName}</div>
                    <div class="text-sm text-gray-600">CI/NIT: ${client.documentNumber} | Tel: ${client.phone || 'N/A'}</div>
                `;
                div.addEventListener('click', function() {
                    selectClient(client);
                });
                clientSearchResults.appendChild(div);
            });
            
            clientSearchResults.classList.remove('hidden');
        }
    } catch (error) {
        searchClientError.textContent = 'Error when searching for clients';
        console.error(error);
    }
});

// Cerrar resultados de clientes al hacer clic fuera
document.addEventListener('click', function(e) {
    if (!searchDocumentInput.contains(e.target) && !clientSearchResults.contains(e.target)) {
        clientSearchResults.classList.add('hidden');
    }
    if (!searchProductInput.contains(e.target) && !productSearchResults.contains(e.target)) {
        productSearchResults.classList.add('hidden');
    }
});

// Seleccionar cliente
function selectClient(client) {
    clientIdInput.value = client.id;
    clientNameInput.value = `${client.companyName} (${client.documentNumber})`;
    searchDocumentInput.value = client.documentNumber;
    clientSearchResults.classList.add('hidden');
    searchClientSuccess.textContent = `✓ Selected client: ${client.companyName}`;
    clientError.textContent = '';
}

// Limpiar cliente
btnClearClient.addEventListener('click', function() {
    searchDocumentInput.value = '';
    clientIdInput.value = '';
    clientNameInput.value = '';
    searchClientError.textContent = '';
    searchClientSuccess.textContent = '';
    clientError.textContent = '';
    clientSearchResults.classList.add('hidden');
});

// Renderizar carrito
function renderCart() {
    if (cart.length === 0) {
        productsList.innerHTML = '<p class="text-gray-500 text-center py-4">No products added</p>';
        return;
    }
    
    productsList.innerHTML = '';
    cart.forEach((item, index) => {
        const productCard = document.createElement('div');
        productCard.className = 'bg-gray-50 p-3 rounded-lg';
        productCard.innerHTML = `
            <div class="flex items-center justify-between">
                <div class="flex-1">
                    <p class="font-semibold">${item.Description}</p>
                    <p class="text-sm text-gray-600">Price: Bs. ${item.UnitPrice.toFixed(2)}</p>
                    <div class="mt-2">
                        <label class="text-sm text-gray-600">Amount:</label>
                        <input type="number" min="1" max="${item.MaxStock}" value="${item.Quantity}" 
                               id="quantity-${index}"
                               onchange="updateCartQuantity(${index}, this.value)"
                               class="w-20 px-2 py-1 border border-gray-300 rounded text-sm ml-2">
                        <span class="text-xs text-gray-500 ml-2">(Stock: ${item.MaxStock})</span>
                        <div id="error-${index}" class="text-red-600 text-xs mt-1"></div>
                    </div>
                </div>
                <div class="text-right mr-4">
                    <p class="font-bold text-green-600">Bs. ${item.Subtotal.toFixed(2)}</p>
                </div>
                <button type="button" onclick="removeFromCart(${index})" class="text-red-600 hover:text-red-800">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                    </svg>
                </button>
            </div>
        `;
        productsList.appendChild(productCard);
    });
}

// Actualizar cantidad en el carrito
function updateCartQuantity(index, newQuantity) {
    const quantity = parseInt(newQuantity);
    const item = cart[index];
    const errorDiv = document.getElementById(`error-${index}`);
    const inputField = document.getElementById(`quantity-${index}`);
    
    // Limpiar error previo
    if (errorDiv) errorDiv.textContent = '';
    
    if (isNaN(quantity) || quantity < 1) {
        if (errorDiv) errorDiv.textContent = 'Quantity must be greater than 0';
        if (inputField) inputField.value = item.Quantity;
        return;
    }
    
    if (quantity > item.MaxStock) {
        if (errorDiv) errorDiv.textContent = `Stock insuficiente. Disponible: ${item.MaxStock}`;
        if (inputField) inputField.value = item.Quantity;
        return;
    }
    
    cart[index].Quantity = quantity;
    cart[index].Subtotal = cart[index].UnitPrice * quantity;
    renderCart();
    calculateTotals();
}

// Eliminar producto del carrito
function removeFromCart(index) {
    cart.splice(index, 1);
    renderCart();
    calculateTotals();
}

// Calcular totales
function calculateTotals() {
    const subtotal = cart.reduce((sum, item) => sum + item.Subtotal, 0);
    const discount = parseFloat(discountInput.value) || 0;
    const total = subtotal - discount;
    
    subtotalDisplay.textContent = `Bs. ${subtotal.toFixed(2)}`;
    totalDisplay.textContent = `Bs. ${total.toFixed(2)}`;
}

// Actualizar totales cuando cambia el descuento
discountInput.addEventListener('input', calculateTotals);

// Validaciones
function validateClient() {
    const clientError = document.getElementById('clientError');
    if (!clientIdInput.value) {
        clientError.textContent = 'You must search and select a client';
        return false;
    }
    clientError.textContent = '';
    return true;
}

function validatePaymentType() {
    const paymentTypeError = document.getElementById('paymentTypeError');
    if (!paymentTypeSelect.value) {
        paymentTypeError.textContent = 'You must select a payment type';
        return false;
    }
    paymentTypeError.textContent = '';
    return true;
}

function validateProducts() {
    const productsError = document.getElementById('productsError');
    if (cart.length === 0) {
        productsError.textContent = 'You must add at least one product';
        return false;
    }
    productsError.textContent = '';
    return true;
}

function validateDiscount() {
    const discountError = document.getElementById('discountError');
    const discount = parseFloat(discountInput.value) || 0;
    const subtotal = cart.reduce((sum, item) => sum + item.Subtotal, 0);
    
    if (discount < 0) {
        discountError.textContent = 'The discount cannot be negative.';
        return false;
    }
    
    if (discount > subtotal) {
        discountError.textContent = 'The discount cannot be greater than the subtotal';
        return false;
    }
    
    discountError.textContent = '';
    return true;
}

// Enviar formulario
saleForm.addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const isClientValid = validateClient();
    const isPaymentTypeValid = validatePaymentType();
    const isProductsValid = validateProducts();
    const isDiscountValid = validateDiscount();
    
    if (!isClientValid || !isPaymentTypeValid || !isProductsValid || !isDiscountValid) {
        return;
    }
    
    const subtotal = cart.reduce((sum, item) => sum + item.Subtotal, 0);
    const discount = parseFloat(discountInput.value) || 0;
    const total = subtotal - discount;
    
    const saleData = {
        ClientId: parseInt(clientIdInput.value),
        PaymentType: paymentTypeSelect.value,
        Discount: discount,
        Total: total,
        Details: cart
    };
    
    try {
        btnSaveSale.disabled = true;
        btnSaveSale.textContent = 'Guardando...';
        
        const response = await fetch('/Sales/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(saleData)
        });
        
        if (response.ok) {
            const result = await response.json();
            showNotification(result.message || 'Sale successfully registered', 'success');
            setTimeout(() => {
                window.location.href = '/Sales/Index';
            }, 1500);
        } else {
            const result = await response.json();
            showNotification(result.message || 'Error saving the sale', 'error');
            btnSaveSale.disabled = false;
            btnSaveSale.textContent = 'Save Sale';
        }
    } catch (error) {
        showNotification('Error al comunicarse con el servidor', 'error');
        btnSaveSale.disabled = false;
        btnSaveSale.textContent = 'Save Sale';
    }
});

// Validaciones en tiempo real
paymentTypeSelect.addEventListener('change', validatePaymentType);
discountInput.addEventListener('blur', validateDiscount);
