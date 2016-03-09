(function ($) {
    var defaultOptions = {
        errorClass: 'has-error',
        validClass: 'has-success',
        highlight: function (element, errorClass, validClass) {
            $(element).closest(".form-group")
                .addClass('has-error')
                .removeClass(validClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).closest(".form-group")
            .removeClass('has-error')
            .addClass(validClass);
        }
    };

    $.validator.setDefaults(defaultOptions);

    $.validator.unobtrusive.options = {
        errorClass: defaultOptions.errorClass,
        validClass: defaultOptions.validClass,
    };
})(jQuery);

$(".phoneno-mask").mask("(999) 999-9999");


function parseCurrency(num) {
    return num.replace(/[^0-9\.]+/g, "");
}

function paymentCaliculation(cardtype, totalProductsPrices) {
    var processFeeValue = 0, conventionFee = 0, result = {};
    //percentage caliculator on basis of card type
    switch (cardtype) {
        case "discover":
        case "mastercard":
        case "visa": { result.processFeeValue = Math.round(((totalProductsPrices / 100) * 8) * 100) / 100; result.conventionFee = 8; break };
        case "amex": { result.processFeeValue = Math.round(((totalProductsPrices / 100) * 10) * 100) / 100; result.conventionFee = 10; break };
        case "echeck": { result.processFeeValue = Math.round(((totalProductsPrices / 100) * 10.75) * 100) / 100; result.conventionFee = 10.75; break };
        default: { result.processFeeValue = Math.round(((totalProductsPrices / 100) * 8) * 100) / 100; result.conventionFee = 8; break };
    }
    return result;
   
}