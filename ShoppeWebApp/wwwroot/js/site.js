const decreaseBtn = document.querySelector('.decrease');
const increaseBtn = document.querySelector('.increase');
const quantityInput = document.querySelector('.quantity-input');

increaseBtn.addEventListener('click', () => {
    let currentValue = parseInt(quantityInput.value) || 0;
    quantityInput.value = currentValue + 1;
    let soLuongKho = parseInt(document.getElementById('So-Luong-Kho').textContent) || 0;
    if (quantityInput.value < 1) {
        quantityInput.value = 1
    }
    if (quantityInput.value > soLuongKho) {
        quantityInput.value = soLuongKho
    }
});

decreaseBtn.addEventListener('click', () => {
    let currentValue = parseInt(quantityInput.value) || 0;
    if (currentValue > 1) {
        quantityInput.value = currentValue - 1;
    }
    let soLuongKho = parseInt(document.getElementById('So-Luong-Kho').textContent) || 0;
    if (quantityInput.value < 1) {
        quantityInput.value = 1
    }
    if (quantityInput.value > soLuongKho) {
        quantityInput.value = soLuongKho
    }
});

quantityInput.addEventListener('input', () => {
    quantityInput.value = quantityInput.value.replace(/[^0-9]/g, '');
    let soLuongKho = parseInt(document.getElementById('So-Luong-Kho').textContent) || 0;
    if (quantityInput.value < 1) {
        quantityInput.value = 1
    }
    if (quantityInput.value > soLuongKho) {
        quantityInput.value = soLuongKho
    }
});