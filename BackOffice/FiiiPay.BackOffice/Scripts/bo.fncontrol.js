//tab
(function ($) {
    var defaults = {
        url: "",
        type: "get",
        title: "",
        pagename: "",
        closeable: true,
        onclose: function () { },
        para: {},
        ischildpage: false,
        pageid: "",
        path: ""
    }
    var tabheaderid = "#main-heading";
    var currentPagePath = "currentPageTitle";
    //生成唯一标志
    var createGuid = function () {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    }
    var gotolirow = function ($li) {
        var $ulheader = $li.parent();
        var ulwidth = $ulheader.width() || $ulheader[0].offsetWidth;
        var liwidth = 162;
        var countperrow = Math.floor(ulwidth / liwidth);
        var liindex = $li.index();
        var currentrow = Math.ceil(liindex / countperrow);
        $ulheader.data("currentrow", currentrow);
        movetabrow($ulheader.parent().prev().children(),0);
    }
    var movetabrow = function ($btn, rowrange, rowistotal) {
        if (rowrange!= 0 && $btn.attr("disabled")) {
            return;
        }
        var $headercontainer = $btn.parent().parent();
        var $ulheader = $headercontainer.find("ul");
        var ulwidth = $ulheader.width() || $ulheader[0].offsetWidth;
        var liwidth = 162;
        var countperrow = Math.floor(ulwidth / liwidth);
        var childcount = $ulheader.children().length - 1;
        var totalrow = Math.ceil(childcount / countperrow);
        var currentrow = rowistotal ? totalrow : Number($ulheader.data("currentrow"));
        currentrow = currentrow + rowrange;
        
        if (totalrow > 1 && !$ulheader.hasClass("mutilpage")) {
            $ulheader.parent().prev().removeClass("hidden");
            $ulheader.parent().next().removeClass("hidden");
            $ulheader.parent().prev().off().on("click","a", function () {
                movetabrow($(this), -1);
            });
            $ulheader.parent().next().off().on("click","a", function () {
                movetabrow($(this), 1);
            });
            $ulheader.addClass("mutilpage");
        }

        if (totalrow <= 1) {
            $headercontainer.find(".tabs-pagerbtn").addClass("hidden");
        }
        else {
            $headercontainer.find(".tabs-pagerbtn").removeClass("hidden");
        }
        if (currentrow >= totalrow) {
            $headercontainer.find(".tabs-pagerbtn").children().last().attr("disabled","disabled");
        }
        else {
            $headercontainer.find(".tabs-pagerbtn").children().last().removeAttr("disabled");
        }
        if (currentrow <= 1) {
            $headercontainer.find(".tabs-pagerbtn").children().first().attr("disabled", "disabled");
        }
        else {
            $headercontainer.find(".tabs-pagerbtn").children().first().removeAttr("disabled");
        }
        if ((rowrange != 0 && totalrow <= 1) || (rowrange!=0 && (currentrow > totalrow || currentrow < 0))) {
            return;
        }
        var showstartindex = (currentrow - 1) * countperrow;
        var showendindex = showstartindex + countperrow;
        $ulheader.children(':not(.uncloaseable)').each(function (liindex) {
            $(this).addClass("hidden");
            if (showstartindex <= liindex && liindex < showendindex) {
                $(this).removeClass("hidden");
            }
        });
        $ulheader.data("currentrow", currentrow);
    }
    //添加tab页
    var addPanel = function (obj, setting) {
        var pageId = setting.pagename + setting.pageid;
        pageId = pageId.replace(/^\s+|\s+|\s+$/g, '_');
        var tabHeader = obj.parent().find(tabheaderid);
        var tabContainer = obj.find(".tabs-container");
        var headerUl = tabHeader.find("ul");
        var excistLi = headerUl.children("#" + pageId);
        if (excistLi.length > 0) {
            showTab(obj, excistLi, setting);
            return;
        }
        var conContainer = tabContainer.children();
        var guid = createGuid();
        var prevActiveLi;
        if (setting.ischildpage) {
            prevActiveLi = headerUl.children(":not(:first)").filter(".active").first();
        }
        headerUl.find(".active").each(function () {
            $(this).removeClass("active")
        });
        conContainer.filter(".show").each(function () {
            $(this).removeClass("show").addClass('hidden')
        });
        var liItem = $('<li role="presentation" class="active" id="' + pageId + '"></li>');
        if (!setting.ischildpage) {
            liItem.attr("menuid", setting.pageid)
        }
        var alink = $('<a href="#">' + setting.title + '</a>');
        if (setting.closeable) {
            var closeBtn = $('<button type="button" class="close" aria-label="Close"></button>');
            closeBtn.append('<span aria-hidden="true">&times;</span>');
        }
        else {
            liItem.addClass("uncloaseable");
        }
        liItem.attr("target", guid);//data("target", guid);
        liItem.attr("title", setting.title);
        liItem.attr("currentPath", setting.path);
        liItem.append(alink);
        if (setting.closeable) {
            liItem.append(closeBtn);
            closeBtn.on("click", function () {
                closeTab(obj, closeBtn,setting);
            });
        }
        headerUl.append(liItem);
        alink.on("click", function () {
            showTab(obj, $(this).parent(), setting);
        });
        var containItem = $('<div class="show" id="' + guid + '" style="height:100%;width:100%;"></div>');
        var iframe = $('<iframe name=' + guid + ' frameborder="0" scrolling="no" style="width:100%; min-height:100%;"></iframe>');
        containItem.append(iframe)
        tabContainer.append(containItem);
        iframe.attr("src", setting.url);
        movetabrow(headerUl.parent().prev().children(), 0, true);
        var showTitle = setting.ischildpage ? setting.title : setting.path;
        showPagePath(obj, liItem, prevActiveLi, showTitle);
    }
    //关闭指定TAB
    var closeTab = function (obj, btn, setting) {
        var liItem = btn.parent();
        var $ul = liItem.parent();
        var target = liItem.attr("target");
        var isactive = liItem.hasClass("active");
        var prevobj = liItem.prev();
        var nextobj = liItem.next();
        if ($ul.children().length == 2) {
            enableTab(obj, $ul.children(":hidden").first());
        }
        else if (isactive) {
            //选中另一个选项卡
            if (prevobj.length > 0) {
                showTab(obj, prevobj, setting);
            }
            else {
                if (nextobj.length > 0) {
                    showTab(obj, nextobj, setting);
                }
            }
        }
        liItem.remove();
        obj.find(".tabs-container").find("#" + target).remove();
    }
    var showTab = function (obj, liItem, setting) {
        gotolirow(liItem);
        if (liItem.hasClass("active")) {
            return;
        }
        var activeItem = liItem.parent().children(".active");
        hideTab(obj, activeItem);
        var target = liItem.attr("target");
        liItem.addClass("active");
        var tabContainer = obj.find(".tabs-container").find("#" + target);
        tabContainer.removeClass("hidden").addClass("show");
        //执行页面刷新方法
        //eval('refreshPage("' + target + '")');
        var refreshBtn = obj.find("a#refreshBtn");;
        refreshBtn.attr("framename", target);
        refreshBtn.click();
        showPagePath(obj, liItem, liItem, "");
        if (setting && setting.ontabshow) {
            setting.ontabshow.call(liItem);
        }
    }
    var hideTab = function (obj, liItem) {
        var target = liItem.attr("target");//.data("target");
        liItem.removeClass("active");
        obj.find(".tabs-container").find("#" + target).removeClass("show").addClass("hidden");
    }
    var disableTab = function (obj, liItem) {
        var target = liItem.attr("target");
        liItem.removeClass("active").removeClass("show").addClass("hidden");
        obj.find(".tabs-container").find("#" + target).removeClass("show").addClass("hidden");
    }
    var enableTab = function (obj, liItem) {
        gotolirow(liItem);
        var target = liItem.attr("target");
        liItem.removeClass("hidden").addClass("show").addClass("active");
        obj.find(".tabs-container").find("#" + target).removeClass("hidden").addClass("show");
        showPagePath(obj, liItem, liItem, "");
    }
    function showPagePath(obj, currentLi, toActiveLi, toCurrentTitle) {
        var prevPath = '';
        if (toActiveLi && toActiveLi.length > 0) {
            prevPath = toActiveLi.attr("currentPath");
            if (toCurrentTitle && toCurrentTitle.length > 0) {
                prevPath += " / ";
            }
        }
        toCurrentTitle = prevPath + toCurrentTitle;
        if (currentLi && currentLi.length > 0) {
            currentLi.attr("currentPath", toCurrentTitle);
        }
        var pathHtml = obj.parents("body").find("#" + currentPagePath);
        pathHtml.html(toCurrentTitle);
    }
    var methods = {
        init: function (options) {
            var obj = $(this);
            var setting = $.extend({}, defaults, options[0] || {});
            var hideBtnHtml = '<div class="hiden"><a href="javascript:" id="refreshBtn" onclick="refreshPage(this)"></a></div>';
            var tabHeader = obj.parent().find(tabheaderid);
            var tabContainer = $('<div class="tabs-container"></div>');
            var tabHeaderHtml = '<ul class="nav nav-tabs nav-tabs-ul"></ul>';
            var tabHeaderContainer = $('<div style="display:table-cell"></div>');
            tabHeader.css({ "width": "100%","height":"38px", "display": "table" });
            tabHeader.append('<div class="tabs-pagerbtn hidden"><a href="javascript:"><span class="glyphicon glyphicon-chevron-left"></span></a></div>')
            tabHeader.append('<div style="display:table-cell">' + tabHeaderHtml+'</div>')
            tabHeader.append('<div class="tabs-pagerbtn hidden"><a href="javascript:"><span class="glyphicon glyphicon-chevron-right"></span></a></div>')
            
            obj.append(tabContainer).append(hideBtnHtml);
            addPanel(obj, setting);
        },
        addtab: function (option) {
            var obj = this;
            var liitems = obj.parent().find(tabheaderid).find("ul").children();
            var tabcount = liitems.length;
            var firsttab = liitems.first();
            if (firsttab.hasClass("uncloaseable")) {
                disableTab(obj, firsttab);
            }
            if (typeof (option) !== "undefined" && option.length > 0) {
                if (option[0] === "addtab") {
                    opt = option[1];
                }
                else {
                    opt = option[0];
                }
                var addOption = $.extend({}, defaults, opt);
                if (!addOption.pagename) {
                    var pagename = addOption.url;
                    var qindex = pagename.indexOf('?');
                    if (qindex && qindex>0) {
                        pagename = pagename.substring(0, qindex);
                    }
                    pagename = pagename.replace(/\//g, "");
                    addOption.pagename = pagename;
                }
                addPanel(obj, addOption);
            }
            else {
                $.error("need appoint a title");
            }
        },
        resettab: function () {
            var obj = this;
            var liactive = obj.parent().find(tabheaderid).find("ul").children(".active");
            gotolirow(liactive);
        },
        getcurrentpath: function () {
            var obj = this;
            var liactive = obj.parent().find(tabheaderid).find("ul").children(".active");
            return liactive.attr("currentPath");
        }
    };
    $.fn.botabs = function () {
        var options = arguments;
        var option = options[0];
        if (typeof (option) === 'object' || !option) {
            return this.each(function () {
                var obj = this;
                methods.init.call(obj, options);
            });
        }
        else if (typeof (option) === 'string') {
            return methods[option].call(this, options)
        }
        else {
            $.error("botabs get an invalid parameter");
        }
    };
})(jQuery);

//jqGrid封装
(function ($) {
    var defaults = {
        url: "",
        mtype: "POST",
        datatype: "json",
        autowidth: true,
        shrinkToFit: true,
        forceFit: false,
        altRows: false,
        height: "auto",
        rowNum: 10,
        rowList: [10, 15, 20],
        viewrecords: true,
        gridview: true,
        styleUI: 'Bootstrap',
        regional: 'cn',
        loadtext: "加载中...",
        prmNames: { page: 'Page', rows: 'Size', sort: 'SortColumn', order: 'OrderBy' },
        treeReader: {
            level_field: "level",
            parent_id_field: "parent",
            leaf_field: "isLeaf",
            expanded_field: "expanded"
        },
        colNames: [],
        colModel: [],
        sortname: "Id",
        sortorder: "ASC",
        gridComplete: function () {
            setiframeheight();
        },
        viewsortcols: [true, 'vertical', true],
        showBtn: false,
        btnHideinxs: true,
        hideView: false,
        hideEdit: false,
        hideDelete: false,
        showlefttoolbar: true,
        showadd: false,
        btnOpts: {
            width: 60,
            colTitle: "操作"
        },
        permvalue: "",
        onShowClick: "",
        onEditClick: "",
        onDeleteClick: "",
        noRecordText: "没有记录"
    }
    function createGrid(container) {
        var opt = $.data(container, 'BoGrid');
        if (opt && opt.options) {
            if (!opt.options.loadComplete) {
                opt.options.loadComplete = function () {
                    var rowNum = $(container).jqGrid('getGridParam', 'records');
                    var noRecordHtml = $("#boGrid-NoRecord");
                    if (!rowNum) {
                        if (noRecordHtml.length <= 0) {
                            noRecordHtml = $('<div id="boGrid-NoRecord" class="boGrid-NoRecord text-center">' + opt.options.noRecordText + '</div>');
                            $(container).parent().append(noRecordHtml);
                        }
                        noRecordHtml.show();
                    }
                    else {
                        noRecordHtml.hide();
                    }
                }
            }
            if (opt.options.showBtn) {
                opt.options.btnOpts.name = "optBtnLink";
                opt.options.btnOpts.formatter = showOptBtn;
                opt.options.btnOpts.sortable = false;
                opt.options.btnOpts.align = 'center';
                opt.options.btnOpts.hideinxs = opt.options.btnHideinxs;
                opt.options.btnOpts.isView = opt.options.permvalue.View && !opt.options.hideView;
                opt.options.btnOpts.isEdit = opt.options.permvalue.Update && !opt.options.hideEdit;
                opt.options.btnOpts.isDelete = opt.options.permvalue.Delete && !opt.options.hideDelete;
                opt.options.colNames.push(opt.options.btnOpts.colTitle);
                opt.options.colModel.push(opt.options.btnOpts);
                $.data(container, 'BoGrid', opt);
            }
            var topwidth = $(window.top).width();
            if (topwidth <= 768) {
                var colLengh = opt.options.colModel.length;
                for (var i = 0; i < colLengh; i++) {
                    if (opt.options.colModel[i].hideinxs) {
                        opt.options.colModel[i].hidden = true;
                    }
                }
                opt.options.pginput = false;
                opt.options.pagerpos = 'left';
                opt.options.showlefttoolbar = false;
            }
            $(container).addClass("bo-grid");
            $(container).jqGrid(opt.options);
            if (opt.options.showlefttoolbar) {
                $(container).navGrid(opt.options.pager, {
                    search: false, // show search button on the toolbar
                    add: false,
                    edit: false,
                    del: false,
                    refresh: true,
                    position: 'left'
                });
                if (opt.options.showadd && opt.options.permvalue.Create) {
                    $(container).navButtonAdd(opt.options.pager, {
                        caption: '',
                        onClickButton: opt.options.onAddclick,
                        postion: 'last',
                        buttonicon: 'glyphicon-plus gridaddbtn'
                    });
                }
            }
        }
    }
    function showOptBtn(cellvalue, options, rowdata) {
        var opt = $.data(this, 'BoGrid');
        if (opt && opt.options) {
            var param = options.rowId;
            if (!param) {
                param = rowdata;
            }
            var linkStr = '';
            var addMargin = false;
            if (options.colModel.isView) {
                linkStr += '<a class="grid-optbtn" title="View"';
                if (opt.options.onShowClick && opt.options.onShowClick.length > 0)
                    linkStr += ' onclick = "' + opt.options.onShowClick + '(\'' + param + '\')"';
                linkStr += '><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a>';
                addMargin = true;
            }
            if (options.colModel.isEdit) {
                linkStr += '<a class="grid-optbtn" title="Update"';
                if (opt.options.onEditClick && opt.options.onEditClick.length > 0)
                    linkStr += ' onclick = "' + opt.options.onEditClick + '(\'' + param + '\')"';
                if (addMargin) {
                    linkStr += ' style="margin-left:8px;"';
                }
                linkStr += '><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span></a>';
                addMargin = true;
            }
            if (options.colModel.isDelete) {
                linkStr += '<a class="grid-optbtn" title="Delete"';
                if (opt.options.onDeleteClick && opt.options.onDeleteClick.length > 0)
                    linkStr += ' onclick = "' + opt.options.onDeleteClick + '(\'' + param + '\')"';
                if (addMargin) {
                    linkStr += ' style="margin-left:8px;"';
                }
                linkStr += '><span class="glyphicon glyphicon-remove" aria-hidden="true"></span></a>';
            }
            return linkStr;
        }
        return cellvalue;
    }
    $.fn.BoGrid = function (options, param) {
        if (typeof options === 'string') {
            return $.fn.BoGrid.methods[options](this, param);
        }
        if (options.permvalue) {
            if (options.permvalue.length <= 0) {
                options.permvalue = {}
            }
            else {
                options.permvalue = $.parseJSON(options.permvalue.replace(/&quot;/gm, '"'));
            }
        }
        options = options || {};
        return this.each(function () {
            var state = $.data(this, 'BoGrid');
            if (state) {
                $.data(this, 'BoGrid', {
                    options: $.extend(true, {}, state.options, options)
                });
            }
            else {
                $.data(this, 'BoGrid', {
                    options: $.extend(true, {}, defaults, options)
                });
                createGrid(this);
            }
        });
    }
})(jQuery);
//validate封装
(function ($) {
    var defaults = {
        errorElement: 'span',
        errorClass: 'help-inline',
        onsubmit: true,
        onkeyup: function (element) { $(element).valid(); },
        onclick: false,
        focusInvalid: true,
        onfocusin: false,
        onfocusout: function (element) { $(element).valid(); },
        highlight: function (e) {
            $(e).closest('.form-group').removeClass('info').addClass('has-error');
        },
        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error').addClass('info');
            $(e).remove();
        },
        errorPlacement: function (error, element) {
            var parantelement = element.parent();
            if (parantelement.hasClass('input-group')) {
                error.appendTo(parantelement.parent());
            }
            else {
                error.appendTo(parantelement);
            }
        }
    }
    function createValidate(options) {
        var obj = $(this);
        var setting = $.extend({}, defaults, options || {});
        obj.validate(setting);
    }

    $.fn.jqValidate = function (options, param) {
        if (typeof options === 'string') {
            return $.fn.validate.methods[options](this, param);
        }
        return this.each(function () {
            createValidate.call(this, options);
        });
    }
})(jQuery);

//
(function ($) {
    var defaults = {
        btnName: 'Select',
        placeholder: 'Click select button',
        pageid: '1',
        dataurl: '',
        param: {},
        pageTitle: 'Select',
        widthStr: ''
    }

    var methods = {
        Init: function (options) {
            var obj = $(this);
            var inputitem = this;
            var setting = $.extend({}, defaults, options || {});
            obj.addClass("form-control").attr("readonly", "readonly").attr("placeholder", setting.placeholder);
            if (setting.value) {
                obj.val(setting.text).attr("datavalue", setting.value);
            }
            var divContainer = $('<div class="input-group"></div>');
            var spanBtn = $('<span class="input-group-btn"></span>');
            var selectBtn = $('<button class="btn btn-info" type="button" style="margin-top:0;">' + setting.btnName + '</button>');
            selectBtn.on('click', function () {
                $.openmodalpage(setting.pageid, setting.dataurl, setting.param, setting.pageTitle, setting.widthStr, function (data) {
                    if (data) {
                        $.data(inputitem, "selecteddata", data);
                        obj.val(data.text).attr("datavalue", data.value);
                        if (setting.callback) {
                            setting.callback.call(obj, data);
                        }
                        obj.valid();
                    }
                });
            });
            spanBtn.append(selectBtn);
            obj.after(divContainer);
            obj.remove();
            divContainer.append(obj).append(spanBtn);
        },
        getData: function () {
            return $(this).data('selecteddata');
        },
        empty: function () {
            $(this).val('').removeAttr("datavalue");
        }
    }

    $.fn.SelectData = function (method, options, param) {
        if (typeof options === 'string') {
            return methods[options].call(this, param);
        }
        if (!options) {
            options = {};
        }
        switch (method) {
            case 'selectaccount':
                options.pageid = 'publicControls_SelecteAccount';
                options.dataurl = '/Controls/AccountSelect';
                break;
            case 'selectpackage':
                options.pageid = 'publicControls_SelectePackage';
                options.dataurl = '/Controls/PackageSelect';
                break;
            case 'selectroomtype':
                options.pageid = 'publicControls_SelecteRoomType';
                options.dataurl = '/Controls/RoomTypeSelect';
                break;
        }
        return this.each(function () {
            methods['Init'].call(this, options);
        });
    }
})(jQuery);