const config = {
    locale: 'zh_TW',
    events: 'change|blur'
};
Vue.use(VeeValidate, config);

Vue.directive('formatter', {
    bind: function (el, binding) {
        if ((el.tagName == 'INPUT' && el.type !== 'checkbox' && el.type !== 'radio' && el.type !== 'file')
            || el.tagName == 'TEXTARE'
        ) {
            let eventName = binding.arg || 'blur';
            el.addEventListener(eventName, function () {
                if (typeof binding.value === 'function') {
                    el.value = binding.value(el.value);
                }

                if (binding.modifiers.trim) {
                    el.value = el.value.trim();
                }

                if (binding.modifiers.lowercase) {
                    el.value = el.value.toLowerCase();
                }

                if (binding.modifiers.uppercase) {
                    el.value = el.value.toUpperCase();
                }
            });
        }
    }
});

Vue.directive('confirm', {
    bind: function (el, binding) {
        if ((el.tagName === 'INPUT' && (el.type === 'submit' || el.type === 'button'))
            || el.tagName === 'BUTTON') {
            el.addEventListener('click', function (event) {
                let message = null;
                if (binding.modifiers.delete) {
                    message = '是否確定刪除資料？';
                }

                if (!message || !confirm(message)) {
                    event.preventDefault();
                }
            });
        }
    }
});

// ref:https://medium.com/@wikycheng/handle-duplicate-ajax-request-and-abort-the-pending-previous-http-call-when-route-changing-using-3706d769f9e6
const pending = {};
const removePending = (config, f) => {
    // make sure the url is same for both request and response
    const url = config.url.replace(config.baseURL, '/');
    // stringify whole RESTful request with URL params
    const flagUrl = url + '&'
        + config.method + '&'
        + JSON.stringify(config.params);
    if (flagUrl in pending) {
        if (f) {
            f(); // abort the request
        } else {
            delete pending[flagUrl];
        }
    } else {
        if (f) {
            pending[flagUrl] = f; // store the cancel function
        }
    }
}

axios.interceptors.request.use(
    config => {
        config.cancelToken = new axios.CancelToken((c) => {
            removePending(config, c);
        });
        let token = document.querySelector('input[name="__RequestVerificationToken"]');
        if (token !== null) {
            config.headers = {
                RequestVerificationToken: token.value
            }
        }
        return config;
    },
    error => {
        return Promise.reject(error);
    }
);
axios.interceptors.response.use(
    response => {
        removePending(response.config);
        return response;
    },
    error => {
        removePending(error.config);

        if (!axios.isCancel(error)) {
            return Promise.reject(error);
        } else {
            // return empty object for aborted request
            return Promise.resolve({});
        }
    }
);