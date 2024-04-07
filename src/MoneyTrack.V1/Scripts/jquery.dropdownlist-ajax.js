'use strict';

/**
 * @typedef LinkedDropDownList
 * @type {object}
 * @property {function} add - 增加一個單一選單至連動下拉式選單裡
 * @property {function} create - 建立連動式下拉選單
 * @property {function} refreshItems - 重新產生指定的下拉式選單項目
 * /
/**
 * @type {LinkedDropDownList} 連動式下拉式選單清單
 */
var LinkedDropDownList = function () {
    if (!(this instanceof LinkedDropDownList)) {
        return new LinkedDropDownList();
    }
    var dropDownLists = [];
    var that = this;

    /**
     * 增加一個單一選單至連動下拉式選單清單裡
     * @member {function}
     * @param {DropDownList} dropDownList 連動下拉式選單的單一選單
     */
    this.add = function (dropDownList) {

        if (!(dropDownList instanceof DropDownList)) {
            throw new 'dropDownList type is error';
        }
        dropDownList.linkedDropDownList = this;
        dropDownLists.push(dropDownList);
    };

    /**
     * 建立連動式下拉選單
     * @member {function}
     */
    this.create = function () {
        this.refreshItems(0);

        for (var i = 0; i < dropDownLists.length; i++) {
            $(dropDownLists[i].select).unbind().change({ index: i }, function (e) {
                // 只是想縮短變數名稱 XD
                var index = e.data.index;

                dropDownLists[index].hidden.value = this.value;

                // 選項如果內容有被改變，就將後面下拉式選單的初始值給清空
                for (var j = index + 1; j < dropDownLists.length; j++) {
                    dropDownLists[j].defaultValue = '';
                    dropDownLists[j].hidden.value = '';
                }
                dropDownLists[index].changeCallback(dropDownLists[index]);

                that.refreshItems(index + 1);
            });
        }
    };

    /**
     * 重新產生指定的下拉式選單項目
     * @member {function}
     * @param {number} index - 第幾個下拉式選單
     */
    this.refreshItems = function (index) {
        var thisDropDownList = dropDownLists[index] || null;
        var nextDropDownList = dropDownLists[index + 1] || null;
        var prevDropDownList = dropDownLists[index - 1] || null;

        if (!thisDropDownList) {
            return;
        }

        thisDropDownList.setQuering();

        // 記錄前一個下拉選單的值，第二個以後下拉選單才會有作用
        var postData = {};
        if (prevDropDownList != null) {
            postData.prevValue = prevDropDownList.select.value;
        }

        // 可能查詢會需要比前一個更早之前的Value
        var values = [];
        for (var i = 0; i < index; i++) {
            values.push(dropDownLists[i].select.value);
        }

        if (values.length > 0) {
            postData.values = values;
        }

        // 額外的查詢條件
        for (var paramKey in thisDropDownList.extraParams) {
            var value = thisDropDownList.extraParams[paramKey];
            // 試著從DOM取值
            if (typeof value === 'string' && value !== null) {
                var $elem = thisDropDownList.select.form ?
                    $(value, thisDropDownList.select.form) : $(value);
                if ($elem.length === 0) {
                    postData[paramKey] = value;
                } else {
                    var type = $elem.attr('type');
                    if (type === 'radio') {
                        postData[paramKey] = $elem.filter(':checked').val();
                    } else if (type === 'checkbox') {
                        postData[paramKey] = $elem.filter(':checked').map(function (elem) {
                            return elem.value;
                        });
                    } else {
                        postData[paramKey] = $elem.val();
                    }
                }
            } else {
                postData[paramKey] = value;
            }
        }

        if (prevDropDownList && !thisDropDownList.allowEmptyPrevValue() && prevDropDownList.select.value === '') {
            thisDropDownList.setNonUsed();

            if (nextDropDownList !== null) {
                that.refreshItems(index + 1);
            }
        } else {
            $.ajax({
                type: 'post',
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                url: thisDropDownList.ajaxUrl,
                data: JSON.stringify(postData),
                cache: false,
                async: false
            }).done(function (result) {
                thisDropDownList.clearItems();

                var resultObj = result.d ? result.d : result;
                if (resultObj.length > 0) {
                    if (thisDropDownList.hasOptionLabel() && resultObj.length > 1) {
                        thisDropDownList.addOptionLabelItem();
                    }

                    for (var i = 0; i < resultObj.length; i++) {
                        thisDropDownList.addItem(resultObj[i]['Text'], resultObj[i]['Value']);
                    }
                }

                thisDropDownList.changeCallback(thisDropDownList);

                if (nextDropDownList !== null) {
                    that.refreshItems(index + 1);
                }
            });
        }
    };
};

/**
 * 解析select的data-元素，並在select.DropDownList建立DropDownList
 * @function
 * @memberof LinkedDropDownList
 * @param {HTMLSelectElement} select - select的DOM
 * @return {DropDownList} DropDownList
 */
LinkedDropDownList.parseSelect = function (select) {
    var $select = $(select);
    if (!$select.data('dropDownList')) {
        $select.data('dropDownList', new DropDownList(select));
    }
    return $select.data('dropDownList');
};

/**
 * 解析jQuery表達式能找到的select，解析其data-元素，並在$(select).data('dropDownList')建立LinkedDropDownList.DropDownList
 * @function
 * @memberof LinkedDropDownList
 * @param {string|Element|JQuery} [jQuerySelector=form] - jQuery表達式、DOM或jQuery物件
 */
LinkedDropDownList.parse = function (jQuerySelector) {
    var $selects = $(jQuerySelector || 'input[type="hidden"]')
        .filter('[data-ajax-url]')
        .add($(jQuerySelector).find('[data-ajax-url]'));

    var groups = {};
    $selects.each(function () {
        var group = $(this).data('selectGroup') || '';
        if (!groups[group]) {
            groups[group] = [];
        }
        groups[group].push(LinkedDropDownList.parseSelect(this));
    });

    for (var group in groups) {
        var dropDownListLists = new LinkedDropDownList();
        for (var i = 0; i < groups[group].length; i++) {
            dropDownListLists.add(groups[group][i]);
        }
        dropDownListLists.create();
    }
};

/**
 * @typedef DropDownList
 * @type {object}
 * @property {string} ajaxUrl - ajaxUrl的url，對應data-ajax-url
 * @property {string} defaultValue - 對應data-defalut-value
 * @property {string} optionLabel - 是否要有空白選項，對應data-option-label
 * @property {string} nonUsedCaption - 上層select未選擇時顯示文字，對應data-non-used-label
 * @property {function} changeCallback - 執行change事件時，額外執行的內容
 * @property {object} extraParams - 傳遞至Ajax Server的額外參數，Key不可為prevValue和values
 * @property {LinkedDropDownList} DropDownLists - 所屬的LinkedDropDownList
 * /
/**
 * 連動式下拉式選單
 * @type {DropDownList}
 */
var DropDownList = (function () {
    /**
     * 從<select /> 解讀資訊產生連動下拉式選單的單一選單
     * @param {HTMLSelectElement} hidden - <input type="hidden" />
     * @returns {DropDownList} 連動下拉式選單的單一選單
     */
    function DropDownList(hidden) {
        if (!(this instanceof DropDownList)) {
            return new DropDownList(hidden);
        }

        this.hidden = hidden;
        this.ajaxUrl = '';
        this.defaultValue = '';
        this.optionLabel = null;
        this.nonUsedLabel = null;
        this.changeCallback = function () { };
        this.extraParams = {};
        this.linkedDropDownList = null;
        this.select = null;

        const hiddenKeys = [
            'ajaxUrl', 'optionLabel', 'nonUsedLabel', 'changeCallback', 'extraParams'
        ];

        for (var i in hiddenKeys) {
            var key = hiddenKeys[i];
            var value = $(hidden).data(key);

            if (value || value === "") {
                if (key === 'changeCallback' && typeof value === 'string') {
                    this[key] = eval(value);
                } else if (key === 'extraParams' && typeof value === 'object') {
                    this.extraParams = value;
                } else {
                    this[key] = value === null ? null : value.toString();
                }
            }
        }

        this.defaultValue = hidden.value;

        var that = this;

        createSelect();

        /**
         * 建立紀錄下拉選單選擇文字的hidden
         */
        function createSelect() {
            if (that.select !== null) {
                $(that.select).remove();
            }
            that.select = document.createElement('select');
            $(that.select).insertAfter(that.hidden);
        }
    };

    var _proto = DropDownList.prototype;

    /**
     * 增加一個選項
     * @param {string} text option的text
     * @param {string} value option的value
     */
    _proto.addItem = function (text, value) {
        var opts = this.select.options;
        opts.add(new Option(text, value));
        if (this.defaultValue === value) {
            this.select.value = value;
        }

        this.hidden.value = $(this.select).val();
    };

    /**
     * 清除全部的選項
     */
    _proto.clearItems = function () {
        this.select.options.length = 0;

        this.hidden.value = '';
    };

    /**
     * 增加一個空白的選項
     */
    _proto.addOptionLabelItem = function () {
        this.addItem(this.optionLabel, '');
    };

    /**
     * 清除全部的選項，並建立一個選項表示應先選上層下拉式選單內容
     */
    _proto.setNonUsed = function () {
        this.clearItems();
        this.addItem(this.nonUsedLabel, '');
    };

    /**
     * 清除全部的選項，並建立一個顯示查詢中的選項
     */
    _proto.setQuering = function () {
        this.clearItems();
        this.addItem('查詢中...', '');
    };

    _proto.hasOptionLabel = function () {
        return typeof this.optionLabel !== 'undefined' && this.optionLabel !== null;
    }

    _proto.allowEmptyPrevValue = function () {
        return typeof this.nonUsedLabel === 'undefined' || this.nonUsedLabel === null;
    }

    return DropDownList;
})();

$(function () {
    LinkedDropDownList.parse();
});