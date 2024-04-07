$(function () {
    $(document).delegate('.delete', 'click', function () {
        let msg = "是否確定刪除資料？";

        return confirm(msg);
    });

    // ref:https://blog.darkthread.net/blog/webform-ie-submit-twice/
    let body = document.body;
    body.isPosting = false;

    document.getElementsByTagName("form").onsubmit = function () {
        if (body.isPosting) {
            return false;
        }

        body.isPosting = true;
        return true;
    };
});