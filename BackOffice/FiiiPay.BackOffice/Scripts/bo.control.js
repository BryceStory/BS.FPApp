jQuery.extend({
    "openmodalpage": function (pageid, url, para, title, widthstr, onclose) {
        var excistDiv = $("#bootstrap-modal-" + pageid);
        if (excistDiv.length > 0) {
            return;
        }
        var modalfadeclass;
        var modaldialogclass;
        switch (widthstr) {
            case "big":
                modalfadeclass = "bs-example-modal-lg";
                modaldialogclass = "modal-lg";
                break;
            case "small":
                modalfadeclass = "bs-example-modal-sm";
                modaldialogclass = "modal-sm";
                break;
            case "normal":
            default:
                modalfadeclass = "";
                modaldialogclass = "modal-xs";
                break;
        }

        var modelCount = $(".modal-backdrop").length;
        $.post(url, para, function (html) {
            var sh = $(window.parent).scrollTop();

            modaldiv = $('<div class="modal '+modalfadeclass+'" id="bootstrap-modal-' + pageid + '" tabindex="-1" role="dialog" aria-labelledby="bootstrap-modal-label"></div>');
            var modalhtml = '<div class="modal-dialog ' + modaldialogclass + ' modal-content-page" role="document" style="top:' + sh +'px;">';
            modalhtml += '<div class="modal-content">';
            modalhtml += '<div class="modal-header">';
            modalhtml += '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>';
            modalhtml += '<h4 class="modal-title" id="bootstrap-modal-label">' + title + '</h4></div>';
            modalhtml += '<div class="modal-body" style="min-height:200px;">';
            modalhtml += html;
            modalhtml += '</div></div></div>';
            modaldiv.append(modalhtml);
            $("body").append(modaldiv);
            modaldiv.on('hide.bs.modal', function (e) {
                if (onclose) {
                    var arg = modaldiv.data("closedata");
                    onclose.call(this, arg);
                }
                $(this).remove();
            });
            modaldiv.on('shown.bs.modal', function (e) {
                var grid = $(".bo-grid");
                if (grid && grid.length > 0) {
                    grid.each(function () {
                        var $this = $(this);
                        var gridContainor = $this.parents('.jqgridContainor');
                        if (gridContainor.length > 0) {
                            $this.setGridWidth(gridContainor.width()-2, true);
                        } else {
                            $this.setGridWidth($(window).width()-2, true);
                        }
                    });
                }
            });
            modaldiv.modal({ backdrop: 'static', keyboard: false });
        });
    },
    "closemodalpage": function (pageid, closedata) {
        var modaldiv = $("#bootstrap-modal-" + pageid);
        if (modaldiv.length <= 0) {
            return;
        }
        modaldiv.data("closedata", closedata);
        modaldiv.modal('hide');
    },
    "opentabpage": function (url, title, pageid) {
        var mainpage = window.top.document.getElementById('main-content');
        $(mainpage).botabs("addtab", { url: url, title: title, pageid: pageid, ischildpage: true });
    },
    "openloadding": function () {
        var loadDiv = $('<div class="fiiipay-bo-loadding"></div>');
        var loadimg = '<div class="fiiipay-bo-loadimg"><span></span><span></span><span></span><span></span><span></span><span></span><span></span><span></span></div>';
        loadDiv.append(loadimg);
        $("body").append(loadDiv);
    },
    "closeloadding": function () {
        var loadDiv = $(".fiiipay-bo-loadding");
        loadDiv.remove();
    },
    "swconfirm": function (text,onclose) {
        swal({
            title: "",
            text: text,
            showCancelButton: "true",
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning",
            closeOnConfirm: false
        }, onclose);
    },
    "dateformat": function (timestamp, fmt) {
        if (!timestamp) {
            return "";
        }
        if (!fmt) {
            fmt = "yyyy-MM-dd HH:mm"
        }
        var o = {
            "M+": timestamp.getMonth() + 1, //月份 
            "d+": timestamp.getDate(), //日 
            "h+": timestamp.getHours(), //小时 
            "H+": timestamp.getHours(), //小时 
            "m+": timestamp.getMinutes(), //分 
            "s+": timestamp.getSeconds(), //秒 
            "q+": Math.floor((timestamp.getMonth() + 3) / 3), //季度 
            "S": timestamp.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (timestamp.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    },
    "ndateformat": function (datetxt,fmt) {
        if (!datetxt) {
            return "";
        }
        if (!fmt) {
            fmt="yyyy-MM-dd HH:mm"
        }
        var timestamp = eval("new Date(" + datetxt.replace("/Date(", "").replace(")/", "") + ")");

        var o = {
            "M+": timestamp.getMonth() + 1, //月份 
            "d+": timestamp.getDate(), //日 
            "h+": timestamp.getHours(), //小时 
            "H+": timestamp.getHours(), //小时 
            "m+": timestamp.getMinutes(), //分 
            "s+": timestamp.getSeconds(), //秒 
            "q+": Math.floor((timestamp.getMonth() + 3) / 3), //季度 
            "S": timestamp.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (timestamp.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    },
    "autoTextarea": function (elem, extra, maxHeight) {
        extra = extra || 0;
        var isFirefox = !!document.getBoxObjectFor || 'mozInnerScreenX' in window,
            isOpera = !!window.opera && !!window.opera.toString().indexOf('Opera'),
            addEvent = function (type, callback) {
                elem.addEventListener ?
                    elem.addEventListener(type, callback, false) :
                    elem.attachEvent('on' + type, callback);
            },
            getStyle = elem.currentStyle ?
                function (name) {
                    var val = elem.currentStyle[name];
                    if (name === 'height' && val.search(/px/i) !== 1) {
                        var rect = elem.getBoundingClientRect();
                        return rect.bottom - rect.top -
                            parseFloat(getStyle('paddingTop')) -
                            parseFloat(getStyle('paddingBottom')) + 'px';
                    };
                    return val;
                } : function (name) {
                    return getComputedStyle(elem, null)[name];
                },
            minHeight = parseFloat(getStyle('height'));

        var change = function () {
            var scrollTop, height,
                padding = 0,
                style = elem.style;

            if (elem._length === elem.value.length) return;
            elem._length = elem.value.length;

            if (!isFirefox && !isOpera) {
                padding = parseInt(getStyle('paddingTop')) + parseInt(getStyle('paddingBottom'));
            };
            scrollTop = document.body.scrollTop || document.documentElement.scrollTop;

            elem.style.height = minHeight + 'px';
            if (elem.scrollHeight > minHeight) {
                if (maxHeight && elem.scrollHeight > maxHeight) {
                    height = maxHeight - padding;
                    style.overflowY = 'auto';
                } else {
                    height = elem.scrollHeight - padding;
                    style.overflowY = 'hidden';
                };
                style.height = height + extra + 'px';
                scrollTop += parseInt(style.height) - elem.currHeight;
                document.body.scrollTop = scrollTop;
                document.documentElement.scrollTop = scrollTop;
                elem.currHeight = parseInt(style.height);
            };
        };

        addEvent('propertychange', change);
        addEvent('input', change);
        addEvent('focus', change);
        change();
    }
});