function formatCurrency(value) {
    return new Intl.NumberFormat('en-US').format(value) + ' ₫';
}
function readImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#uploadedImage').attr('src', e.target.result);
            $('#uploadedImage').css('display', 'block');
        };

        reader.readAsDataURL(input.files[0]);
    }
}

