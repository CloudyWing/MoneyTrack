(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined'
        ? factory(exports)
        : typeof define === 'function' && define.amd
            ? define(['exports'], factory)
            : (global = typeof globalThis !== 'undefined'
                ? globalThis
                : global || self, factory(global.VeeValidateRules));
})(this, (function (exports) {
    'use strict';

    function isEmpty(value) {
        if (value === null || value === undefined || value === '') {
            return true;
        }
        if (Array.isArray(value) && value.length === 0) {
            return true;
        }
        return false;
    }

    function validateCreditCardRule(value) {
        let sum = 0;
        let digit;
        let tmpNum;
        let shouldDouble;

        let sanitized = value.replace(/[- ]+/g, '');

        for (let i = sanitized.length - 1; i >= 0; i--) {
            digit = sanitized.substring(i, i + 1);
            tmpNum = parseInt(digit, 10);

            if (shouldDouble) {
                tmpNum *= 2;

                if (tmpNum >= 10) {
                    sum += tmpNum % 10 + 1;
                } else {
                    sum += tmpNum;
                }
            } else {
                sum += tmpNum;
            }

            shouldDouble = !shouldDouble;
        }

        return !!(sum % 10 === 0 ? sanitized : false);
    }

    const creditCardValidator = (value) => {
        if (isEmpty(value)) {
            return true;
        }
        const re = /^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|(222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11}|6[27][0-9]{14})$/;
        if (Array.isArray(value)) {
            return value.every(val => re.test(String(val)) && validateCreditCardRule(String(val)));
        }
        return re.test(String(value)) && validateCreditCardRule(String(val));
    };

    const urlValidator = (value) => {
        if (isEmpty(value)) {
            return true;
        }
        const re = /^(?:(?:(?:https?|ftp):)?\/\/)(?:\S+(?::\S*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})).?)(?::\d{2,5})?(?:[/?#]\S*)?$/i;
        if (Array.isArray(value)) {
            return value.every(val => re.test(String(val)));
        }
        return re.test(String(value));
    }

    /* eslint-disable camelcase */
    exports["default"].credit_card = creditCardValidator;
    exports["default"].url = urlValidator;

    exports.credit_card = creditCardValidator;
    exports.url = urlValidator;

    Object.defineProperty(exports, '__esModule', { value: true });

}));